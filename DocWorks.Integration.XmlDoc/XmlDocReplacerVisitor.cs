using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DocWorks.Integration.XmlDoc.Extensions;

namespace DocWorks.Integration.XmlDoc
{
    internal class XmlDocReplacerVisitor : CSharpSyntaxRewriter
    {
        private SemanticModel _semanticModel;
        private XmlDocument _xmlDoc;
        private PartialTypeInfoCollectorVisitor _partialTypeInfoCollector;

        public XmlDocReplacerVisitor(string docXml, PartialTypeInfoCollectorVisitor partialInfoCollector) : base(true)
        {
            _partialTypeInfoCollector = partialInfoCollector;
            _xmlDoc = new XmlDocument();
            _xmlDoc.LoadXml(docXml);

        }

        internal SyntaxNode Visit(SyntaxNode rootNode, SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            return Visit(rootNode);
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            var updatedNode = base.Visit(node);
            if (node is BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
                return AddOrUpdateXmlDoc(baseTypeDeclarationSyntax, (BaseTypeDeclarationSyntax)updatedNode);

            if (node is BaseFieldDeclarationSyntax baseFieldDeclarationSyntax)
                return VisitBaseFieldDeclaration(baseFieldDeclarationSyntax);

            if (node is MemberDeclarationSyntax memberDeclarationSyntax && !(node is NamespaceDeclarationSyntax))
                return AddOrUpdateXmlDoc(memberDeclarationSyntax, (MemberDeclarationSyntax)updatedNode);

            return updatedNode;
        }

        private bool isVisitingField = false;
        private SyntaxNode VisitBaseFieldDeclaration(BaseFieldDeclarationSyntax node)
        {
            //for Field declarations, the Xml may be on the field itself or on the variable declarator.
            // ex. on the field
            //   ///<summary>Docs</summary>
            //   int value;
            // ex. on the variable declarator
            //   int
            //      ///<summary>Docs</summary>
            //      value;
            // ex. with two declarations
            //   int value,
            //      ///<summary>Docs</summary>
            //      value2;

            // We handle this by visiting the Field declaration and the variable declarations, treating them the same way we treat partial classes.
            isVisitingField = true;
            try
            {
                var symbol = _semanticModel.GetDeclaredSymbol(node.Declaration.Variables[0]);
                var docNodes = XmlDocNodeForMember(symbol);
                var updatedNode = base.Visit(node);
                return AddOrUpdateXmlDoc(node, updatedNode, docNodes, symbol);
            }
            finally
            {
                isVisitingField = false;
            }
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (!isVisitingField)
                return node;

            var symbol = _semanticModel.GetDeclaredSymbol(node);
            Debug.Assert(symbol != null, "No symbol found for field");

            var docNodes = XmlDocNodeForMember(symbol);

            var updatedNode = AddOrUpdateXmlDoc(node, node, docNodes, symbol);
            return updatedNode;
        }

        private SyntaxNode AddOrUpdateXmlDoc(BaseTypeDeclarationSyntax originalNode, BaseTypeDeclarationSyntax nodeToUpdate)
        {
            var typeSymbol = _semanticModel.GetDeclaredSymbol(originalNode);

            var typeNode = SelectTypeNode(typeSymbol);
            if (typeNode == null)
                return nodeToUpdate;

            var docNode = typeNode["xmldoc"];
            if (docNode == null)
                return nodeToUpdate;

            return AddOrUpdateXmlDoc(originalNode, nodeToUpdate, docNode, typeSymbol);
        }

        private XmlNode SelectTypeNode(INamedTypeSymbol typeSymbol)
        {
            var selector =
                new StringBuilder($"@name='{typeSymbol.MetadataName}' and @namespace='{typeSymbol.ContainingNamespace.FullyQualifiedName(true, NameFormat.MetadataName)}'");
            if (typeSymbol.ContainingType != null)
                selector.Append($" and @containingType='{typeSymbol.ContainingType.FullyQualifiedName(false, NameFormat.MetadataName)}'");

            var docNodes = _xmlDoc.SelectNodes($"descendant::member[{selector}]");
            if (docNodes.Count > 1)
            {
                throw new DuplicateMemberException($@"Multiple matches for ""{typeSymbol}"" found in xml.\nMatches:\n{string.Join("\n", docNodes.Cast<XmlNode>().Select(n => n.OuterXml))}");
            }

            if (docNodes.Count == 0)
                return null;

            return docNodes[0];
        }

        private SyntaxNode AddOrUpdateXmlDoc(MemberDeclarationSyntax originalNode, MemberDeclarationSyntax nodeToUpdate)
        {
            var symbol = _semanticModel.GetDeclaredSymbol(originalNode);

            XmlNode parentNode;
            if (symbol is INamedTypeSymbol)
                parentNode = _xmlDoc.SelectSingleNode("doc");
            else
                parentNode = SelectTypeNode(symbol.ContainingType);

            if (parentNode == null)
                return nodeToUpdate;

            var constraints = "";

            ImmutableArray<IParameterSymbol> parameters = default(ImmutableArray<IParameterSymbol>);
            if (symbol is IMethodSymbol)
                parameters = ((IMethodSymbol) symbol).Parameters;

            if (symbol is IPropertySymbol)
                parameters = ((IPropertySymbol) symbol).Parameters;

            if (!parameters.IsDefault)
            {
                int xpathIdx = 1;
                foreach (var parameter in parameters)
                {
                    var parameterType = parameter.Type;
                    if (parameterType is ITypeParameterSymbol)
                        constraints += $@" and signature/parameters/parameter[{xpathIdx}]/typeParameter/@name='{parameterType.Name}'";
                    else
                    {
                        var constraintPrefix = $@" and signature/parameters/parameter[{xpathIdx}]/";
                        constraints = AddTypeConstraints(constraints, constraintPrefix, parameterType);
                    }

                    xpathIdx++;
                }
                constraints += $@" and not(signature/parameters/parameter[{xpathIdx}])";
            }

            var docNodes = parentNode.SelectNodes($"member[@name='{symbol.MemberNameUnescaped()}'{constraints}]/xmldoc");

            return AddOrUpdateXmlDoc(originalNode, nodeToUpdate, docNodes, symbol);
        }

        private static string AddTypeConstraints(string constraints, string constraintPrefix, ITypeSymbol parameterType)
        {
            var isTypeParameter = parameterType is ITypeParameterSymbol;
            if (isTypeParameter)
                constraints += $@"{constraintPrefix}typeParameter/@name='{parameterType.Name}'";
            else
                constraints += $@"{constraintPrefix}type/@typeId='{parameterType.Id()}'";

            if (parameterType is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
            {
                foreach (var typeParameterSymbol in namedTypeSymbol.TypeArguments)
                {
                    var subTypeConstraintPrefix = $@"{constraintPrefix}type[@typeId='{parameterType.Id()}']/typeArguments/";
                    constraints = AddTypeConstraints(constraints, subTypeConstraintPrefix, typeParameterSymbol);
                }
            }

            return constraints;
        }

        private SyntaxNode AddOrUpdateXmlDoc(SyntaxNode originalNode, SyntaxNode nodeToBeUpdated, XmlNodeList matchingDocNodes, ISymbol symbol)
        {
            if (matchingDocNodes.Count > 1)
            {
                throw new DuplicateMemberException($@"Multiple matches for ""{symbol}"" found in xml.\nMatches:\n{string.Join("\n", matchingDocNodes.Cast<XmlNode>().Select(n => n.OuterXml))}");
            }

            if (matchingDocNodes.Count == 0)
                return nodeToBeUpdated;

            var docNode = matchingDocNodes[0];

            return AddOrUpdateXmlDoc(originalNode, nodeToBeUpdated, docNode, symbol);
        }

        private SyntaxNode AddOrUpdateXmlDoc(SyntaxNode originalNode, SyntaxNode nodeToBeUpdated, XmlNode docNode,
            ISymbol symbol)
        {
            SyntaxTriviaList docTrivia = nodeToBeUpdated.GetLeadingTrivia();

            var lastNewlineIndex = docTrivia.ToFullString().LastIndexOf('\n') + docTrivia.FullSpan.Start;
            SyntaxTrivia[] initialWhitespace = docTrivia.Where(t => t.FullSpan.Start > lastNewlineIndex && t.IsKind(SyntaxKind.WhitespaceTrivia)).ToArray();
//            int lastNewLineIndex = Array.FindLastIndex(initialWhitespace, t => t.Kind() != SyntaxKind.WhitespaceTrivia);
//            if (lastNewLineIndex >= 0)
//                initialWhitespace = initialWhitespace.Skip(lastNewLineIndex + 1).ToArray();

            if (nodeToBeUpdated.ContainsDirectives)
            {
                initialWhitespace = initialWhitespace.Take(1).ToArray();
            }
            string rawWhitespace = string.Join("", initialWhitespace.Select(t => t.ToFullString()));

            // break lines (to inject ///) at both CRLF & LF.
            string comment = string.Join("\r\n" + rawWhitespace, 
                docNode.InnerText.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(item => "/// " + item));

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(comment);
            SyntaxNode xmlDocumentNode = syntaxTree.GetRoot();

            SyntaxTriviaList newTrivia = SyntaxFactory.TriviaList();

            bool shouldUpdate = _partialTypeInfoCollector.ShouldThisNodeBeDocumented(originalNode, symbol);
            foreach (SyntaxTrivia trivia in docTrivia)
            {
                if (!trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    newTrivia = newTrivia.Add(trivia);
                }
                else if (shouldUpdate)
                {
                    shouldUpdate = false;
                    newTrivia = newTrivia.AddRange(xmlDocumentNode.GetLeadingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                }
            }

            if (shouldUpdate && !string.IsNullOrEmpty(comment))
            {
                newTrivia = newTrivia.AddRange(xmlDocumentNode.GetLeadingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed).AddRange(initialWhitespace));
            }

            return nodeToBeUpdated.WithLeadingTrivia(newTrivia);
        }

        private XmlNodeList XmlDocNodeForMember(ISymbol symbol)
        {
            return _xmlDoc.SelectNodes($"descendant::member[@name='{symbol.ContainingType.MetadataName}']/member[@name='{symbol.MemberNameUnescaped()}']/xmldoc");
        }

    }
}