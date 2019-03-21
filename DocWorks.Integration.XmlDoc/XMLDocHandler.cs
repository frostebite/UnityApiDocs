using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DocWorks.Integration.XmlDoc.Extensions;
using ISymbolExtensions = DocWorks.Integration.XmlDoc.Extensions.ISymbolExtensions;

namespace DocWorks.Integration.XmlDoc
{
    public class CompilationParameters
    {
        public IEnumerable<string> DefinedSymbols { get; }
        public string RootPath { get; }
        public IEnumerable<string> ExcludedPaths { get; }
        public IEnumerable<string> ReferencedAssemblyPaths { get; }

        public CompilationParameters(string rootPath, IEnumerable<string> excludedPaths, IEnumerable<string> definedSymbols, IEnumerable<string> referencedAssemblyPaths)
        {
            ExcludedPaths = (excludedPaths ?? new string[0]).Select(Path.GetFullPath).ToArray();
            DefinedSymbols = definedSymbols ?? throw new ArgumentNullException(nameof(definedSymbols));
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            ReferencedAssemblyPaths = referencedAssemblyPaths ?? throw new ArgumentNullException(nameof(referencedAssemblyPaths));
        }
    }

    public class XMLDocHandler
    {
        private CompilationParameters compilationParameters;
        private CSharpCompilation csharpCompilation;
        private Dictionary<string, SyntaxTree> treesForPaths;

        public XMLDocHandler(CompilationParameters compilationParameters)
        {
            this.compilationParameters = compilationParameters;
        }

        public string GetTypesXml()
        {
            var compilationParametersRootPath = Path.GetFullPath(compilationParameters.RootPath);
            if (!Directory.Exists(compilationParametersRootPath))
                throw new ArgumentException($"Directory \"{compilationParametersRootPath}\" does not exist.");

            var parserOptions = new CSharpParseOptions(LanguageVersion.CSharp6, DocumentationMode.Parse, SourceCodeKind.Regular, compilationParameters.DefinedSymbols);

            var filePaths = GetFullPathsFromDirectory(compilationParametersRootPath);

            var startIndex = compilationParametersRootPath.Length + (compilationParametersRootPath.EndsWith("\\") || compilationParametersRootPath.EndsWith("/") ? 0 : 1);
            var syntaxTrees = filePaths.Select(
                p =>
                {
                    return SyntaxFactory.ParseSyntaxTree(File.ReadAllText(p), parserOptions, p.Substring(startIndex));
                }).ToArray();

            var compilerOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            compilerOptions = compilerOptions.WithAllowUnsafe(true);
            var compilation = CSharpCompilation.Create("Test", syntaxTrees, GetMetadataReferences(), compilerOptions);

            var getTypesVisitor = new GetTypesVisitor();
            foreach (var syntaxTree in syntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                getTypesVisitor.Visit(syntaxTree.GetRoot(), semanticModel);
            }

            EnsureCompiled();

            return FormatXml(getTypesVisitor.GetXml());
        }

