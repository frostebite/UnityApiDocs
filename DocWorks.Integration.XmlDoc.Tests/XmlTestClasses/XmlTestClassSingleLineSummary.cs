using System;

namespace DocWorks.Integration.XmlDoc.Tests.XmlTestClasses
{
    /// <summary>class summary</summary>
    public class XmlTestClassSingleLineSummary
    {     
        /// <summary>function summary</summary>
        /// <param name="randomParameter">parameter description</param>
        /// <returns>this is what the function returns</returns>
        /// <exception cref="Exception">exception description</exception>
        public string GetStructName(string randomParameter)
        {
            try
            {
                return randomParameter;
            }
            catch (Exception e)
            {
                throw new Exception("random exception1");
            }
        }
    }
}