namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    public class ClassWithRefFunction
    {
        public int p = 12;
        
        /// <summary>
        /// Test summary
        /// </summary>
        public ref int GetContactInformation(string fname, string lname)
        {
            return ref p;
        }
    }
}