        public string GetTypeDocumentation(string id, params string[] paths)
        {
            EnsureCompiled();

            var fullPaths = GetFullPathsFromRelativePath(paths);

            var extraMemberRegEx = new Regex("\\<member name=[^\\>]+\\>|\\</member\\>", RegexOptions.Compiled);
            foreach (var path in fullPaths)
            {
                SyntaxTree syntaxTree;
                if (!treesForPaths.TryGetValue(path, out syntaxTree))
                    throw new ArgumentException("File \"" + path + "\" does not exist or was not found under root \"" + compilationParameters.RootPath + "\"");

                var semanticModel = csharpCompilation.GetSemanticModel(syntaxTree);

                var descendants = syntaxTree.GetRoot().DescendantNodes();
                var res = descendants.OfType<BaseTypeDeclarationSyntax>().Cast<MemberDeclarationSyntax>().Concat(descendants.OfType<DelegateDeclarationSyntax>()).ToArray();
                foreach (var typeDeclaration in res)
                {
                    var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
                    if (id == typeSymbol.QualifiedName(true, NameFormat.MetadataName))
                    {
                        var containingType = typeSymbol.ContainingType != null ?
                            $@"containingType=""{typeSymbol.ContainingType.FullyQualifiedName(false, NameFormat.MetadataName)}"" " :
                            string.Empty;



                        string xmlAttributes = "";
                        var baseType = BaseType(typeSymbol);
                        if (!string.IsNullOrEmpty(baseType))
                            xmlAttributes = $@" inherits=""{baseType}""";
                        if (typeSymbol.IsStatic)
                            xmlAttributes += @" isStatic=""true""";
                        if (typeSymbol.IsSealed && !typeSymbol.IsValueType)
                            xmlAttributes += @" isSealed=""true""";

                        var extraContent = "";
                        if (typeDeclaration is DelegateDeclarationSyntax)
                        {
                            extraContent += $@"<signature>
{SignatureFor(typeSymbol.DelegateInvokeMethod)}
</signature>";
                        }

                        extraContent += AttributesXml(typeSymbol);

                        var xml = new StringBuilder($@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""{typeSymbol.MetadataName}"" type=""{typeSymbol.TypeKind}"" {containingType}namespace=""{typeSymbol.ContainingNamespace.FullyQualifiedName(true, NameFormat.MetadataName)}""{xmlAttributes}>
        {InterfaceList(typeSymbol)}
        {TypeParametersXmlForDeclaration(typeSymbol.TypeParameters)}
        {extraContent}
        <xmldoc>
            { GetCDataDocXml(extraMemberRegEx, typeSymbol)}
        </xmldoc>");

                        var members = typeSymbol.GetMembers()
                            .Where(m => m.Kind != SymbolKind.NamedType &&
                                        !m.IsImplicitlyDeclared &&
                                        m.MayHaveXmlDoc());

                        foreach (var member in members)
                        {
                            string methodAttributes = "";
                            var memberName = ISymbolExtensions.MemberName(member);

                            int typeParameterCount = 0;
                            if (member.Kind == SymbolKind.Method)
                            {
                                var methodSymbol = (IMethodSymbol)member;

                                methodAttributes = $@" methodKind=""{methodSymbol.MethodKind}""";
                                if (methodSymbol.IsStatic)
                                    methodAttributes += @" isStatic=""true""";
                                if (methodSymbol.IsExtensionMethod)
                                    methodAttributes += @" isExtensionMethod=""true""";
                            }

                            xml.Append($@"<member name=""{memberName}"" type=""{member.Kind}""{methodAttributes}>
            <signature>{SignatureFor(member)}</signature>
            {AttributesXml(member)}
            <xmldoc>
                { GetCDataDocXml(extraMemberRegEx, member) }
            </xmldoc>
        </member>
");
                        }

                        xml.Append(@"</member></doc>");
                        var formatedXml = FormatXml(xml.ToString());
                        return formatedXml;
                    }
                }
            }

            throw new Exception($"Type not found Id={id}");
        }

        private static string GetCDataDocXml(Regex extraMemberRegEx, ISymbol typeSymbol)
        {
            var xml = typeSymbol.GetDocumentationCommentXml();
            xml = extraMemberRegEx.Replace(xml, "");
            xml = string.Join("\r\n", xml.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Substring(4)));
            xml = XmlUtility.LegalString(xml);
            //escape end of CDATA tags
            xml = xml.Replace("]]>", "]]]]><![CDATA[>");
            return $@"<![CDATA[{xml}]]>";
        }

        private string FormatXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        private void EnsureCompiled()
        {
            if (treesForPaths != null)
                return;

            treesForPaths = new Dictionary<string, SyntaxTree>();
            var parserOptions = new CSharpParseOptions(LanguageVersion.CSharp6, DocumentationMode.Parse,
                SourceCodeKind.Regular, compilationParameters.DefinedSymbols);

            var csFilePaths = GetFullPathsFromDirectory(compilationParameters.RootPath);
            
            var syntaxTrees = csFilePaths.Select(
                p =>
                {
                    Regex regexGetTripleSlashContent = new Regex("([\r\n ]*///(.*?)\r?\n)+");
                    string csFileContent = File.ReadAllText(p);
                    MatchCollection matchCollection = regexGetTripleSlashContent.Matches(csFileContent);
                    foreach (Match match in matchCollection)
                    {
                        string pattern = @"(?<=<([a-z][^>]*?)>)(.*?)(?=<\/[a-z]*>)";
                        Regex regexGetXmlTagContent = new Regex(pattern, RegexOptions.Singleline);
                        MatchCollection xmlTagMatchCollection = regexGetXmlTagContent.Matches(match.Value);
                        foreach (Match xmlTagMatch in xmlTagMatchCollection)
                        {
                            if (!string.IsNullOrEmpty(xmlTagMatch.Value))
                            {
                                string convertedContent = XmlUtility.EscapeString(xmlTagMatch.Value);
                                csFileContent = csFileContent.Replace(xmlTagMatch.Value, convertedContent);
                            }
                        }
                    }
                    var syntaxTree = SyntaxFactory.ParseSyntaxTree(csFileContent, parserOptions, p);
                    treesForPaths[p] = syntaxTree;
                    return syntaxTree;
                }).ToArray();

            var compilerOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            compilerOptions = compilerOptions.WithAllowUnsafe(true);
            csharpCompilation = CSharpCompilation.Create("Test", syntaxTrees, GetMetadataReferences(), compilerOptions);
        }

        private static string AttributesXml(ISymbol typeSymbol)
        {
            var attributeData = typeSymbol.GetAttributes();
            return AttributesXml(attributeData);
        }

        private static string AttributesXml(ImmutableArray<AttributeData> attributeData)
        {
            var attributes = attributeData.Where(a => a.AttributeClass.IsPublicApi()).ToArray();
            var attributeXml = string.Empty;
            if (attributes.Length > 0)
            {
                attributeXml = $@"<attributes>
{string.Join("\n", attributes.Select(AttributeXml))}
</attributes>";
            }

            return attributeXml;
        }

        private static string AttributeXml(AttributeData attribute)
        {
            var tag = $@"attribute typeId=""{attribute.AttributeClass.Id()}""";
            var namedArguments = attribute.NamedArguments;
            var constructorArguments = attribute.ConstructorArguments;
            if (constructorArguments.IsEmpty && namedArguments.IsEmpty)
                return $@"<{tag}/>";
            else
            {
                StringBuilder sb = new StringBuilder($@"<{tag}>");
                if (!constructorArguments.IsEmpty)
                {
                    sb.AppendLine("<constructorArguments>");
                    foreach (var argument in constructorArguments)
                    {
                        sb.AppendLine($@"<argument value=""{XmlUtility.EscapeString(argument.ToCSharpStringNoStringQuotes())}"">");
                        sb.AppendLine(TypeReferenceXml(argument.Type));
                        sb.AppendLine("</argument>");
                    }

                    sb.AppendLine("</constructorArguments>");
                }
                if (!namedArguments.IsEmpty)
                {
                    sb.AppendLine("<namedArguments>");
                    foreach (var argument in namedArguments)
                    {
                        sb.AppendLine($@"<argument name=""{argument.Key}"" value=""{XmlUtility.EscapeString(argument.Value.ToCSharpStringNoStringQuotes())}"">");
                        sb.AppendLine(TypeReferenceXml(argument.Value.Type));
                        sb.AppendLine("</argument>");

                    }
                    sb.AppendLine("</namedArguments>");
                }

                sb.AppendLine("</attribute>");
                return sb.ToString();
            }
        }

        private IEnumerable<PortableExecutableReference> GetMetadataReferences()
        {
            var platformAssembliesString = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")?.ToString();
            IEnumerable<string> assemblies;
            if (platformAssembliesString != null)
            {
                assemblies = platformAssembliesString.Split(Path.PathSeparator)
                    .Where(p => !p.Contains("DocWorks.Integration"));
            }
            else
                assemblies = new[] { typeof(object).Assembly.Location };

            return compilationParameters.ReferencedAssemblyPaths.Concat(assemblies).Select(p => MetadataReference.CreateFromFile(p));
        }

        private static string BaseType(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind == TypeKind.Interface || typeSymbol.TypeKind == TypeKind.Struct)
                return null;

            if (typeSymbol.BaseType.FullyQualifiedName(true, NameFormat.MetadataName) == "System.Object")
                return null;

            return typeSymbol.BaseType.Id();
        }

