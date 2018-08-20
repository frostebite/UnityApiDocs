using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DocWorks.Integration.XmlDoc.Tests
{
    class SaveXmlForMultipleDelegatesTest : XmlDocHandlerTestBase
    {
        public struct UpdateTestData
        {
            public string expectedSource;
            public string sourcePath;
            public bool compareRaw;
        }

        public static IEnumerable<TestCaseData> UpdateTestCases()
        {
            yield return new TestCaseData(
                    new UpdateTestData
                    {
                        expectedSource = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.Integration.XmlDoc.Tests.TestTypes.Delegates
{
    /// <summary>new doc1</summary>
    public class ClassWithMultipleDelegates
    {
        /// <summary>new doc2</summary>
        public delegate float AudioCurveEvaluator(float x);
        /// <summary>new doc3</summary>
        public delegate float AudioCurveAndColorEvaluator(float x, out string col);
    }
}",
                        compareRaw = true,
                        sourcePath = "TestTypes/Delegates/ClassWithMultipleDelegates.cs"
                    }).SetName("Update_Multiple_Delegate");
        }

        [Test]
        [TestCaseSource(nameof(UpdateTestCases))]
        public void Update(UpdateTestData data)
        {
            var handler = new XMLDocHandler(new CompilationParameters(
                Path.GetDirectoryName(data.sourcePath),
                new string[0], new[] { typeof(object).Assembly.Location }, new string[0]));

            string typesXml = handler.GetTypesXml();
            XmlDocument getTypesXml = new XmlDocument();
            getTypesXml.LoadXml(typesXml);

            int i = 1;
            foreach (XmlNode typeNode in getTypesXml.SelectNodes("//type"))
            {
                var id = typeNode.SelectSingleNode("id").InnerText;
                var pathNodes = typeNode.SelectNodes("relativeFilePaths/path");
                var paths = new List<string>();
                foreach (XmlNode pathNode in pathNodes)
                {
                    paths.Add(pathNode.InnerText);
                }

                string typeXml = handler.GetTypeDocumentation(id, paths.ToArray());
                XmlDocument getTypeXml = new XmlDocument();
                getTypeXml.LoadXml(typeXml);

                var xmlDocNodes = getTypeXml.SelectNodes("//xmldoc");
                HashSet<string> randomComments = new HashSet<string>();

                foreach (XmlNode xmlDocNode in xmlDocNodes)
                {
                    var randomComment = "<summary>new doc" + i + "</summary>";
                    randomComments.Add(randomComment);
                    xmlDocNode.ReplaceChild(getTypeXml.CreateCDataSection(randomComment), xmlDocNode.FirstChild);
                }

                var getTypeXmlString = getTypeXml.OuterXml;
                handler.SetType(getTypeXmlString, paths.ToArray());
                i++;
            }

            var actualSource = File.ReadAllText(data.sourcePath);
            AssertSourceContains(data.expectedSource, actualSource, data.compareRaw);
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
    }
}
