using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    public class ClassWithMemberWithRefArgument
    {
        public void RefMember(ref Object o)
        {

        }

        public void RefMember(Object o)
        {

        }

        public static void RefMemberStatic(ref Object o)
        {

        }

        public static void RefMemberStatic(Object o)
        {

        }
    }
}
