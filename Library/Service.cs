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
        private readonly SortedDictionary<string, Dictionary<string, string>> _folderAndFileNameMapping;
        private int _currentFolderCount;
        private int Max = 1000;

        public Service(string extension, string sourcePath, string destinationPath)
        {
            _extension = extension;
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
            _filesToIgnore = new FileFilter();
            _filePatternsToIgnore = new FilePatternFilter();
            _folderAndFileNameMapping = new SortedDictionary<string, Dictionary<string, string>>(new PathSorter());
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
            LoadFilePatterns("FilesToIgnore.xml", ref _filesToIgnore);
            LoadFilePatterns("FilePatternsToIgnore.xml", ref _filePatternsToIgnore);

            for (int i = 0; i < Max; i++)
            {
                _folderAndFileNameMapping.Add(GetNextFolderName(), new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        private void LoadFilePatterns<TFilter>(string fileName, ref TFilter filter)
            where TFilter : IFilter
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var toIgnore = new XmlDocument();
            toIgnore.Load(fileName);
            var deserializedJsonFilesToIgnore = JsonConvert.SerializeXmlNode(toIgnore, Newtonsoft.Json.Formatting.Indented, true);
            filter = JsonConvert.DeserializeObject<TFilter>(deserializedJsonFilesToIgnore);
            filter.AppendExtension(_extension);
            filter.ToIgnoreDict = filter.ToIgnore.ToDictionary(x => x, x => x, StringComparer.OrdinalIgnoreCase);
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
            foreach (var pattern in _filePatternsToIgnore.ToIgnoreDict.Keys)
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
                foreach (string key in _folderAndFileNameMapping.Keys)
                {
                    Dictionary<string, string> listOfFileNames = _folderAndFileNameMapping[key];

                    if (_filesToIgnore.ToIgnoreDict.ContainsKey(data.Name) == false
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
            foreach (string key in _folderAndFileNameMapping.Keys)
            {
                string targetDirectoryPath = key;
                Dictionary<string, string> filePaths = _folderAndFileNameMapping[key];
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
