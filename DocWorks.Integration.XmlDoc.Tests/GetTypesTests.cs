using DocWorks.Integration.XmlDoc.TestUtilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DocWorks.Integration.XmlDoc.Tests
{
    [TestFixture]
    public class XmlDocHandlerTest : XmlDocHandlerTestBase
    {
        [Test]
        public void GetTypes_Returns_Relative_Path_When_Given_Path_Without_Trailing_Slash()
        {
            var testFileDirectory = "TestTypes/CommonTypes";
            var handler = new XMLDocHandler(MakeCompilationParameters(testFileDirectory));
            string xmlActual = handler.GetTypesXml();

            Assert.That(xmlActual, Contains.Substring("<path>AClass.cs</path>"));
            Assert.That(xmlActual, Contains.Substring(@"<path>AFolder\AClass.part2.cs</path>"));
        }

        [Test]
        public void GetTypes_Full_ReturnsCorrectXml()
        {
            var testFileDirectory = "TestTypes/CommonTypes/";
            var handler = new XMLDocHandler(MakeCompilationParameters(testFileDirectory));
            string xmlActual = handler.GetTypesXml();

            var expected = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""1"">
    <types>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass</id>
            <parentId></parentId>
            <name>AClass</name>
            <kind>Class</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes</namespace>
            <relativeFilePaths>
                <path>AClass.cs</path>
<path>AFolder\AClass.part2.cs</path>

            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.INestedInterface</id>
            <parentId>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass</parentId>
            <name>INestedInterface</name>
            <kind>Interface</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes</namespace>
            <relativeFilePaths>
                <path>AClass.cs</path>

            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.Delegate</id>
            <parentId>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass</parentId>
            <name>Delegate</name>
            <kind>Delegate</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes</namespace>
            <relativeFilePaths>
                <path>AClass.cs</path>
            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AnEnum</id>
            <parentId></parentId>
            <name>AnEnum</name>
            <kind>Enum</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes</namespace>
            <relativeFilePaths>
                <path>AnEnum.cs</path>

            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.ADelegate</id>
            <parentId></parentId>
            <name>ADelegate</name>
            <kind>Delegate</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes</namespace>
            <relativeFilePaths>
                <path>GlobalDelegate.cs</path>
            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.AnEnum</id>
            <parentId>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass</parentId>
            <name>AnEnum</name>
            <kind>Enum</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes</namespace>
            <relativeFilePaths>
                <path>AFolder\AClass.part2.cs</path>

            </relativeFilePaths>
        </type></types></doc>";

            AssertXml(expected, xmlActual);

        }

        [Test]
        public void GetException_For_SpecificInputType_InGetTypeDocumentation()
        {
            var testFileDirectory = "TestTypes";
            var handler = new XMLDocHandler(MakeCompilationParameters(testFileDirectory));
            Assert.DoesNotThrow(() => handler.GetTypeDocumentation("UnityEditor.Experimental.Animations.GameObjectRecorder", new string[] { "SpecificTypeWithSymbolException.cs" }));
        }

        [Test]
        public void GetTypes_Generics_ReturnsCorrectXml()
        {
            var testFileDirectory = "TestTypes/Generics/";
            var handler = new XMLDocHandler(MakeCompilationParameters(testFileDirectory));
            string xmlActual = handler.GetTypesXml();

            var expected = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""1"">
    <types>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.ExtendsGenericInterface</id>
            <parentId></parentId>
            <name>ExtendsGenericInterface</name>
            <kind>Class</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
            <relativeFilePaths>
                <path>ExtendsGenericInterface.cs</path>
            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericArrayField</id>
            <parentId></parentId>
            <name>GenericArrayField</name>
            <kind>Class</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
            <relativeFilePaths>
                <path>GenericArrayField.cs</path>
            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericArrayMethodParameter</id>
            <parentId></parentId>
            <name>GenericArrayMethodParameter</name>
            <kind>Class</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
            <relativeFilePaths>
                <path>GenericArrayMethodParameter.cs</path>
            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericClass`1</id>
            <parentId></parentId>
            <name>GenericClass</name>
            <kind>Class</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
            <relativeFilePaths>
                <path>GenericClass.cs</path>
            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericClass</id>
            <parentId></parentId>
            <name>GenericClass</name>
            <kind>Class</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
            <relativeFilePaths>
                <path>GenericClass.cs</path>
            </relativeFilePaths>
        </type>
        <type>
          <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericClassWithField`1</id>
          <parentId></parentId>
          <name>GenericClassWithField</name>
          <kind>Class</kind>
          <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
          <relativeFilePaths>
            <path>GenericClassWithField.cs</path>
          </relativeFilePaths>
        </type>
        <type>
          <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericDelegate`1</id>
          <parentId></parentId>
          <name>GenericDelegate</name>
          <kind>Delegate</kind>
          <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
          <relativeFilePaths>
            <path>GenericDelegate.cs</path>
          </relativeFilePaths>
        </type>
        <type>
          <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericMethod</id>
          <parentId></parentId>
          <name>GenericMethod</name>
          <kind>Class</kind>
          <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
          <relativeFilePaths>
            <path>GenericMethod.cs</path>
          </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericMethodParameter</id>
            <parentId></parentId>
            <name>GenericMethodParameter</name>
            <kind>Class</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
            <relativeFilePaths>
                <path>GenericMethodParameter.cs</path>
            </relativeFilePaths>
        </type>
        <type>
            <id>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericStructWithConstraints`1</id>
            <parentId></parentId>
            <name>GenericStructWithConstraints</name>
            <kind>Struct</kind>
            <namespace>DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics</namespace>
            <relativeFilePaths>
                <path>GenericStructWithConstraints.cs</path>
            </relativeFilePaths>
        </type></types></doc>";

            AssertXml(expected, xmlActual);
        }

        [Test]
        public void GetType_Documentation_ReturnsCorrectXml()
        {
            var handler = new XMLDocHandler(MakeCompilationParameters("TestTypes/CommonTypes/"));
            string actualXml = handler.GetTypeDocumentation("DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass", "AClass.cs", "AFolder/AClass.part2.cs");
            Console.WriteLine(actualXml);

            var expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""AClass"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"">
        <interfaces>
            <type typeId=""System.Collections.IEnumerable"" typeName=""System.Collections.IEnumerable"" />
            <type typeId=""System.ICloneable"" typeName=""System.ICloneable"" />
        </interfaces>
        <xmldoc><![CDATA[
            <summary>I have a summary</summary>
            <example>In a partial type...</example>
            Here is some more docs
        ]]></xmldoc>

        <member name=""Foo"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Int32"" typeName=""int"" />
                </return>
                <parameters>
                    <parameter name=""i"">
                        <type typeId=""System.Int32"" typeName=""int"" />
                    </parameter>
                </parameters>
            </signature>
            <xmldoc><![CDATA[
                <summary>So do I</summary>
                <returns>whatever you want.</returns>
            ]]></xmldoc>                
        </member>

        <member name=""VoidProtectedMethod"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Protected</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                </parameters>
            </signature>
            <xmldoc><![CDATA[
                some docs
            ]]></xmldoc>                
        </member>
        <member name=""System.Collections.IEnumerable.GetEnumerator"" type=""Method"" methodKind=""ExplicitInterfaceImplementation"">
            <signature>
                <accessibility>Private</accessibility>
                <return>
                    <type typeId=""System.Collections.IEnumerator"" typeName=""System.Collections.IEnumerator"" />
                </return>
                <parameters></parameters>
            </signature>
            <xmldoc><![CDATA[
                <summary>
                Explicit Implementation
                </summary>
                <returns></returns>
            ]]></xmldoc>
        </member>
        <member name=""Clone"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Object"" typeName=""object"" />
                </return>
                <parameters></parameters>
            </signature>
            <xmldoc><![CDATA[    
            ]]></xmldoc>
        </member>
    </member>
</doc>";
            AssertXml(expectedXml, actualXml);
        }

        [Test]
        public void GetTypes_ExcludeDirectories()
        {
            var handler = new XMLDocHandler(new CompilationParameters("TestTypes/", new[] { "TestTypes/CommonTypes" }, new string[0], new[] { typeof(object).Assembly.Location }, new string[0]));
            string actualXml = handler.GetTypesXml();
            Assert.That(actualXml, !Contains.Substring("AClass"));
        }

        [Test]
        public void Test_Documentation_Under_Conditional_Compilation_Symbols_Works()
        {
            Assert.Inconclusive("Not implementated yet");
        }

        public struct TestIsReportedData
        {
            public string typeId;
            public string sourceFile;
            public string expectedXml;
            public bool exact;
            public string[] referencedAssemblyPaths;
        }

        static IEnumerable<TestCaseData> TestIsReportedTestCases()
        {
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithField.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithField",
                    expectedXml = @"<member name=""value"" type=""Field"">
<signature>
    <accessibility>Public</accessibility>
    <type typeId=""System.Int32"" typeName=""int"" />
</signature>
<attributes>
    <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
</attributes>
<xmldoc><![CDATA[
    <summary>
    Value field
    </summary>
]]></xmldoc>
</member>"
                }).SetName("Field_Is_Reported");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassImplementingGenericInterface.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassImplementingGenericInterface",
                    expectedXml = @"
<member name=""ClassImplementingGenericInterface"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
    <interfaces>
        <type typeId=""System.IEquatable`1"" typeName=""System.IEquatable&lt;bool&gt;"">
            <typeArguments>
                <type typeId=""System.Boolean"" typeName=""bool"" />
            </typeArguments>
        </type>
    </interfaces>"
                }).SetName("Generic_Interface");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Generics/GenericClass.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericClass`1",
                    expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""GenericClass`1"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics"">
        <typeParameters>
            <typeParameter name=""T"" />
        </typeParameters>
        <xmldoc>
            <![CDATA[
    <summary>
    Existing Docs for GenericClass-T
    </summary>]]>
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
                <![CDATA[
    <summary>
    Existing GenericClass-T.Foo
    </summary>

]]>
            </xmldoc>
        </member>
    </member>
</doc>",
                    exact = true
                }).SetName("Generic_Types");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithProperty.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithProperty",
                    expectedXml = @"<member name=""Value"" type=""Property"">
<signature>
    <accessibility>Public</accessibility>
    <type typeId=""System.Int32"" typeName=""int"" />
    <get><accessibility>Public</accessibility></get>
    <parameters></parameters>
</signature>
<attributes>
    <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
</attributes>
<xmldoc><![CDATA[
                
    <summary>
    Value property
    </summary>


]]></xmldoc>
</member>"
                }).SetName("Property");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithIndexer.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithIndexer",
                    expectedXml = @"  <?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
    <doc version=""3"">
        <member name=""ClassWithIndexer"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        
        <xmldoc><![CDATA[
        
        ]]></xmldoc><member name=""this[]"" type=""Property"">
            <signature>
                <accessibility>Public</accessibility>
                <type typeId=""System.Int32"" typeName=""int"" />

                <get><accessibility>Public</accessibility></get>
                <set><accessibility>Protected</accessibility></set>
                <parameters>
                    <parameter name=""a"">
                        <type typeId=""System.Int32"" typeName=""int"" />
                    </parameter>
                </parameters>
            </signature>
            <xmldoc><![CDATA[
                
    <summary>
    Indexer property
    </summary>


            ]]></xmldoc>
        </member>
</member></doc>",
                    exact = true
                }).SetName("Property_With_Indexer");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/CommonTypes/AClass.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.INestedInterface",
                    expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""INestedInterface"" type=""Interface"" containingType=""AClass"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"">
        <xmldoc><![CDATA[
            <summary>
            I am a nested interface
            </summary>
        ]]></xmldoc>
    </member>
</doc>",
                    exact = true
                }).SetName("Inner_Types");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassInGlobalNamespace.cs",
                    typeId = "ClassInGlobalNamespace",
                    expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassInGlobalNamespace"" type=""Class"" namespace="""">
        <xmldoc><![CDATA[
            <summary>
            ClassInGlobalNamespace
            </summary>
        ]]></xmldoc>
    </member>
</doc>",
                    exact = true
                }).SetName("Global_Namespace");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithEvent.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithEvent",
                    expectedXml =
@"<member name=""anEvent"" type=""Event"">
    <signature>
        <accessibility>Public</accessibility>
        <type typeId=""System.Action`1"" typeName=""System.Action&lt;bool&gt;"">
            <typeArguments>
                <type typeId=""System.Boolean"" typeName=""bool"" />
            </typeArguments>
        </type>
    </signature>
    <attributes>
        <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
    </attributes>
    <xmldoc><![CDATA[
    <summary>
    anEvent
    </summary>
    ]]></xmldoc>
</member>"
                }).SetName("Event_Is_Reported");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithEventAddRemove.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithEventAddRemove",
                    expectedXml =
                        @"<member name=""anEvent"" type=""Event"">
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
    <xmldoc><![CDATA[
    <summary>
    anEvent
    </summary>
    ]]></xmldoc>
