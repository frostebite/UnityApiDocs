using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DocWorks.Integration.XmlDoc.Extensions
{
    public static class TypedConstantExtensions
    {
        public static string ToCSharpXMLEncodedString(this TypedConstant constant)
        {
            if (constant.Type.QualifiedName(true, true) == "System.String")
                return SymbolDisplay.FormatPrimitive(constant.Value, false, false).XMLEncoded();

            return constant.ToCSharpString();
        }
    }
}
