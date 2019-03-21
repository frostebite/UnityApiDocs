using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NUnit.Framework;

namespace DocWorks.Integration.XmlDoc.Tests
{
    [TestFixture]
    class IntegrationTests : XmlDocHandlerTestBase
    {
        private const string TestFileDirectory = "TestTypes/";

        static IEnumerable<TestCaseData> CanReadTypesAndWriteToAllMembersTestCases()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var handler = new XMLDocHandler(MakeCompilationParameters(TestFileDirectory));
            string typesXml = handler.GetTypesXml();
            XmlDocument getTypesXml = new XmlDocument();
            Assert.DoesNotThrow(() => getTypesXml.LoadXml(typesXml), "Failed to parse types xml: \n" + typesXml);

            foreach (XmlNode typeNode in getTypesXml.SelectNodes("//type"))
            {
                var id = typeNode.SelectSingleNode("id").InnerText;
                var pathNodes = typeNode.SelectNodes("relativeFilePaths/path");
                var paths = new List<string>();
                foreach (XmlNode pathNode in pathNodes)
                    paths.Add(pathNode.InnerText);

                var pathArray = paths.ToArray();

                yield return new TestCaseData(handler, id, pathArray).SetName(id);
            }
        }

        /// <summary>
        /// Exercizes GetTypesXml, GetTypeDocumentation, and SetType.
        /// 
        /// Iterates over all types returned for the TestTypes folder, getting docs for each one.
        /// The doc is then modified to put random xmldocs on each documented member and written back using SetType.
        /// We then check to see if all random docs have been written to the files.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(CanReadTypesAndWriteToAllMembersTestCases))]
        public void CanReadTypesAndWriteToAllMembers(XMLDocHandler handler, string id, string[] paths)
        {
            var random = new Random();
            string typeXml = handler.GetTypeDocumentation(id, paths);
            XmlDocument getTypeXml = new XmlDocument();
            Assert.DoesNotThrow(() => getTypeXml.LoadXml(typeXml), "Failed to parse type xml: \n" + typeXml);

            var xmlDocNodes = getTypeXml.SelectNodes("//xmldoc");
            HashSet<string> randomComments = new HashSet<string>();
            foreach (XmlNode xmlDocNode in xmlDocNodes)
            {
                var randomComment = random.Next().ToString();
                randomComments.Add(randomComment);
                xmlDocNode.ReplaceChild(getTypeXml.CreateCDataSection(randomComment), xmlDocNode.FirstChild);
            }

            var tempPaths = paths.ToDictionary(p => p, p =>
            {
                var tempPath = Path.GetTempFileName();
                File.Copy(Path.Combine(TestFileDirectory, p), tempPath, true);
                return tempPath;
            });

            var getTypeXmlString = getTypeXml.OuterXml;
            handler.SetType(getTypeXmlString, paths);
            foreach (var path in paths)
            {
                var fullPath = Path.Combine(TestFileDirectory, path);
                var content = File.ReadAllText(fullPath);
                randomComments.RemoveWhere(comment => content.Contains("/// " + comment));
                File.Copy(tempPaths[path], fullPath, true);
            }

            Assert.IsEmpty(randomComments, "Did not write all comments back properly. Xml used to set:\n" + getTypeXmlString);
        }
        
        [Test]
        [TestCaseSource(nameof(CanReadTypesAndWriteToAllMembersTestCases))]
        public void CanReadTypesAndWriteToAllMembers_WithoutEditingXml(XMLDocHandler handler, string id, string[] paths)
        {
            string typeXml = handler.GetTypeDocumentation(id, paths);

            var tempPaths = paths.ToDictionary(p => p, p =>
            {
                var tempPath = Path.GetTempFileName();
                File.Copy(Path.Combine(TestFileDirectory, p), tempPath, true);
                return tempPath;
            });

            try
            {
                handler.SetType(typeXml, paths);
                foreach (var path in paths)
                {
                    var fullPath = Path.Combine(TestFileDirectory, path);
                    var content = File.ReadAllText(fullPath);
                    var tempPathContent = File.ReadAllText(tempPaths[path]);
                    Assert.AreEqual(tempPathContent, content);
                }
            }
            finally
            {
                foreach (var path in paths)
                {
                    var fullPath = Path.Combine(TestFileDirectory, path);
                    File.Copy(tempPaths[path], fullPath, true);
                }
            }
        }
    }
}
