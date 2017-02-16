using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace Library
{
    public class Service
    {
        private readonly string _extension;
        private readonly string _sourcePath;
        private readonly string _destinationPath;
        private FileFilter _filesToIgnore;
        private FilePatternFilter _filePatternsToIgnore;
        private static readonly SortedDictionary<string, Dictionary<string, string>> FolderAndFileNameMapping = new SortedDictionary<string, Dictionary<string, string>>(new PathSorter());
        private static int _currentFolderCount;
        private static int Max = 1000;

        public Service(string extension, string sourcePath, string destinationPath)
        {
            _extension = extension;
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
        }

        public double Execute()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            Init();
            List<string> directories = GetAllSubDirectories(_sourcePath);

            foreach (string directory in directories)
            {
                List<Data> fileNames = GetAllFileNamesInADirectory(directory);
                ProcessFiles(fileNames);
            }

            WriteToDisk();
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;
        }

        private void Init()
        {
            XmlDocument filesToIgnore = new XmlDocument();
            filesToIgnore.Load("FilesToIgnore.xml");
            var deserializedJsonFilesToIgnore = JsonConvert.SerializeXmlNode(filesToIgnore, Newtonsoft.Json.Formatting.Indented, true);
            _filesToIgnore = JsonConvert.DeserializeObject<FileFilter>(deserializedJsonFilesToIgnore);
            _filesToIgnore.AppendExtension(_extension);
            _filesToIgnore.FilesToIgnoreDict = _filesToIgnore.FilesToIgnore.ToDictionary(x => x, x => x, StringComparer.OrdinalIgnoreCase);

            var filePatternsToIgnore = new XmlDocument();
            filePatternsToIgnore.Load("FilePatternsToIgnore.xml");
            var deserializedJsonFilePatternsToIgnore = JsonConvert.SerializeXmlNode(filePatternsToIgnore, Newtonsoft.Json.Formatting.Indented, true);
            _filePatternsToIgnore = JsonConvert.DeserializeObject<FilePatternFilter>(deserializedJsonFilePatternsToIgnore);
            _filePatternsToIgnore.FilePatternToIgnoreDict = _filePatternsToIgnore.FilePatternsToIgnore.ToDictionary(x => x, x => x, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < Max; i++)
            {
                FolderAndFileNameMapping.Add(GetNextFolderName(), new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        private string GetCurrentFolderName()
        {
            string currentFolderName = _currentFolderCount.ToString();
            return Path.Combine(_destinationPath, currentFolderName);
        }

        private string GetNextFolderName()
        {
            ++_currentFolderCount;
            return GetCurrentFolderName();
        }

        private List<string> GetAllSubDirectories(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList();
        }

        private List<Data> GetAllFileNamesInADirectory(string path)
        {
            List<string> listOfFiles = Directory.GetFiles(path, _extension).ToList();
            List<Data> data = listOfFiles.Select(x => new Data(Path.GetFileName(x), x)).ToList();
            return data;
        }

        private bool DoesFileNameMatchesNotAllowedPatterns(string fileName)
        {
            foreach (var pattern in _filePatternsToIgnore.FilePatternToIgnoreDict.Keys)
            {
                if (fileName.ContainsIgnoreCase(pattern))
                    return true;
            }
            return false;
        }

        private void ProcessFiles(List<Data> files)
        {
            foreach (Data data in files)
            {
                // Sorted Dictionary : FolderAndFileNameMapping - key : folder name, value : listOfFileNames
                foreach (string key in FolderAndFileNameMapping.Keys)
                {
                    Dictionary<string, string> listOfFileNames = FolderAndFileNameMapping[key];

                    if (_filesToIgnore.FilesToIgnoreDict.ContainsKey(data.Name) == false
                        &&
                        DoesFileNameMatchesNotAllowedPatterns(data.Name) == false
                        &&
                        listOfFileNames.ContainsKey(data.Name) == false)
                    {
                        // file doesn't exist in this folder so put it there
                        listOfFileNames.Add(data.Name, data.Path);
                        break;
                    }
                    // a file with the same name already exists in this folder so loop again and look for next folder
                }
            }
        }

        private void WriteToDisk()
        {
            foreach (string key in FolderAndFileNameMapping.Keys)
            {
                string targetDirectoryPath = key;
                Dictionary<string, string> filePaths = FolderAndFileNameMapping[key];
                if (filePaths == null || filePaths.Count == 0)
                    return;

                if (Directory.Exists(targetDirectoryPath) == false)
                    Directory.CreateDirectory(targetDirectoryPath);

                foreach (KeyValuePair<string, string> keyValuePair in filePaths)
                {
                    string sourceFileFullPath = keyValuePair.Value;
                    string targetFileFullPath = Path.Combine(targetDirectoryPath, keyValuePair.Key);

                    File.Copy(sourceFileFullPath, targetFileFullPath, true);
                }
            }
        }
    }
}
