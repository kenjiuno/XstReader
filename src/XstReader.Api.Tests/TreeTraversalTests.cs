using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace XstReader.Api.Tests
{
    public class TreeTraversalTests
    {
        private bool GenerateComparisonSource = true;

        [Test]
        [TestCaseSource(nameof(CollectXSTFiles))]
        public void WalkXstFile2(string xstFile)
        {
            using var xst = new XstFile(xstFile);
            var root = xst.RootFolder;
            var writer = new StringWriter();

            void WalkFolder(XstFolder folder, string prefix)
            {
                prefix += $"{folder.DisplayName}/";
                writer.WriteLine($"{prefix}");

                foreach (var subFolder in folder.GetFolders())
                {
                    WalkFolder(subFolder, prefix);
                }
                foreach (var message in folder.GetMessages())
                {
                    writer.WriteLine($"{prefix}{message.Subject}");

                    foreach (var prop in message.GetType().GetProperties())
                    {
                        if (true
                            && prop.PropertyType.IsGenericType 
                            && prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            && prop.GetValue(message) is IEnumerable items
                        )
                        {
                            foreach (var item in items)
                            {
                                writer.WriteLine($" {prop.Name}: {item}".Replace("\r\n", "\n").Replace("\n", "⏎"));
                            }
                        }
                        else
                        {
                            writer.WriteLine($" {prop.Name}: {(prop.GetValue(message) + "").Replace("\r\n", "\n").Replace("\n", "⏎")}");
                        }
                    }
                }
            }

            WalkFolder(root, "");
            ApplyWalkResult(xstFile, writer);
            Console.WriteLine(writer);
        }

        private void ApplyWalkResult(string xstFile, StringWriter actual)
        {
            var filePathExpected = $"{xstFile}.comparsion-source.txt";
            if (GenerateComparisonSource)
            {
                File.WriteAllText(filePathExpected, actual.ToString());
            }
            else
            {
                var expected = File.ReadAllText(filePathExpected);
                Assert.That(actual.ToString(), Is.EqualTo(expected));
            }
        }

        private static string ResolveRootPath(params string[] tokens)
        {
            var cwd = TestContext.CurrentContext.WorkDirectory;
            while (cwd != null)
            {
                var resolveRoot = Path.Combine(cwd, "ResolveRoot");
                if (File.Exists(resolveRoot))
                {
                    return Path.Combine(new string[] { cwd }.Concat(tokens).ToArray());
                }

                cwd = Path.GetDirectoryName(cwd);
            }
            throw new ArgumentException("`ResolveHere` file not found!");
        }

        public static IEnumerable<object[]> CollectXSTFiles()
        {
            return new object[0][]
                .Concat(
                    Directory.GetFiles(ResolveRootPath("files"), "*.pst", SearchOption.AllDirectories)
                        .Select(xstPath => new object[] { xstPath })
                )
                .Concat(
                    Directory.GetFiles(ResolveRootPath("files"), "*.ost", SearchOption.AllDirectories)
                        .Select(xstPath => new object[] { xstPath })
                )
                ;
        }
    }
}
