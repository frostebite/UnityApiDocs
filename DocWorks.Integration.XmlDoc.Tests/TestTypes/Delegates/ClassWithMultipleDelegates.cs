using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.Integration.XmlDoc.Tests.TestTypes.Delegates
{
    public class ClassWithMultipleDelegates
    {
        public delegate float AudioCurveEvaluator(float x);
        public delegate float AudioCurveAndColorEvaluator(float x, out string col);
    }
}