        private string InterfaceList(INamedTypeSymbol typeSymbol)
        {
            List<INamedTypeSymbol> interfaces = new List<INamedTypeSymbol>(typeSymbol.Interfaces);
            if (typeSymbol.BaseType?.TypeKind == TypeKind.Interface)
                interfaces.Add(typeSymbol.BaseType);

            if (interfaces.Count == 0)
                return String.Empty;

            return $@"<interfaces>
{String.Join(Environment.NewLine, interfaces.Select(i => TypeReferenceXml(i)))}
</interfaces>";
        }

        private string SignatureFor(ISymbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Field:
                    {

                        var field = (IFieldSymbol)member;
                        var typeXml = TypeReferenceXml(field.Type);
                        var accessibilityXml = AccessibilityXml(member.DeclaredAccessibility);
                        return $@"
{accessibilityXml}
{typeXml}";
                    }
                case SymbolKind.Method:
                    {
                        var method = (IMethodSymbol)member;
                        string returnXml = "";
                        if (method.Name != ".ctor")
                        {
                            var returnTypeAttributes = method.GetReturnTypeAttributes();
                            returnXml = $@"<return>
    {TypeReferenceXml(method.ReturnType)}
    {AttributesXml(returnTypeAttributes)}
</return>";
                        }

                        return $@"
{AccessibilityXml(member.DeclaredAccessibility)}
{returnXml}
<parameters>{ParametersSignature(method.Parameters)}</parameters>
{TypeParametersXmlForDeclaration(method.TypeParameters)}";
                    }

                case SymbolKind.Property:
                    {
                        var property = (IPropertySymbol)member;
                        var typeXml = TypeReferenceXml(property.Type);
                        var accessorsXml = string.Empty;

                        if (property.GetMethod != null && property.GetMethod.IsPublicApi())
                        {
                            accessorsXml = $"\n<get><accessibility>{property.GetMethod.DeclaredAccessibility}</accessibility></get>";
                        }

                        if (property.SetMethod != null && property.SetMethod.IsPublicApi())
                        {
                            accessorsXml += $"\n<set><accessibility>{property.SetMethod.DeclaredAccessibility}</accessibility></set>";
                        }


                        return $@"
{AccessibilityXml(member.DeclaredAccessibility)}
{typeXml}
{accessorsXml}
<parameters>{ParametersSignature(property.Parameters)}</parameters>";
                    }

                case SymbolKind.Event:
                    {
                        return $@"
{AccessibilityXml(member.DeclaredAccessibility)}
{TypeReferenceXml(((IEventSymbol)member).Type)}";
                    }
                default:
                    throw new NotSupportedException($"Unsupported type {member.Kind} : {member.Name}");
            }
        }

