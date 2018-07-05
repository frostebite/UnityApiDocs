namespace DocWorks.Integration.XmlDoc
{
    static class XmlUtility
    {
        public static string EscapeString(string xmlString)
        {
            return System.Security.SecurityElement.Escape(xmlString);
        }
    }
}
