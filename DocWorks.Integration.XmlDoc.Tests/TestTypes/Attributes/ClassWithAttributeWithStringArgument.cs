﻿namespace DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes
{
    [TestPublic("string")]
    public class ClassWithAttributeWithStringArgument
    {
        [TestPublic("& > < \" '")]
        public void Foo() { }
    }
}