        private static string TypeParametersXmlForDeclaration(ImmutableArray<ITypeParameterSymbol> typeParameters)
        {
            if (typeParameters.IsEmpty)
                return "";

            return $@"<typeParameters>
{string.Join((string)"\n", typeParameters.Select(p => TypeParameterXml(p, true, false)))}
</typeParameters>";
        }

        private static string AccessibilityXml(Accessibility accessibility)
        {
            return $@"<accessibility>{accessibility}</accessibility>";
        }

        private static string TypeReferenceXml(ITypeSymbol typeSymbol, bool includeMetaInfo = false)
        {

            var sourceTypeParameterSymbol = typeSymbol as ITypeParameterSymbol;
            if (sourceTypeParameterSymbol != null)
            {
                return TypeParameterXml(sourceTypeParameterSymbol, includeMetaInfo, true);
            }
            var typeTagAttributes =
                $"typeId=\"{typeSymbol.Id()}\" typeName=\"{XmlUtility.EscapeString(typeSymbol.ToDisplayString())}\"";

            if (typeSymbol is IArrayTypeSymbol)
            {
                return
                    $@"<type {typeTagAttributes}>
    {TypeReferenceXml(((IArrayTypeSymbol)typeSymbol).ElementType)}
    </type>";
            }
            if (typeSymbol is IPointerTypeSymbol)
            {
                return
                    $@"<type {typeTagAttributes}>
    {TypeReferenceXml(((IPointerTypeSymbol)typeSymbol).PointedAtType)}
    </type>";
            }

            var namedTypeSymbol = typeSymbol as INamedTypeSymbol;
            if (namedTypeSymbol != null && namedTypeSymbol.IsGenericType)
            {
                var typeArguments = TypeArguments(namedTypeSymbol.TypeArguments);

                return
                    $@"<type {typeTagAttributes}>
    {typeArguments}
    </type>";
            }
            else
                return $"<type {typeTagAttributes}/>";
        }

