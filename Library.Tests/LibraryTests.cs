using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Library.Tests
{
    [TestClass]
    public class LibraryTests
    {
        [TestMethod]
        public void CheckIfNumberOfSourceFilesEqualsDistributedFiles()
        {
            string sourcePath = @"..\..\..\Library.Tests\TestFiles";
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    +
                    @"\"
                    +
                    Guid.NewGuid().ToString("N")
                    +
                    @"_"
                    +
                    Guid.NewGuid().ToString("N")
                    + @"\";

            var service = new Service("*.cs", sourcePath, path);
            var timeInSeconds = service.Execute();
            Assert.IsTrue(timeInSeconds > 0);

            var directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList();

            int total = 0;
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.cs");
                files = files.Where(f => !f.Contains("TemporaryGeneratedFile")
                                                     &&
                                                     !f.Contains("AssemblyInfo.cs")
                                                     &&
                                                     !f.Contains("GlobalSuppressions.cs")).ToArray();
                total += files.Length;
            }
            Assert.AreEqual(total, 6 );
        }
    }
}
