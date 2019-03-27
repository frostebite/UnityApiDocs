using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace DocWorks.Integration.XmlDoc.Tests
{
    [TestFixture]
    class SaveXmlTests : XmlDocHandlerTestBase
    {
        public struct UpdateTestData
        {
            public string newDocXml;
            public string expectedSource;
            public string sourcePath;
            public bool compareRaw;
        }

        public static IEnumerable<TestCaseData> UpdateTestCases()
        {
            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
    <doc version=""3"">
        <member name=""ClassWithProperty"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
            <member name=""Value"" type=""Property"">
                <signature>
<accessibility>Public</accessibility>
<type typeId=""System.Int32"" typeName=""int"" />

<get><accessibility>Public</accessibility></get>
<parameters></parameters></signature>
            <xmldoc>
    <![CDATA[<summary>
    New Value Propery
    </summary>
]]>
            </xmldoc>
        </member>
</member></doc>",
                    expectedSource = @"
namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{    
    public class ClassWithProperty
    {
        /// <summary>
        ///New Value Propery
        ///</summary>
        [TestInternal][TestPublic]
        public int Value
",
                    sourcePath = "TestTypes/ClassWithProperty.cs"
                }).SetName("Update_Property");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf -8"" standalone =""yes"" ?>
    <doc version=""3"">
        <member name=""SimpleClassWithXmlDoc"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
<![CDATA[
<summary>Updated Doc</summary>
]]>
        </xmldoc></member></doc>
",
                    expectedSource = @"
    /// <summary>Updated Doc</summary>
    public class SimpleClassWithXmlDoc
    {
        /// <summary>
        /// Foo XmlDoc
        /// </summary>
        public void Foo() {}
}",
                    sourcePath = "TestTypes/SimpleClassWithXmlDoc.cs"
                }).SetName("Update_Class");
            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf -8"" standalone =""yes"" ?>
    <doc version=""3"">
        <member name=""AnEnum"" type=""Enum"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"" inherits=""Enum"">
        
        <xmldoc>
<![CDATA[
<summary>
Some Docs
</summary>
]]>
        </xmldoc></member></doc>
",
                    expectedSource = @"
    /// <summary>
    /// Some Docs
    /// </summary>
    public enum AnEnum
    {
    }",
                    sourcePath = "TestTypes/CommonTypes/AnEnum.cs"
                }).SetName("Add_To_Enum");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""INestedInterface"" type=""Interface"" containingType=""AClass"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"">
        <xmldoc>
            <![CDATA[<summary>
Updated Docs
</summary>]]>
        </xmldoc>
    </member>
