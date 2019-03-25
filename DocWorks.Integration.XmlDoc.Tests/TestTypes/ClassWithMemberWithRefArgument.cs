using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>
    /// class 1
    /// </summary>
    public class ClassWithMemberWithRefArgument
    {
        /// <summary>
        /// ref member with ref param
        /// </summary>
        /// <param name="o"></param>
        public void RefMember(ref Object o)
        {

        }

        /// <summary>
        /// ref member with no ref param
        /// </summary>
        /// <param name="o"></param>
        public void RefMember(Object o)
        {

        }

        /// <summary>
        /// static ref member with ref param
        /// </summary>
        /// <param name="o"></param>
        public static void RefMemberStatic(ref Object o)
        {

        }

        /// <summary>
        /// static ref member with no ref param
        /// </summary>
        /// <param name="o"></param>
        public static void RefMemberStatic(Object o)
        {

        }
    }
}
