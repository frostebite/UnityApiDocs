﻿namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    public class ClassWithIndexer
    {
        /// <summary>
        /// Indexer property
        /// </summary>
        public int this[int a]
        {
            get
            {
                return 1;
            }
            protected set { }
        }
    }
}
