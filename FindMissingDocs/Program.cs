using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using DocWorks.Integration.XmlDoc;
using Unity.Options;

[assembly: InternalsVisibleTo("DocWorks.Integration.XmlDoc.Tests")]

namespace DocWorks.Integration.FindMissingDocs
{
    class Program
    {
        [ProgramOptions]
        internal class DriverOptions
        {
            public static string RootPath;
            public static string[] ExcludedPaths;
            public static string[] Defines;
        }

        private static string TestFileDirectory = "TestFiles";

        static void Main(string[] args)
        {
            if (OptionsParser.HelpRequested(args))
            {
                OptionsParser.DisplayHelp(typeof(Program).Assembly);
                return;
            }

            OptionsParser.Prepare(args, typeof(Program).Assembly);

            Console.WriteLine("API missing docs:");
            FindMissingDocs(DriverOptions.RootPath,
                DriverOptions.ExcludedPaths,
                DriverOptions.Defines,
                Console.WriteLine);
        }

        internal static void FindMissingDocs(string rootPath, string[] excludedPaths, string[] defines, Action<string> foundMemberMissingDocCallback)
        {
            var handler = new XMLDocHandler(new CompilationParameters(
                rootPath ?? ".",
                excludedPaths ?? new string[0],
                defines ?? new string[0],
                new string[0],
                new string[0]));

            string typesXml = handler.GetTypesXml();
            XmlDocument getTypesXml = new XmlDocument();

            getTypesXml.LoadXml(typesXml);

            foreach (XmlNode typeNode in getTypesXml.SelectNodes("//type"))
            {
                var id = typeNode.SelectSingleNode("id").InnerText;
                var pathNodes = typeNode.SelectNodes("relativeFilePaths/path");
                var paths = new List<string>();
                foreach (XmlNode pathNode in pathNodes)
                    paths.Add(pathNode.InnerText);

                string typeXml = handler.GetTypeDocumentation(id, paths.ToArray());
                XmlDocument getTypeXml = new XmlDocument();
                getTypeXml.LoadXml(typeXml);

                var xmlDocNodes = getTypeXml.SelectNodes("//xmldoc");

                foreach (XmlNode xmlDocNode in xmlDocNodes)
                {
                    if (string.IsNullOrEmpty(xmlDocNode.InnerText))
                    {
                        var parent = xmlDocNode.ParentNode;
                        var name = parent.Attributes["name"].Value;
                        if (id.EndsWith(name)) //a rough way of detecting whether we are looking at the type itself
                        {
                            foundMemberMissingDocCallback(id);
                        }
                        else
                        {
                            foundMemberMissingDocCallback(id + "." + name);
                        }
                    }
                }
            }
        }
    }
}