        private static string TypeParameterXml(ITypeParameterSymbol sourceTypeParameterSymbol, bool includeMetaInfo, bool includeDeclaringTypeId)
        {
            string constraintAttributes = "";
            string body = string.Empty;
            string suffix = "/>";
            if (includeMetaInfo)
            {
                if (sourceTypeParameterSymbol.HasConstructorConstraint)
                    constraintAttributes += @" hasConstructorConstraint=""true""";
                if (sourceTypeParameterSymbol.HasReferenceTypeConstraint)
                    constraintAttributes += @" hasReferenceTypeConstraint=""true""";
                if (sourceTypeParameterSymbol.HasValueTypeConstraint)
                    constraintAttributes += @" hasValueTypeConstraint=""true""";

                if (!sourceTypeParameterSymbol.ConstraintTypes.IsEmpty)
                {
                    body += $@"{string.Join("\n", sourceTypeParameterSymbol.ConstraintTypes.Select(c => TypeReferenceXml(c)))}";
                }

                var attributesXml = AttributesXml(sourceTypeParameterSymbol.GetAttributes());
                if (!string.IsNullOrEmpty(attributesXml))
                {
                    body += attributesXml;
                }
            }

            string declaringTypeId = "";
            if (includeDeclaringTypeId)
                declaringTypeId = $@" declaringTypeId=""{sourceTypeParameterSymbol.DeclaringType.Id()}""";

            string typeParameterTag = $@"typeParameter{declaringTypeId} name=""{sourceTypeParameterSymbol.Name}""{constraintAttributes}";
            if (!string.IsNullOrEmpty(body))
            {
                return $@"<{typeParameterTag}>
{body}
</typeParameter>";
            }
            else
                return $"<{typeParameterTag}/>";
        }

        private static string TypeArguments(ImmutableArray<ITypeSymbol> typeArguments)
        {
            if (typeArguments.IsEmpty)
                return "";

            string typeArgumentsXml = "";
            foreach (var typeArgument in typeArguments)
                typeArgumentsXml += TypeReferenceXml(typeArgument);

            return $@"<typeArguments>
{typeArgumentsXml}
</typeArguments>";
        }

