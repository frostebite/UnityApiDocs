namespace DocWorks.Integration.XmlDoc.Extensions
{
    public static class StringExtensions
    {
        public static string XMLEncoded(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            return System.Security.SecurityElement.Escape(s);
        }
    }
}