</member>"
                }).SetName("Event_With_Add_Remove_Is_Reported");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithOperator.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOperator",
                    expectedXml =
                        @"<member name=""op_Addition"" type=""Method"" methodKind=""UserDefinedOperator"" isStatic=""true"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
    <type typeId=""System.Int32"" typeName=""int"" />
</return>
        <parameters>
            <parameter name=""classWithOperator"">
                <type typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOperator"" typeName=""DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOperator"" />
            </parameter>
            <parameter name=""other"">
                <type typeId=""System.Int32"" typeName=""int"" />
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[
        <summary>
        Plus Operator
        </summary>]]></xmldoc>
</member>"
                }).SetName("Operator_Is_Reported");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithConstructor.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithConstructor",
                    expectedXml =
                        @"<member name="".ctor"" type=""Method"" methodKind=""Constructor"">
    <signature>
        <accessibility>Public</accessibility>
        <parameters></parameters>
    </signature>
    <xmldoc><![CDATA[<summary>A Constructor</summary>]]></xmldoc>
</member>"
                }).SetName("Constructor_Is_Reported");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Generics/GenericStructWithConstraints.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericStructWithConstraints`1",
                    expectedXml =
                        @"<member name=""GenericStructWithConstraints`1"" type=""Struct"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics"">
        <typeParameters>
            <typeParameter name=""T"" hasConstructorConstraint=""true"" hasReferenceTypeConstraint=""true"">
                <type typeId=""System.Collections.Generic.IList`1"" typeName=""System.Collections.Generic.IList&lt;DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass&gt;"">
                    <typeArguments>
                        <type typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass"" typeName=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass"" />
                    </typeArguments>
                </type>
            </typeParameter>
        </typeParameters>
        <xmldoc>
            <![CDATA[
            <summary>
                Existing Docs for GenericStructWithConstraints-T
            </summary>
            ]]>
        </xmldoc>
        <member name=""GenericMethodWithGenericConstraint`1"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                    <parameter name=""t2"">
                        <typeParameter declaringTypeId="""" name=""T2"" />
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
                <![CDATA[
                <summary>
                    Existing GenericStructWithConstraints-T.GenericMethodWithGenericConstraint-T2
                </summary>
                ]]>
            </xmldoc>
        </member>
        <member name=""GenericMethodWithGenericConstraint"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                </parameters>
            </signature>
            <xmldoc>
                <![CDATA[
                <summary>
                    Existing GenericStructWithConstraints-T.GenericMethodWithGenericConstraint
                </summary>
                ]]>
            </xmldoc>
        </member>
  </member>"
                }).SetName("GenericStructWithConstraints");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/GenericOverload.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.GenericOverload",
                    expectedXml =
                        @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""GenericOverload"" type=""Struct"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        <xmldoc><![CDATA[]]></xmldoc>
        <member name=""GenericMethod`1"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters></parameters>
                <typeParameters>
                    <typeParameter name=""T"" />
                </typeParameters>
            </signature>
            <xmldoc>
                <![CDATA[
                    <summary>
                    Existing GenericMethod-T
                    </summary>]]>
            </xmldoc>
        </member>
        <member name=""GenericMethod"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters></parameters>
            </signature>
            <xmldoc>
                <![CDATA[
                    <summary>
                    Existing GenericMethod
                    </summary>]]>
            </xmldoc>
        </member>
    </member>
</doc>"
                }).SetName("Generic_Overload");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithExtensionMethods.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithExtensionMethods",
                    expectedXml =
                        @"<member name=""ClassWithExtensionMethods"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" isStatic=""true"">
<xmldoc>
    <![CDATA[]]>
</xmldoc>
<member name=""ExtensionMethod"" type=""Method"" methodKind=""Ordinary"" isStatic=""true"" isExtensionMethod=""true"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Int32"" typeName=""int"" />
        </return>
        <parameters>
            <parameter name=""s"">
                <type typeId=""System.String"" typeName=""string"" />
            </parameter>
        </parameters>
    </signature>
    <attributes>
        <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
    </attributes>
    <xmldoc><![CDATA[
        <summary>
        Extension method
        </summary>]]></xmldoc>
</member>
</member>"
                }).SetName("Extension_Method_Is_Reported");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithOptionalParameters.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOptionalParameters",
                    expectedXml =
                        @"<member name=""OptionalInt"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""i"" isOptional=""true"" defaultValue=""3"">
                <type typeId=""System.Int32"" typeName=""int"" />
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[<summary>
        OptionalInt
        </summary>]]>
    </xmldoc>
</member>
<member name=""OptionalNoDefaultValue"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""s"" isOptional=""true"">
                <type typeId=""System.String"" typeName=""string"" />
                <attributes>
                    <attribute typeId=""System.Runtime.InteropServices.OptionalAttribute"" />
                </attributes>
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[<summary>
        OptionalNoDefaultValue
        </summary>]]>
    </xmldoc>
</member>
<member name=""OptionalConstValue"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""f"" isOptional=""true"" defaultValue=""4"">
                <type typeId=""System.Single"" typeName=""float"" />
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[<summary>
        OptionalConstValue
        </summary>]]>
    </xmldoc>
</member>
<member name=""OptionalDefaultStruct"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""s"" isOptional=""true"" defaultValue=""default(AStruct)"">
                <type typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOptionalParameters.AStruct"" typeName=""DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithOptionalParameters.AStruct"" />
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[<summary>
        OptionalDefaultStruct
        </summary>]]>
    </xmldoc>
</member>
    <member name=""OptionalStringWithNonValidXMLChars"" type=""Method"" methodKind=""Ordinary"">
      <signature>
        <accessibility>Public</accessibility>
        <return>
          <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
          <parameter name=""s"" isOptional=""true"" defaultValue=""&lt;&gt;&amp;&quot;"">
            <type typeId=""System.String"" typeName=""string"" />
          </parameter>
        </parameters>
      </signature>
      <xmldoc><![CDATA[
    <summary>
    OptionalStringInvalidXmlChars
    </summary>

]]></xmldoc>",
                }).SetName("Optional_Parameter_Is_Reported");

            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/CommonTypes/GlobalDelegate.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.ADelegate",
                    expectedXml =
                        @"<member name=""ADelegate"" type=""Delegate"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"" inherits=""System.MulticastDelegate"" isSealed=""true"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
    <type typeId=""System.Int32"" typeName=""int"" />
</return>
        <parameters>
            <parameter name=""o"">
                <type typeId=""System.Object"" typeName=""object"" />
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[
        <summary>
        A Delegate
        </summary>]]>
    </xmldoc>
</member>"
                }).SetName("Global_Delegate");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/CommonTypes/AClass.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.Delegate",
                    expectedXml =
                        @"<member name=""Delegate"" type=""Delegate"" containingType=""AClass"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes"" inherits=""System.MulticastDelegate"" isSealed=""true"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters></parameters>
    </signature>
    <xmldoc><![CDATA[
        <summary>
        Delegate
        </summary>]]>
    </xmldoc>
</member>"
                }).SetName("Inner_Delegate");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/DerivedClass.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.DerivedClass",
                    expectedXml =
                        @"<member name=""DerivedClass"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"" inherits=""DocWorks.Integration.XmlDoc.Tests.TestTypes.SimpleClassWithXmlDoc"">
    <xmldoc><![CDATA[
        <summary>
        DerivedClass docs
        </summary>]]>
    </xmldoc>
</member>"
                }).SetName("Derived_Class");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Attributes/ClassWithAttributes.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.ClassWithAttributes",
                    expectedXml =
                        @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithAttributes"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes"">

        <attributes>
            <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"">
                <constructorArguments>
                    <argument value=""50"">
                        <type typeId=""System.Int32"" typeName=""int"" />
                    </argument>
                </constructorArguments>
                <namedArguments>
                    <argument name=""AnEnum"" value=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.AnEnum.Value"">
                        <type typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.AnEnum"" typeName=""DocWorks.Integration.XmlDoc.Tests.TestTypes.GetTypes.AClass.AnEnum"" />
                    </argument>
                    <argument name=""AnString"" value=""Foo"">
                        <type typeId=""System.String"" typeName=""string"" />
                    </argument>
                </namedArguments>
            </attribute>
        </attributes>
        <xmldoc>
            <![CDATA[]]>
        </xmldoc>
        <member name=""MethodWithParameter"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Void"" typeName=""void"" />
                </return>
                <parameters>
                    <parameter name=""i"">
                        <type typeId=""System.Int32"" typeName=""int"" />
                        <attributes>
                            <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
                        </attributes>
                    </parameter>
                </parameters>
            </signature>
            <xmldoc>
                <![CDATA[]]>
            </xmldoc>
        </member>
        <member name=""MethodWithReturn"" type=""Method"" methodKind=""Ordinary"">
            <signature>
                <accessibility>Public</accessibility>
                <return>
                    <type typeId=""System.Int32"" typeName=""int"" />
                    <attributes>
                        <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
                    </attributes>
                </return>
                <parameters></parameters>
            </signature>
            <xmldoc>
                <![CDATA[]]>
            </xmldoc>
        </member>
    </member>
</doc>"
                }).SetName("Class_With_Attributes");

            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Attributes/ClassWithExternalAttribute.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.ClassWithExternalAttribute",
                    expectedXml =
                        @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithExternalAttribute"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes"">
        <attributes>
            <attribute typeId=""DocWorks.Integration.XmlDoc.TestUtilities.PublicExternalAttribute"">
                <constructorArguments>
                    <argument value=""true"">
                        <type typeId=""System.Boolean"" typeName=""bool"" />
                    </argument>
                </constructorArguments>
            </attribute>
        </attributes>
        <xmldoc>
            <![CDATA[]]>
        </xmldoc>
    </member>
</doc>",
                    referencedAssemblyPaths = new[] { typeof(PublicExternalAttribute).Assembly.Location }
                }).SetName("Class_With_External_Attribute_With_Reference");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Attributes/ClassWithExternalAttribute.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.ClassWithExternalAttribute",
                    expectedXml =
                        @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithExternalAttribute"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes"">
        <xmldoc>
            <![CDATA[]]>
        </xmldoc>
    </member>
</doc>",
                }).SetName("Class_With_External_Attribute_Without_Reference");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Attributes/ClassWithAttributeWithStringArgument.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.ClassWithAttributeWithStringArgument",
                    expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithAttributeWithStringArgument"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes"">
        <attributes>
            <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"">
                <constructorArguments>
                    <argument value=""string"">
                        <type typeId=""System.String"" typeName=""string"" />
                    </argument>
               </constructorArguments>
            </attribute>
        </attributes>
        <xmldoc><![CDATA[]]></xmldoc><member name=""Foo"" type=""Method"" methodKind=""Ordinary"">
      <signature>
        <accessibility>Public</accessibility>
        <return>
          <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters></parameters>
      </signature>
      <attributes>
        <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"">
          <constructorArguments>
            <argument value=""&amp; &gt; &lt; &quot; '"">
              <type typeId=""System.String"" typeName=""string"" />
            </argument>
          </constructorArguments>
        </attribute>
      </attributes>
      <xmldoc><![CDATA[]]></xmldoc>
    </member>
    </member>
</doc>",
                    exact = true
                }).SetName("Class_With_Attribute_With_String_Argument");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Attributes/StructWithAttributes.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.StructWithAttributes",
                    expectedXml =
                        @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""StructWithAttributes"" type=""Struct"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes"">
        <attributes>
            <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
        </attributes>
        <xmldoc>
            <![CDATA[]]>
        </xmldoc>
    </member>
</doc>"
                }).SetName("Struct_With_Attributes");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Attributes/IInterfaceWithAttributes.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.IInterfaceWithAttributes",
                    expectedXml =
                        @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""IInterfaceWithAttributes"" type=""Interface"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes"">
        <attributes>
            <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
        </attributes>
        <xmldoc>
            <![CDATA[]]>
        </xmldoc>
    </member>
</doc>"
                }).SetName("Interface_With_Attributes");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Attributes/DelegateWithAttributes.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.DelegateWithAttributes",
                    expectedXml =
                        @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""DelegateWithAttributes"" type=""Delegate"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes"" inherits=""System.MulticastDelegate"" isSealed=""true"">
        <signature>
            <accessibility>Public
            </accessibility>
            <return>
                <type typeId=""System.Void"" typeName=""void"" />
            </return>
            <parameters>
            </parameters>
        </signature>
        <attributes>
            <attribute typeId=""DocWorks.Integration.XmlDoc.Tests.TestTypes.Attributes.TestPublicAttribute"" />
        </attributes>
        <xmldoc>
            <![CDATA[]]>
        </xmldoc>
    </member>
</doc>"
                }).SetName("Delegate_With_Attributes");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithParams.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithParams",
                    expectedXml =
                        @"
<member name=""Method"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""ints"" isParams=""true"">
                <type typeId=""System.Int32[]"" typeName=""int[]"">
                    <type typeId=""System.Int32"" typeName=""int"" />
                </type>
            </parameter>
        </parameters>
    </signature>
    <xmldoc>
        <![CDATA[
    <summary>
    Params field
    </summary>]]>
    </xmldoc>
</member>"
                }).SetName("Params_Parameter");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithPointerParam.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithPointerParam",
                    expectedXml =
                        @"<member name=""Method"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""p"">
                <type typeId=""System.Int32*"" typeName=""int*"">
                    <type typeId=""System.Int32"" typeName=""int"" />
                </type>
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[]]></xmldoc>
</member>"
                }).SetName("Class_With_Pointer_Param");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Generics/GenericMethodParameter.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericMethodParameter",
                    expectedXml =
                        @"<member name=""Method"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""ints"">
                <type typeId=""System.Collections.Generic.List`1"" typeName=""System.Collections.Generic.List&lt;int&gt;"">
                    <typeArguments>
                        <type typeId=""System.Int32"" typeName=""int"" />
                    </typeArguments>
                </type>
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[]]></xmldoc>
</member>"
                }).SetName("Generic_Method_Parameter");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Generics/GenericArrayMethodParameter.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericArrayMethodParameter",
                    expectedXml =
                        @"<member name=""Method"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""ints"">
                <type typeId=""System.Collections.Generic.List`1[]"" typeName=""System.Collections.Generic.List&lt;int&gt;[]"">
                    <type typeId=""System.Collections.Generic.List`1"" typeName=""System.Collections.Generic.List&lt;int&gt;"">
                        <typeArguments>
                            <type typeId=""System.Int32"" typeName=""int"" />
                        </typeArguments>
                    </type>
                </type>
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[]]></xmldoc>
</member>"
                }).SetName("Generic_Array_Method_Parameter");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/Generics/GenericArrayField.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.Generics.GenericArrayField",
                    expectedXml =
                        @"<member name=""intListArray"" type=""Field"">
    <signature>
        <accessibility>Public</accessibility>
        <type typeId=""System.Collections.Generic.List`1[]"" typeName=""System.Collections.Generic.List&lt;int&gt;[]"">
            <type typeId=""System.Collections.Generic.List`1"" typeName=""System.Collections.Generic.List&lt;int&gt;"">
                <typeArguments>
                    <type typeId=""System.Int32"" typeName=""int"" />
                </typeArguments>
            </type>
        </type>
    </signature>
    <xmldoc><![CDATA[List Array]]></xmldoc>
</member>
<member name=""intList"" type=""Field"">
    <signature>
        <accessibility>Public</accessibility>
        <type typeId=""System.Collections.Generic.List`1"" typeName=""System.Collections.Generic.List&lt;int&gt;"">
            <typeArguments>
                <type typeId=""System.Int32"" typeName=""int"" />
            </typeArguments>
        </type>
    </signature>
    <xmldoc><![CDATA[List]]></xmldoc>
</member>"
                }).SetName("Generic_Array_Field");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithCDataXml.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithCDataXml",
                    expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithCDataXml"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        <xmldoc><![CDATA[
            <summary>
            &lt;![CDATA[
            Some
            &lt;CData&gt;
            ]]&gt;
            </summary>
]]></xmldoc>
    </member>
</doc>"
                }).SetName("CData_Xml");
            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/ClassWithMalformedXml.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithMalformedXml",
                    expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
    <member name=""ClassWithMalformedXml"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
        <xmldoc><![CDATA[<!-- Badly formed XML comment ignored for member ""T:DocWorks.Integration.XmlDoc.Tests.TestTypes.ClassWithMalformedXml"" -->]]></xmldoc>
    </member>
</doc>"
                }).SetName("Malformed_Xml");

            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/EscapeCharactersXml.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.EscapeCharactersXml",
                    expectedXml = @"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<doc version=""3"">
  <member name=""EscapeCharactersXml"" type=""Class"" namespace=""DocWorks.Integration.XmlDoc.Tests.TestTypes"">
    <xmldoc><![CDATA[
    <summary>
    &lt;&gt; &amp; &apos; &quot;
    </summary>
]]></xmldoc>
  </member>
</doc>"
                }).SetName("EscapeCharacters_Xml");

            yield return new TestCaseData(
                new TestIsReportedData
                {
                    sourceFile = "TestTypes/DefaultExternalEnumParameter.cs",
                    typeId = "DocWorks.Integration.XmlDoc.Tests.TestTypes.DefaultExternalEnumParameter",
                    expectedXml =
                        @"<member name=""Method"" type=""Method"" methodKind=""Ordinary"">
    <signature>
        <accessibility>Public</accessibility>
        <return>
            <type typeId=""System.Void"" typeName=""void"" />
        </return>
        <parameters>
            <parameter name=""v"" isOptional=""true"" defaultValue=""ExternalEnum.Value"">
                <type typeId=""ExternalEnum"" typeName=""ExternalEnum"" />
            </parameter>
        </parameters>
    </signature>
    <xmldoc><![CDATA[]]></xmldoc>
</member>"
                }).SetName("Default_External_Enum_Parameter");
        }

        [Test]
        [TestCaseSource(nameof(TestIsReportedTestCases))]
        public void Test_Is_Reported(TestIsReportedData data)
        {
            var handler = new XMLDocHandler(MakeCompilationParameters(".", data.referencedAssemblyPaths));
            string actualXml = handler.GetTypeDocumentation(data.typeId, data.sourceFile);
            Console.WriteLine(actualXml);

            AssertValidXml(actualXml);
            if (data.exact)
                AssertXml(data.expectedXml, actualXml);
            else
                AssertXmlContains(data.expectedXml, actualXml);
        }

        [Test]
        public void GivenDuplicateTypes_WithOneTypeDefinitionExcluded_ExclusionPathIsAbsolute_GetTypeDocumentation_ShouldReturn_OneMember()
        {
            string excludePath = Path.Combine(AppContext.BaseDirectory, "TestTypes", "Excluded");
            CompilationParameters compilationParameters = new CompilationParameters(AppContext.BaseDirectory, new[] { excludePath }, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            XMLDocHandler handler = new XMLDocHandler(compilationParameters);
            string actualXml = handler.GetTypeDocumentation("DocWorks.Integration.XmlDoc.Tests.TestTypes.DuplicateClass", "TestTypes/DuplicateClass.cs");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(actualXml);
            int nodeCount = doc.DocumentElement.SelectNodes("member/member").Count;
            Assert.That(nodeCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenDuplicateTypes_WithOneTypeDefinitionExcluded_ExclusionPathIsRelative_GetTypeDocumentation_ShouldReturn_OneMember()
        {
            string excludePath = Path.Combine("TestTypes", "Excluded");
            CompilationParameters compilationParameters = new CompilationParameters(AppContext.BaseDirectory, new[] { excludePath }, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            XMLDocHandler handler = new XMLDocHandler(compilationParameters);
            string actualXml = handler.GetTypeDocumentation("DocWorks.Integration.XmlDoc.Tests.TestTypes.DuplicateClass", "TestTypes/DuplicateClass.cs");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(actualXml);
            int nodeCount = doc.DocumentElement.SelectNodes("member/member").Count;
            Assert.That(nodeCount, Is.EqualTo(1));
        }

        private void AssertValidXml(string actualXml)
        {
            var doc = new XmlDocument();
            Assert.DoesNotThrow(() => doc.LoadXml(actualXml), $@"Xml parse error. Xml:
{actualXml}");
        }
    }
}
