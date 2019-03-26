using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DocWorks.Integration.XmlDoc.Extensions
{
    public enum NameFormat
    {
        Name,
        MetadataName,
        NameWithGenericArguments
    }
    public static class ISymbolExtensions
    {
        public static string QualifiedName(this ISymbol symbol, bool includeNamespace, NameFormat nameFormat)
        {
            if (symbol == null)
                return null;

            if (symbol is IArrayTypeSymbol)
            {
                var arraySymbol = (IArrayTypeSymbol) symbol;
                var elementName = QualifiedName(arraySymbol.ElementType, includeNamespace, nameFormat);
                return $"{elementName}[{string.Join(" ", Enumerable.Repeat(',', arraySymbol.Rank - 1))}]";
            }

            if (symbol is IPointerTypeSymbol)
            {
                var pointerSymbol = (IPointerTypeSymbol)symbol;
                return $"{pointerSymbol.PointedAtType.QualifiedName(includeNamespace, nameFormat)}*";
            }

            if (symbol is INamespaceSymbol && ((INamespaceSymbol) symbol).IsGlobalNamespace)
                return string.Empty;

            string name;
            switch (nameFormat)
            {
                case NameFormat.Name:
                    name = symbol.Name;
                    break;
                case NameFormat.MetadataName:
                {
                    if (symbol is IMethodSymbol methodSymbol)
                        name = methodSymbol.MemberName();
                    else
                        name = symbol.MetadataName;

                    break;
                }
                case NameFormat.NameWithGenericArguments:
                {
                    var typeArguments = GetTypeArguments(symbol);

                    name = symbol.Name;
                    if (!typeArguments.IsDefaultOrEmpty)
                        name += $@"<{string.Join(", ", typeArguments)}>";

                    break;
                }
                default:
                    throw new NotSupportedException("Unsupported NameFormat");
            }

            string prefix = null;
            if (symbol.ContainingType != null)
                prefix = QualifiedName(symbol.ContainingType, includeNamespace, nameFormat);
            else if (includeNamespace && symbol.ContainingNamespace  != null && !symbol.ContainingNamespace.IsGlobalNamespace)
                prefix = QualifiedName(symbol.ContainingNamespace, includeNamespace, nameFormat);

            string separator = symbol is IMethodSymbol ? "::" : ".";

            return prefix != null ? prefix + separator + name : name;
        }

        internal static ImmutableArray<ITypeSymbol> GetTypeArguments(this ISymbol symbol)
        {
            ImmutableArray<ITypeSymbol> typeArguments;
            if (symbol is IMethodSymbol methodSymbol)
                typeArguments = methodSymbol.TypeArguments;
            else if (symbol is INamedTypeSymbol namedTypeSymbol)
                typeArguments = namedTypeSymbol.TypeArguments;
            else
                typeArguments = default(ImmutableArray<ITypeSymbol>);
            return typeArguments;
        }

        internal static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(this ISymbol symbol)
        {
            ImmutableArray<ITypeParameterSymbol> typeParameters;
            if (symbol is IMethodSymbol methodSymbol)
                typeParameters = methodSymbol.TypeParameters;
            else if (symbol is INamedTypeSymbol namedTypeSymbol)
                typeParameters = namedTypeSymbol.TypeParameters;
            else
                typeParameters = default(ImmutableArray<ITypeParameterSymbol>);
            return typeParameters;
        }


        internal static string Id(this ISymbol symbol)
        {
            if (symbol is IMethodSymbol methodSymbol)
                return Id(methodSymbol);
            if (symbol is IPropertySymbol propertySymbol && !propertySymbol.Parameters.IsDefaultOrEmpty)
                return $"{FullyQualifiedName(symbol, true, NameFormat.MetadataName)}[{string.Join(", ", propertySymbol.Parameters.Select(p => p.Type.Id()))}]";

            return FullyQualifiedName(symbol, true, NameFormat.MetadataName);
        }

        internal static string Id(this IMethodSymbol symbol)
        {
            string id = $@"{FullyQualifiedName(symbol.ReturnType, true, NameFormat.NameWithGenericArguments)} {FullyQualifiedName(symbol, true, NameFormat.NameWithGenericArguments)}({string.Join(", ", symbol.Parameters.Select(ParameterId))})";
            if (symbol.IsStatic)
                id = "static " + id;

            return id;
        }

        private static string ParameterId(IParameterSymbol p)
        {
            return $@"{p.RefKind} " + FullyQualifiedName(p.Type, true, NameFormat.NameWithGenericArguments); //Refactor later - if ref kind is none this is not needed
        }

        internal static string FullyQualifiedName(this ISymbol t, bool includeNamespace, NameFormat nameFormat)
        {
            return t.QualifiedName(includeNamespace, nameFormat);
        }

        internal static bool MayHaveXmlDoc(this ISymbol symbol)
        {
            if (symbol is IMethodSymbol methodSymbol)
            {
                switch (methodSymbol.MethodKind)
                {
                    case MethodKind.Ordinary:
                    case MethodKind.UserDefinedOperator:
                    case MethodKind.Destructor:
                    case MethodKind.Constructor:
                    case MethodKind.Conversion:
                        break;
                    case MethodKind.StaticConstructor:
                    case MethodKind.EventAdd:
                    case MethodKind.EventRemove:
                    case MethodKind.EventRaise:
                    case MethodKind.PropertyGet:
                    case MethodKind.PropertySet:
                        return false;
                    case MethodKind.ExplicitInterfaceImplementation:
                        return true;
                    case MethodKind.AnonymousFunction:
                    case MethodKind.DelegateInvoke:
                    case MethodKind.BuiltinOperator:
                    case MethodKind.ReducedExtension:
                    case MethodKind.DeclareMethod:
                        throw new NotSupportedException($"Unsupported MethodKind: {methodSymbol.MethodKind}");
                }
            }

            return IsPublicApi(symbol);
        }

        public static bool IsPublicApi(this ISymbol symbol)
        {
            if (symbol.ContainingType != null && !IsPublicApi(symbol.ContainingType))
                return false;

            var accessibility = symbol.DeclaredAccessibility;
            return accessibility == Accessibility.Public ||
                   accessibility == Accessibility.Protected ||
                   accessibility == Accessibility.ProtectedAndInternal;
        }

        public static string MemberName(this ISymbol member)
        {
            return XmlUtility.EscapeString(member.MemberNameUnescaped());
        }

        public static string MemberNameUnescaped(this ISymbol member)
        {
            var name = member.Name;
            var typeParameters = member.GetTypeParameters();
            if (!typeParameters.IsDefaultOrEmpty)
                name += "`" + typeParameters.Length;

            return name;
        }
    }
}