        private string ParametersSignature(IEnumerable<IParameterSymbol> parameters)
        {
            var sb = new StringBuilder();
            foreach (var parameter in parameters)
            {
                string paramsAttribute = parameter.IsParams ? @" isParams=""true""" : "";
                string optionalAttribute = parameter.IsOptional ? @" isOptional=""true""" : "";
                string defaultValueAttribute;
                ParameterSyntax theParameter;
                if (parameter.HasExplicitDefaultValue)
                {
                    string defaultValue;
                    if (parameter.ExplicitDefaultValue == null)
                    {
                        var parameterWithDefault = parameter.DeclaringSyntaxReferences.Select(reference => (ParameterSyntax)reference.GetSyntax()).FirstOrDefault(p => p.Default != null);
                        if (parameterWithDefault != null)
                            defaultValue = parameterWithDefault.Default.Value.ToFullString();
                        else
                            throw new NotSupportedException("Unsupported default value declaration: " + parameter.ToDisplayString());
                    }
                    else
                        defaultValue = parameter.ExplicitDefaultValue.ToString();

                    var isString = parameter.Type.SpecialType == SpecialType.System_String;
                    if (isString)
                        defaultValue = XmlUtility.EscapeString(defaultValue);

                    defaultValueAttribute = $@" defaultValue=""{defaultValue}""";
                }
                else
                    defaultValueAttribute = "";

                string parameterTag =
                    $"parameter name=\"{parameter.Name}\"{paramsAttribute}{optionalAttribute}{defaultValueAttribute}";


                var attributesXml = AttributesXml(parameter);

                sb.AppendLine($@"<{parameterTag}>
    {TypeReferenceXml(parameter.Type)}
    {attributesXml}
</parameter>");
            }
            return sb.ToString();
        }

        public void SetType(string docXml, params string[] sourcePaths)
        {
            EnsureCompiled();

            var fullPaths = GetFullPathsFromRelativePath(sourcePaths);

            var nonExistantFile = fullPaths.FirstOrDefault(p => !File.Exists(p));
            if (nonExistantFile != null)
                throw new FileNotFoundException(nonExistantFile + " does not exist");
            var nonScriptFile = sourcePaths.FirstOrDefault(p => !string.Equals(".cs", Path.GetExtension(p), StringComparison.OrdinalIgnoreCase));
            if (nonScriptFile != null)
                throw new ArgumentException(nonScriptFile + " is not a .cs file. Only .cs files are supported.");

            var partialInfoCollector = new PartialTypeInfoCollectorVisitor(docXml);
            foreach (var fullPath in fullPaths)
            {
                SyntaxTree syntaxTree;
                if (!treesForPaths.TryGetValue(fullPath, out syntaxTree))
                    throw new ArgumentException(fullPath + " is not contained in root path " + compilationParameters.RootPath);

                var semanticModel = csharpCompilation.GetSemanticModel(syntaxTree);
                partialInfoCollector.Visit(syntaxTree.GetRoot(), semanticModel);
            }

            var docUpdater = new XmlDocReplacerVisitor(docXml, partialInfoCollector);
            foreach (var fullPath in fullPaths)
            {
                var syntaxTree = treesForPaths[fullPath];
                var semanticModel = csharpCompilation.GetSemanticModel(syntaxTree);
                var result = docUpdater.Visit(syntaxTree.GetRoot(), semanticModel);

                if (result != syntaxTree.GetRoot())
                {
                    string fileContent = XmlUtility.LegalString(result.ToFullString());
                    File.WriteAllText(syntaxTree.FilePath, fileContent);
                }
            }
        }
        
        private IEnumerable<string> GetFullPathsFromRelativePath(string[] relativeFilePaths)
        {
            return relativeFilePaths.Select(p => Path.GetFullPath(Path.Combine(compilationParameters.RootPath, p))).Where(p => !compilationParameters.ExcludedPaths.Any(p.StartsWith));
        }

        private IEnumerable<string> GetFullPathsFromDirectory(string rootDirectory)
        {
            return Directory.GetFiles(rootDirectory, "*.cs", SearchOption.AllDirectories)
                .Select(Path.GetFullPath)
                .Where(p => !compilationParameters.ExcludedPaths.Any(p.StartsWith));
        }
    }