</doc>",
                    expectedSource = @"    /// <summary>
    /// I have a summary
    /// </summary>
    public partial class AClass : IEnumerable, ICloneable
    {
        /// <summary>
        /// Updated Docs
        /// </summary>
        public interface INestedInterface
        {",
                    sourcePath = "TestTypes/CommonTypes/AClass.cs"
                }).SetName("Update_Nested_Interface");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""GenericClass`1"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics"" inherits=""Object"">
        <typeParameters>
            <typeParameter name=""T"" />
        </typeParameters>
        <xmldoc>
            <![CDATA[<summary>New Class docs</summary>]]>
        </xmldoc>
        <member name=""Foo"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters></parameters>
            </signature>
            <xmldoc>
                <![CDATA[<summary>New Foo docs</summary>]]>
            </xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics
{
    /// <summary>New Class docs</summary>
    public class GenericClass<T>
    {
        /// <summary>New Foo docs</summary>
        public void Foo()
        {}
    }
    /// <summary>
    /// Existing Docs for GenericClass
    /// </summary>
    public class GenericClass
    {
        /// <summary>
        /// Existing Docs for GenericClass.Foo
        /// </summary>
        public void Foo()
        {}
    }",
                    sourcePath = "TestTypes/Generics/GenericClass.cs"
                }).SetName("Update_Generic");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""INestedInterface"" type=""Interface"" containingType=""AClass"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"">
        <xmldoc>
            <![CDATA[<summary>
Updated Docs
</summary>]]>
        </xmldoc>
    </member>
</doc>",
                    expectedSource = @"    /// <summary>
    /// I have a summary
    /// </summary>
    public partial class AClass : IEnumerable, ICloneable
    {
        /// <summary>
        /// Updated Docs
        /// </summary>
        public interface INestedInterface
        {",
                    sourcePath = "TestTypes/CommonTypes/AClass.cs"
                }).SetName("Update_Nested_Interface");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
    <doc version=""3"">
        <member name=""ClassWithXmlDocsAndNormalComments"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>Only the summary</summary>]]>
        </xmldoc>
</member></doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    // Type-Prefix
    /// <summary>Only the summary</summary>
    // Type-Sufix
    public class ClassWithXmlDocsAndNormalComments
    {",
                    sourcePath = "TestTypes/ClassWithXmlDocsAndNormalComments.cs"
                }).SetName("Update_With_Non_Xml_Docs");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
    <doc version=""3"">
        <member name=""ClassWithMultipleXmlDocs"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>Only the summary</summary>]]>
        </xmldoc>
</member></doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>Only the summary</summary> 
    // after summary
    // after remarks
    public class ClassWithMultipleXmlDocs
    {",
                    sourcePath = "TestTypes/ClassWithMultipleXmlDocs.cs"
                }).SetName("Update_Multiple_Xml_Docs");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithField"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <member name=""value"" type=""Field"">
            <signature>
                <accessibility>Public</accessibility>
                <type typeId=""System.Int32"" typeName=""int"" />
            </signature>
            <xmldoc>
                <![CDATA[<summary>New Docs</summary>]]>
            </xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    public class ClassWithField
    {
        /// <summary>New Docs</summary>
        [TestInternal][TestPublic]
        public int value;
    }
}",
                    sourcePath = "TestTypes/ClassWithField.cs"
                }).SetName("Update_Field");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithField"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>New ClassWithField Docs</summary>]]>
        </xmldoc>
        <member name=""value"" type=""Field"">
            <signature>
                <accessibility>Public</accessibility>
                <type typeId=""System.Int32"" typeName=""int"" />
            </signature>
            <xmldoc>
                <![CDATA[<summary>New value Docs</summary>]]>
            </xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>New ClassWithField Docs</summary>
    public class ClassWithField
    {
        /// <summary>New value Docs</summary>
        [TestInternal][TestPublic]
        public int value;
    }
}",
                    sourcePath = "TestTypes/ClassWithField.cs"
                }).SetName("Update_Field_And_Enclosing_Class");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""AClass"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>New class docs</summary>]]>
        </xmldoc>
        
        <member name=""Foo"" type=""Method"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Int32"" typeName=""int"" />
                </return>
                <parameters>
                    <parameter name=""i"" >
                        <type typeId=""System.Int32"" typeName=""int"" />
                    </parameter>
                </parameters>
            </signature>
            <xmldoc><![CDATA[<summary>New method docs</summary>]]></xmldoc>                
        </member>
    </member>
</doc>",
                    expectedSource = @"
    /// <summary>New class docs</summary>
    public partial class AClass : IEnumerable, ICloneable
    {
        /// <summary>
        /// I am a nested interface
        /// </summary>
        public interface INestedInterface
        {
            
        }

        private class APrivateClass
        {
        }

        /// <summary>New method docs</summary>
        public int Foo(int i)
        {",
                    sourcePath = "TestTypes/CommonTypes/AClass.cs"
                }).SetName("Update_Method");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithEvent"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>New class docs</summary>]]>
        </xmldoc>
        <member name=""anEvent"" type=""Event"">
            <signature>
                <accessibility>Public</accessibility>
                <type typeId=""System.Action`1"" typeName=""System.Action&lt;bool&gt;"">
                    <typeArguments>
                        <type typeId=""System.Boolean"" typeName=""bool"" />
                    </typeArguments>
                </type>
            </signature>
            <xmldoc><![CDATA[<summary>new docs</summary>]]></xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>New class docs</summary>
    public class ClassWithEvent
    {
        /// <summary>new docs</summary>
        [TestInternal] [TestPublic]
        public event System.Action<bool> anEvent;
    }
}
",
                    sourcePath = "TestTypes/ClassWithEvent.cs"
                }).SetName("Update_Event");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithEventAddRemove"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>New class docs</summary>]]>
        </xmldoc>
        <member name=""anEvent"" type=""Event"">
            <signature>
                <accessibility>Public</accessibility>
                <type typeId=""System.Action`1"" typeName=""System.Action&lt;System.Func&lt;bool&gt;&gt;"">
                    <typeArguments>
                        <type typeId=""System.Func`1"" typeName=""System.Func&lt;bool&gt;"">
                            <typeArguments>
                                <type typeId=""System.Boolean"" typeName=""bool"" />
                            </typeArguments>
                        </type>
                    </typeArguments>
                </type>
            </signature>
            <xmldoc><![CDATA[<summary>new docs</summary>]]></xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>New class docs</summary>
    public class ClassWithEventAddRemove
    {
        /// <summary>new docs</summary>
        public event System.Action<System.Func<bool>> anEvent
        {
            add { }
            remove { }
        }
    }
}",
                    sourcePath = "TestTypes/ClassWithEventAddRemove.cs"
                }).SetName("Update_Event_With_Add_Remove");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithOperator"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>New class docs</summary>]]>
        </xmldoc>
        <member name=""op_Addition"" type=""Method"" methodKind=""UserDefinedOperator"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Int32"" typeName=""int"" />
                </return>
                <parameters>
                    <parameter name=""classWithOperator"" >
                        <type typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOperator"" typeName=""DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOperator"" />
                    </parameter>
                    <parameter name=""other"" >
                        <type typeId=""System.Int32"" typeName=""int"" />
                    </parameter>
                </parameters>
            </signature>
            <xmldoc><![CDATA[<summary>new docs</summary>]]></xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>New class docs</summary>
    public class ClassWithOperator
    {
        /// <summary>new docs</summary>
        public static int operator +(ClassWithOperator classWithOperator, int other)
        {
            return 1;
        }
    }
}",
                    sourcePath = "TestTypes/ClassWithOperator.cs"
                }).SetName("Update_Operator");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithConstructor"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>New class docs</summary>]]>
        </xmldoc>
        <member name="".ctor"" type=""Method"" methodKind=""Constructor"">
            <signature>
                <accessibility>Public</accessibility>
            </signature>
            <xmldoc><![CDATA[<summary>new docs</summary>]]></xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>New class docs</summary>
    public class ClassWithConstructor
    {
        /// <summary>new docs</summary>
        public ClassWithConstructor() { }
    }
}",
                    sourcePath = "TestTypes/ClassWithConstructor.cs"
                }).SetName("Update_Constructor");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithExtensionMethods"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[<summary>New class docs</summary>]]>
        </xmldoc>
        <member name=""ExtensionMethod"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Int32"" typeName=""int"" />
                </return>
                <parameters>
                    <parameter name=""s"" isThis=""true"">
                        <type typeId=""System.String"" typeName=""string"" />
                    </parameter>
                </parameters>
            </signature>
            <xmldoc><![CDATA[<summary>new docs</summary>]]></xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"
    /// <summary>New class docs</summary>
    public static class ClassWithExtensionMethods
    {
        /// <summary>new docs</summary>
        [TestInternal][TestPublic]
        public static int ExtensionMethod(this string s)
        {",
                    sourcePath = "TestTypes/ClassWithExtensionMethods.cs"
                }).SetName("Update_Extension_Method");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ADelegate"" type=""Delegate"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"" inherits=""System.MulticastDelegate"" isSealed=""true"">
        <signature>
            <accessibility>Public
            </accessibility>
            <return>
                <type typeId=""System.Int32"" typeName=""int"" />
            </return>
            <parameters>
                <parameter name=""o"" >
                    <type typeId=""System.Object"" typeName=""object"" />
                </parameter>
            </parameters>
        </signature>
        <xmldoc>
            <![CDATA[New docs]]>
        </xmldoc>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes
{
    /// New docs
    public delegate int ADelegate(System.Object o);",
                    sourcePath = "TestTypes/CommonTypes/GlobalDelegate.cs"
                }).SetName("Update_Global_Delegate");


            yield return new TestCaseData(
                    new UpdateTestData
                    {
                        newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""Delegate"" type=""Delegate"" containingType=""AClass"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"" inherits=""System.MulticastDelegate"" isSealed=""true"">
        <signature>
            <accessibility>Public
            </accessibility>
            <return>
                <type typeId=""System.Void"" typeName=""void"" />
            </return>
            <parameters>
            </parameters>
        </signature>
        <xmldoc>
            <![CDATA[New docs]]>
        </xmldoc>
    </member>
</doc>",
                        expectedSource = @"

        /// New docs
        public delegate void Delegate();",
                        sourcePath = "TestTypes/CommonTypes/AClass.cs"
                    }).SetName("Update_Inner_Delegate");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""GenericStructWithConstraints`1"" type=""Struct"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics"">
        
        <xmldoc>
            <![CDATA[New class docs]]>
        </xmldoc>

        <member name=""GenericMethodWithGenericConstraint`1"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public
                </accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                    <parameter name=""t2"">
                        <typeParameter declaringTypeId="""" name=""T2"">
                            <attributes>
                                <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
                            </attributes>
                        </typeParameter>
                    </parameter>
                </parameters>
                <typeParameters>
                    <typeParameter name=""T2"">
                        <typeParameter declaringTypeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericStructWithConstraints`1"" name=""T"" />
                        <attributes>
                            <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
                        </attributes>
                    </typeParameter>
                </typeParameters>
            </signature>
            <xmldoc>
                <![CDATA[new GenericMethodWithGenericConstraint`1 docs]]>
            </xmldoc>
        </member>     
        <member name=""GenericMethodWithGenericConstraint"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public
                </accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                </parameters>
            </signature>
            <xmldoc>
                <![CDATA[new GenericMethodWithGenericConstraint docs]]>
            </xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"
    /// New class docs
    public struct GenericStructWithConstraints<T> where T : class, IList<DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass>, new()
    {
        /// new GenericMethodWithGenericConstraint`1 docs
        public void GenericMethodWithGenericConstraint<[TestInternal][TestPublic]T2>(T2 t2) where T2 : T
        { }
        /// new GenericMethodWithGenericConstraint docs
        public void GenericMethodWithGenericConstraint()
        { }",
                    sourcePath = "TestTypes/Generics/GenericStructWithConstraints.cs"
                }).SetName("Update_Non_Generic_Overload");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithOverloads"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" id="""">
        <xmldoc>
            <![CDATA[New class docs]]>
        </xmldoc>
        <member name=""Method"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                </parameters>
            </signature>
            <xmldoc>
                <![CDATA[new docs for Method()]]>
            </xmldoc>
        </member>
        <member name=""Method"" type=""Method"" methodKind=""Ordinary"" id="""">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                    <parameter name=""i"">
                        <type typeId=""System.Int32"" typeName=""int"" />
                    </parameter>
                </parameters>
            </signature>
            <xmldoc>
                <![CDATA[new docs for Method(int)]]>
            </xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// New class docs
    public class ClassWithOverloads
    {
        /// new docs for Method()
        public void Method() { }
        /// new docs for Method(int)
        public void Method(int i) { }
    }",
                    sourcePath = "TestTypes/ClassWithOverloads.cs"
                }).SetName("Update_Overload");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassInGlobalNamespace"" type=""Class"" namespace="""">
        <xmldoc><![CDATA[New Docs]]></xmldoc>
    </member>
