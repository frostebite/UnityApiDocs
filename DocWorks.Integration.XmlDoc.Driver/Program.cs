using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DocWorks.Integration.XmlDoc.Driver
{
    class Program
    {
        private static long total;

        static void Main(string[] args)
        {
            var folders = new[] { "Editor", "Extensions", "Runtime", "Modules" };

            if (args.Length == 2)
            {
                folders = args[1].Split(',');
            }

            foreach (var subfolder in folders)
            {
                var rootPath = args.Length  >= 1 ? args[0] : @"G:\Work\repo\Trunk";
                var sourceFolder = Path.Combine(rootPath, subfolder);

                var handler = new XMLDocHandler(MakeCompilationParameters(sourceFolder));
                var typesXML = handler.GetTypesXml();

                var xml = new XmlDocument();
                xml.LoadXml(typesXML);

                var types = xml.SelectNodes("doc/types/type");

                foreach (XmlNode typeNode in types)
                {
                    var id = string.Empty;
                    try
                    {
                        var relativeFilePaths = typeNode.SelectNodes("relativeFilePaths/path").OfType<XmlNode>()
                            .Select(node => node.InnerText).ToArray();

                        id = typeNode.SelectSingleNode("id").InnerText;

                        var sw = Stopwatch.StartNew();
                        string actualXml = handler.GetTypeDocumentation(id, relativeFilePaths);

                        sw.Stop();
                        total += sw.ElapsedMilliseconds;
                        Console.WriteLine($"{id} : {sw.ElapsedMilliseconds} ms");

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"\r\n\r\n============================== FAILED (id : {id}) ===================================================");
                        Console.WriteLine("Exception: {0}", e);
                        Console.WriteLine("\r\n\r\n------------------------------ GENERATED XML --------------------------------------------");
                    }
                }
            }

            Console.WriteLine($"Total: {total} ms");
        }

        protected static CompilationParameters MakeCompilationParameters(string testFileDirectory, string[] referencedAssemblyPaths = null)
        {
            return new CompilationParameters(testFileDirectory, new string[0], new string[0], referencedAssemblyPaths ?? new string[0]);
        }
    }
}