    public class PartialTypeInfoCollectorVisitor : CSharpSyntaxWalker
    {

        /**
         * 1) Find the right SyntaxNode should be updated (a)
         * 2) Add all other partial SyntaxNode into a list (b)
         * 3) In the *Updater Visitor* take that list and
         *      3.1) When visiting node *a* update the documentation
         *      3.2) When visiting node from *b* just wipe out the xml comments
         */
        private readonly XmlDocument _xmlDoc;
        private SemanticModel _semanticModel;
        private Dictionary<string, SyntaxNode> _documentationTargetTypeNodes = new Dictionary<string, SyntaxNode>();

        public PartialTypeInfoCollectorVisitor(string content)
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.LoadXml(content);
        }

        internal void Visit(SyntaxNode syntaxNode, SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            Visit(syntaxNode);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);
            DecidePriority(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            base.VisitInterfaceDeclaration(node);
            DecidePriority(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            base.VisitStructDeclaration(node);
            DecidePriority(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            base.VisitEnumDeclaration(node);
            DecidePriority(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
            DecidePriority(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);
            DecidePriority(node);
        }


        public override void Visit(SyntaxNode node)
        {
            base.Visit(node);

            if (node is BaseFieldDeclarationSyntax baseFieldDeclarationSyntax)
                VisitBaseFieldDeclaration(baseFieldDeclarationSyntax);
            else if (node is BaseTypeDeclarationSyntax
                     || node is MemberDeclarationSyntax
                     || (node is VariableDeclaratorSyntax && isVisitingField))
                DecidePriority(node);
        }

        private bool isVisitingField = false;
        public void VisitBaseFieldDeclaration(BaseFieldDeclarationSyntax node)
        {
            isVisitingField = true;
            try
            {
                DecidePriority(node, _semanticModel.GetDeclaredSymbol(node.Declaration.Variables[0]));
                base.Visit(node);
            }
            finally
            {
                isVisitingField = false;
            }
        }

        private void DecidePriority(SyntaxNode node)
        {
            var typeSymbol = _semanticModel.GetDeclaredSymbol(node);
            if (typeSymbol == null)
                return;

            DecidePriority(node, typeSymbol);
        }

        private void DecidePriority(SyntaxNode node, ISymbol typeSymbol)
        {
            var id = typeSymbol.Id();
            SyntaxNode existingTargetNode;
            if (_documentationTargetTypeNodes.TryGetValue(id, out existingTargetNode))
            {
                if (CecilUtility.HasXmlDocs(node) && !CecilUtility.HasXmlDocs(existingTargetNode))
                    _documentationTargetTypeNodes[id] = node;
            }
            else
                _documentationTargetTypeNodes[id] = node;
        }

        public bool ShouldThisNodeBeDocumented(SyntaxNode node, ISymbol symbol)
        {
            if (symbol == null)
                return false;

            SyntaxNode targetTypeNode;
            if (!_documentationTargetTypeNodes.TryGetValue(symbol.Id(), out targetTypeNode))
                throw new NotSupportedException("This type of node has not been prioritized yet.");

            return targetTypeNode == node;
        }
    }

    internal class XMLDocExtractVisitor : CSharpSyntaxWalker
    {
        private readonly string _id;
        private SemanticModel _semanticModel;

        public XMLDocExtractVisitor(string id) : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            _id = id;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var symbol = _semanticModel.GetDeclaredSymbol(node);
            var symbolId = symbol.QualifiedName(true, NameFormat.MetadataName);
            if (symbolId == _id)
            {

                return;
            }

            base.VisitClassDeclaration(node);
        }

        internal void Visit(SyntaxNode syntaxNode, SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }
    }
}