</doc>",
                    expectedSource = @"/// New Docs
public class ClassInGlobalNamespace
{}
",
                    sourcePath = "TestTypes/ClassInGlobalNamespace.cs"
                }).SetName("Update_Class_In_Global_Namespace");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithProtectedMethod"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        <xmldoc><![CDATA[<summary>
New class docs
</summary>]]></xmldoc>
        <member name=""ProtectedMethod"" type=""Method"">
            <signature>
                <accessibility>Protected</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                </parameters>
            </signature>
            <xmldoc><![CDATA[<summary>
New method docs
</summary>]]></xmldoc>
        </member>
    </member>
</doc>",
                    expectedSource = @"    /// <summary>
    /// New class docs
    /// </summary>
    public class ClassWithProtectedMethod
    {
        

        
        /// <summary>
        /// New method docs
        /// </summary>
        protected void ProtectedMethod()
        {
        }
    }",
                    sourcePath = "TestTypes/ClassWithProtectedMethod.cs",
                    compareRaw = true
                }).SetName("Update_Protected_Method");

            yield return new TestCaseData(
                new UpdateTestData
                {
                    newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""SingleLineFeed"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        <member name=""Method"" type=""Method"" methodKind=""Ordinary"">
        <signature>
            <accessibility>Public</accessibility>
            <return>
                <type typeId=""System.Int32"" typeName=""int"" />
            </return>
            <parameters>
                <parameter name=""i"">
                    <type typeId = ""System.Int32"" typeName = ""i"" />
                </parameter>
            </parameters>
        </signature>
        " + "<xmldoc><![CDATA[<summary>\nMethod\n</summary>\n<param name=\"i\">\nvalue\n</param>\n<returns>\nThe same value it receives\n</returns>\n<description>\nMethod Description\n</description>]]>\n" +
    @"</xmldoc>
   </member>
   </member>
</doc>",
                    expectedSource = @"
namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    public class SingleLineFeed
    {
        /// <summary>
        /// Method
        /// </summary>
        /// <param name=""i"">
        /// value
        /// </param>
        /// <returns>
        /// The same value it receives
        /// </returns>
        /// <description>
        /// Method Description
        /// </description>
        public int Method(int i)
        {
            return i;
        }
    }
}",

                    sourcePath = "TestTypes/SingleLineFeed.cs",
                    compareRaw = true
                }).SetName("SingleLineFeeds");
        }

        //TODO: Add tests for: Formating
        [Test]
        [TestCaseSource(nameof(UpdateTestCases))]
        public void Update(UpdateTestData data)
        {
            var testFilePath = Path.GetTempFileName();

            File.Copy(data.sourcePath, testFilePath, true);
            try
            {
                var handler = new XMLDocHandler(MakeCompilationParameters(Path.GetDirectoryName(data.sourcePath)));

                handler.SetType(data.newDocXml, Path.GetFileName(data.sourcePath));

                var actualSource = File.ReadAllText(data.sourcePath);
                AssertSourceContains(data.expectedSource, actualSource, !data.compareRaw);
            }
            finally
            {
                File.Copy(testFilePath, data.sourcePath, true);
            }
        }


        [Test]
        public void Test_PrivateMember_Documentation_whenPropertyNameIsSame_InAnotherPublicMember()
        {
            UpdateTestData data = new UpdateTestData
            {
                newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
	<member name=""Settings"" type=""Class"" containingType=""CameraEditor"" namespace=""UnityEditor"" isSealed=""true"">
		<xmldoc>
			<![CDATA[]]>
		</xmldoc>
		<member name=""_clearFlags"" type=""Property"">
			<signature>
				<accessibility>Public</accessibility>
				<type typeId=""System.String"" typeName=""string"" />
				<get>
					<accessibility>Public</accessibility>
				</get>
				<parameters></parameters>
			</signature>
			<xmldoc>
				<![CDATA[<summary>Exposed [[SerializedProperty]] for the inspected [[Camera]].</summary><description>See also: [[Camera.clearFlags]]</description>]]>
			</xmldoc>
		</member>
	</member>
</doc>",
                expectedSource = @"public class CameraEditor 
    {
        private static class Styles
        {
            public static string _clearFlags;
        }
        
        public sealed class Settings
        {
            /// <summary>Exposed [[SerializedProperty]] for the inspected [[Camera]].</summary><description>See also: [[Camera.clearFlags]]</description>
            public string _clearFlags { get; private set; }",
                sourcePath = "TestTypes/CameraEditor.cs"
            };
            var testFilePath = Path.GetTempFileName();

            File.Copy(data.sourcePath, testFilePath, true);
            try
            {
                var handler = new XMLDocHandler(MakeCompilationParameters(Path.GetDirectoryName(data.sourcePath)));

                handler.SetType(data.newDocXml, Path.GetFileName(data.sourcePath));

                var actualSource = File.ReadAllText(data.sourcePath);
                AssertSourceContains(data.expectedSource, actualSource, !data.compareRaw);
            }
            catch (Exception e)
            {

            }
        }

        [Test]
        public void Test_DuplicateMemberDocumentation_InDifferentClass_whenPropertyNameIsSame()
        {
            UpdateTestData data = new UpdateTestData
            {
                newDocXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
	<member name=""TakeInfo"" type=""Struct"" namespace=""UnityEditor"">
		<xmldoc>
			<![CDATA[]]>
		</xmldoc>
		<member name=""name"" type=""Field"">
			<signature>
				<accessibility>Public</accessibility>
				<type typeId=""System.String"" typeName=""string"" />
			</signature>
			<xmldoc>
				<![CDATA[<summary>Take name as define from imported file.</summary>]]>
			</xmldoc>
		</member>
	</member>
</doc>",
                expectedSource = @"namespace UnityEditor
{
    public struct ClipAnimationInfoCurve
    {
        public string name;
    }
    
    public struct TakeInfo
    {
        /// <summary>Take name as define from imported file.</summary>
        public string name;
    }
}",
                sourcePath = "TestTypes/ModelImporterBindingFake.cs"
            };
            var testFilePath = Path.GetTempFileName();

            File.Copy(data.sourcePath, testFilePath, true);
            try
            {
                var handler = new XMLDocHandler(MakeCompilationParameters(Path.GetDirectoryName(data.sourcePath)));

                handler.SetType(data.newDocXml, Path.GetFileName(data.sourcePath));

                var actualSource = File.ReadAllText(data.sourcePath);
                AssertSourceContains(data.expectedSource, actualSource, !data.compareRaw);
            }
            catch (Exception e)
            {

            }
        }

        [Test]
        public void Update_Duplicate_Member_Throws()
        {
            var updateTestData = new UpdateTestData
            {
                newDocXml = @"<?xml version=""1.0"" encoding=""utf -8"" standalone =""yes"" ?>
<doc version=""3"">
    <member name=""SimpleClassWithXmlDoc"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[Doc 1]]>
        </xmldoc>
    </member>
    <member name=""SimpleClassWithXmlDoc"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""Object"">
        <xmldoc>
            <![CDATA[Doc 2]]>
        </xmldoc>
    </member>
</doc>
",
                expectedSource = @"
/// <summary>Updated Doc</summary>
public class SimpleClassWithXmlDoc
{
    /// <summary>
    /// Foo XmlDoc
    /// </summary>
    public void Foo() {}
}",
                sourcePath = "TestTypes/SimpleClassWithXmlDoc.cs"
            };
            Assert.Throws(typeof(DuplicateMemberException), () => Update(updateTestData));
        }

        // Partials: 
        //          1) Udating comment on single partial, 
        //          2) Updating comments in multiple partials (we should remove the comment in all but one partial)
        //          3) Adding comments in multiple partials (ensure that comment end up in only one of the partials)

        public struct UpdatePartialsTestData
        {
            public string filename1, filename2;
            public string newContent;
            public string expectedFile1Source;
            public string expectedFile2Source;
        }

        public static IEnumerable<TestCaseData> UpdatePartialsTestCases()
        {
            yield return new TestCaseData(
                new UpdatePartialsTestData
                {
                    filename1 = "TestTypes/CommonTypes/AClass.cs",
                    filename2 = "TestTypes/CommonTypes/AFolder/AClass.part2.cs",
                    newContent = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""AClass"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"" inherits=""Object"">
        <xmldoc><![CDATA[<summary>new doc</summary>]]></xmldoc>
    </member>
</doc>
",
                    expectedFile1Source = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes
{
    public partial class AClass { }

    /// <summary>new doc</summary>
    public partial class AClass : IEnumerable, ICloneable
    {
",
                    expectedFile2Source = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes
{
    //Here is a partial for implementation details...
    public partial class AClass
    {
"
                }).SetName("Update_Partial_Class_With_Existing_Doc");
            yield return new TestCaseData(
                new UpdatePartialsTestData
                {
                    filename1 = "TestTypes/PartialInterfaceNoDocs.cs",
                    filename2 = "TestTypes/PartialInterfaceNoDocs.part2.cs",
                    newContent = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""PartialInterfaceNoDocs"" type=""Interface"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        <xmldoc><![CDATA[<summary>new doc</summary>]]></xmldoc>
    </member>
</doc>
",
                    expectedFile1Source = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    /// <summary>new doc</summary>
    partial interface PartialInterfaceNoDocs
    {
    }
}
",
                    expectedFile2Source = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    partial interface PartialInterfaceNoDocs
    {
    }
}
"
                }).SetName("Update_Partial_Interface_With_No_Existing_Doc");
            yield return new TestCaseData(
                new UpdatePartialsTestData
                {
                    filename1 = "TestTypes/PartialStructWithDocs.cs",
                    filename2 = "TestTypes/PartialStructWithDocs.part2.cs",
                    newContent = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""PartialStructWithDocs"" type=""Struct"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        <xmldoc><![CDATA[<summary>new doc</summary>]]></xmldoc>
    </member>
</doc>
",
                    expectedFile1Source = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    partial struct PartialStructWithDocs
    {
    }
    /// <summary>new doc</summary>
    partial struct PartialStructWithDocs
    {
    }
}
",
                    expectedFile2Source = @"namespace DocWorks.Integration.XmlDoc.Tests.TestTypes
{
    partial struct PartialStructWithDocs
    {
    }
}
"
                }).SetName("Update_Partial_Struct_With_Multiple_Existing_Doc");
        }

        [Test]
        [TestCaseSource(nameof(UpdatePartialsTestCases))]
        public void Test_Update_Partials(UpdatePartialsTestData testData)
        {
            var testFilePath1 = Path.GetTempFileName();
            var testFilePath2 = Path.GetTempFileName();

            File.Copy(testData.filename1, testFilePath1, true);
            File.Copy(testData.filename2, testFilePath2, true);
            try
            {

                var handler = new XMLDocHandler(MakeCompilationParameters("."));

                handler.SetType(testData.newContent, testData.filename1, testData.filename2);

                var actualSource1 = File.ReadAllText(testData.filename1);
                AssertSourceContains(testData.expectedFile1Source, actualSource1);

                var actualSource2 = File.ReadAllText(testData.filename2);
                AssertSourceContains(testData.expectedFile2Source, actualSource2);
            }
            finally
            {
                File.Copy(testFilePath1, testData.filename1, true);
                File.Copy(testFilePath2, testData.filename2, true);
            }
        }

        [Test]
        public void Throws_When_Given_Cs_Outside_Root()
        {
            var handler = new XMLDocHandler(MakeCompilationParameters("."));
            var tempPath = Path.GetTempFileName();
            var tempScriptPath = tempPath + ".cs";
            File.Move(tempPath, tempScriptPath);
            Assert.Throws(typeof(ArgumentException), () => handler.SetType(@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?><doc />", tempScriptPath));
        }

        [Test]
        public void Throws_When_Given_Non_Existant_Script()
        {
            var handler = new XMLDocHandler(MakeCompilationParameters("."));
            Assert.Throws(typeof(FileNotFoundException), () => handler.SetType(@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?><doc />", "asdf.cs"));
        }

        [Test]
        public void Throws_When_Given_Non_Cs_file()
        {
            var handler = new XMLDocHandler(MakeCompilationParameters(Path.GetTempPath()));
            Assert.Throws(typeof(ArgumentException), () => handler.SetType("", Path.GetTempFileName()));
        }

        private void AssertSourceContains(string expectedSource, string actualSource, bool normalize = true)
        {
            if (normalize)
            {
                actualSource = Normalize(actualSource);
                expectedSource = Normalize(expectedSource);
            }

            Assert.IsTrue(actualSource.Contains(expectedSource), $"Expected\n --\n{expectedSource}\n--\nbut got\n --\n{actualSource}\n--\n");
        }

        [Test]
        public void ImportDocumentation_ShouldUpdateSuccessfully()
        {
            string sourcePath = "TestTypes/UpdateEnums.cs";
            var testFilePath = Path.GetTempFileName();
            File.Copy(sourcePath, testFilePath, true);
            try
            {
                var handler = new XMLDocHandler(new CompilationParameters(Path.GetDirectoryName(sourcePath), new string[0], new string[] { "Win64", "MacOs" }, new string[0]));
                string newDocXml = "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><doc version=\"3\"><member name=\"TextureImporterFormat\" type=\"Enum\" namespace=\"UnityEditor\" inherits=\"System.Enum\"><xmldoc><![CDATA[<summary>\r\nImported texture format for TextureImporter.\r\n</summary>\r\n<description>\r\nMost of the values match the ones in TextureFormat, with addition of the \"Automatic\" ones that pick the best format based\non platform and texture type or usage.\n\nSA: TextureImporter.textureFormat.\r\n</description>\r\n]]></xmldoc><member name=\"Automatic\" type=\"Field\"><signature><accessibility>Public</accessibility><type typeId=\"UnityEditor.TextureImporterFormat\" typeName=\"UnityEditor.TextureImporterFormat\" /></signature><xmldoc><![CDATA[<summary>\r\nChoose texture format automatically based on the texture parameters.\r\n</summary>\r\n]]></xmldoc></member><member name=\"AutomaticCompressed\" type=\"Field\"><signature><accessibility>Public</accessibility><type typeId=\"UnityEditor.TextureImporterFormat\" typeName=\"UnityEditor.TextureImporterFormat\" /></signature><attributes><attribute typeId=\"System.ObsoleteAttribute\"><constructorArguments><argument value=\"Use textureCompression property instead\"><type typeId=\"System.String\" typeName=\"string\" /></argument></constructorArguments></attribute></attributes><xmldoc><![CDATA[<summary>\r\nChoose a compressed format automatically.\r\n</summary>\r\n<description>\r\nSA: TextureImporter.textureFormat.\r\n</description>\r\n]]></xmldoc></member><member name=\"Automatic16bit\" type=\"Field\"><signature><accessibility>Public</accessibility><type typeId=\"UnityEditor.TextureImporterFormat\" typeName=\"UnityEditor.TextureImporterFormat\" /></signature><attributes><attribute typeId=\"System.ObsoleteAttribute\"><constructorArguments><argument value=\"Use textureCompression property instead\"><type typeId=\"System.String\" typeName=\"string\" /></argument></constructorArguments></attribute></attributes><xmldoc><![CDATA[<summary>\r\nChoose a 16 bit format automatically.\r\n</summary>\r\n<description>\r\nSA: TextureImporter.textureFormat.\r\n</description>\r\n]]></xmldoc></member></member></doc>";
                string expectedSource = @"using System;

namespace UnityEditor
{
    // Imported texture format for [[TextureImporter]].
    /// <summary>
    /// Imported texture format for TextureImporter.
    /// </summary>
    /// <description>
    /// Most of the values match the ones in TextureFormat, with addition of the ""Automatic"" ones that pick the best format based
    /// on platform and texture type or usage.
    /// SA: TextureImporter.textureFormat.
    /// </description>
    public enum TextureImporterFormat
    {
        /// <summary>
        /// Choose texture format automatically based on the texture parameters.
        /// </summary>
        Automatic = -1,
#if Win64
        // Choose a compressed format automatically.
        /// <summary>
        /// Choose a compressed format automatically.
        /// </summary>
        /// <description>
        /// SA: TextureImporter.textureFormat.
        /// </description>
        [System.Obsolete(""Use textureCompression property instead"")]
        AutomaticCompressed = -1,
#endif
#if MacOs
        // Choose a 16 bit format automatically.
        /// <summary>
        /// Choose a 16 bit format automatically.
        /// </summary>
        /// <description>
        /// SA: TextureImporter.textureFormat.
        /// </description>
        [System.Obsolete(""Use textureCompression property instead"")]
        Automatic16bit = -2,
#endif
    }
}";
                handler.SetType(newDocXml, Path.GetFileName(sourcePath));

                var actualSource = File.ReadAllText(sourcePath);
                AssertSourceContains(expectedSource, actualSource, false);
            }
            finally
            {
                File.Copy(testFilePath, sourcePath, true);
            }
        }
    }
}
