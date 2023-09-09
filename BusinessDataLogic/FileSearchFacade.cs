using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessDataLogic
{
    public class FileSearchFacade : INotifyPropertyChanged
    {
        private const int MaxDegreeOfParallelism = 100;

        private readonly FileManager fileManager;
        private readonly string _folderPath;
        private readonly IEnumerable<string> _drives;
        private object locker;
        private int _filesCount;
        private IEnumerable<string> _illegalWords;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int FilesCount
        {
            get => _filesCount;
            set
            {
                _filesCount = value;
                OnPropertyChnaged();
            }
        }

        public int MaxFilesCount { get; private set; }

        public FileSearchFacade(FileSearchOptions fileSearchOptions)
        {
            fileManager = new FileManager();
            locker = new object();
            _drives = fileSearchOptions.Drives;
            _folderPath = fileSearchOptions.SelectedFolder;
            _illegalWords = fileSearchOptions.IlligalWords;
            FilesCount = 0;
        }

        public void OnPropertyChnaged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Get the count of text files in the drives
        /// </summary>
        /// <returns>count of files</returns>
        public async Task<int> GetAllFilesCount()
        {
            int count = 0;
            foreach (var name in _drives)
            {
                await Task.Factory.StartNew(async () => count += await GetFilesCount(name));
            }

            MaxFilesCount = count;
            return count;
        }

        private async Task<int> GetFilesCount(string path)
        {
            int count = 0;
            var directory = new DirectoryInfo(path);
            count += directory.GetFiles().Length;
            foreach (var inner in directory.GetDirectories())
            {
                try
                {
                    count += await GetFilesCount(inner.FullName);
                }
                catch (Exception) { }
            }
            return count;
        }

        /// <summary>
        /// Get the list of file pathes that contain illigal words 
        /// </summary>
        /// <returns>list of file pathes</returns>
        public List<string> GetAllIllegalFiles()
        {
            var illigalFiles = new List<string>();
            FilesCount = 0;

            Parallel.ForEach(_drives, async (name) => await GetIllegalFilesAsync(illigalFiles, name));

            return illigalFiles;
        }

        //private void GetAllPathes(List<string> files, string path)
        //{
        //    foreach(var file in Directory.EnumerateFiles(path))
        //    {
        //        try
        //        {
        //            files.Add(file);
        //        }
        //        catch(Exception) { }
        //    }

        //    foreach(var directory in Directory.EnumerateDirectories(path))
        //    {
        //        try
        //        {
        //            GetAllPathes(files, directory);
        //        }
        //        catch(Exception) { }
        //    }
        //}

        private async Task GetIllegalFilesAsync(List<string> files, string driveName)
        {
            foreach(var file in Directory.EnumerateFiles(driveName))
            {
                try
                {
                    await Task.Factory.StartNew(() => CheckFileForIllegalWordsAsync(files, file));
                }
                catch (Exception) { }
            }

            foreach (var directory in Directory.EnumerateDirectories(driveName))
            {
                try
                {
                    await GetIllegalFilesAsync(files, directory);
                }
                catch (Exception) { }
            }
        }

        private void CheckFileForIllegalWordsAsync(List<string> files, string path)
        {
            lock (locker)
            {
                FilesCount++;

                try
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            while (!sr.EndOfStream)
                            {
                                string? line = sr.ReadLine();
                                if (ContainsIllegalWord(line!))
                                {
                                    files.Add(path);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Handle file access permission issues if needed
                }
                catch (IOException)
                {
                    // Handle file I/O errors if needed
                }
            }
        }

        private bool ContainsIllegalWord(string line)
        {
            //TODO Add Regex
            return _illegalWords.Any(word => line.Contains(word));
        }

        #region TestCheckFilesMethods

        //public async Task<List<string>> GetAllIllegalFiles()
        //{
        //    var illegalFiles = new ConcurrentBag<string>();
        //    var tasks = new List<Task>();
        //    FilesCount = 0;

        //    foreach (var name in _drives)
        //    {
        //        tasks.Add(ScanDriveAsync(name, illegalFiles));
        //    }

        //    await Task.WhenAll(tasks);

        //    return illegalFiles.ToList();
        //}

        //public async Task<List<string>> ScanDriveAsync(string drivePath, ConcurrentBag<string> illegalFiles)
        //{
        //    var directoriesToScan = new ConcurrentQueue<string>();
        //    directoriesToScan.Enqueue(drivePath);

        //    var tasks = new List<Task>();

        //    while (directoriesToScan.Count > 0)
        //    {
        //        if (tasks.Count >= MaxDegreeOfParallelism)
        //        {
        //            await Task.WhenAny(tasks.ToArray());
        //            tasks.RemoveAll(t => t.IsCompleted);
        //        }

        //        if (directoriesToScan.TryDequeue(out string directory))
        //        {
        //            tasks.Add(Task.Run(() => ScanDirectory(directory, illegalFiles, directoriesToScan)));
        //        }
        //    }

        //    await Task.WhenAll(tasks);

        //    return illegalFiles.ToList();
        //}

        //private void ScanDirectory(string directoryPath, ConcurrentBag<string> illegalFiles, ConcurrentQueue<string> directoriesToScan)
        //{
        //    try
        //    {
        //        Parallel.ForEach(
        //            Directory.EnumerateFiles(directoryPath),
        //            new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism },
        //            filePath => CheckFileForIllegalWords(filePath, illegalFiles)
        //        );

        //        foreach (var subdirectory in Directory.EnumerateDirectories(directoryPath))
        //        {
        //            directoriesToScan.Enqueue(subdirectory);
        //        }
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        // Handle directory access permission issues if needed
        //    }
        //}

        //private void CheckFileForIllegalWords(string filePath, ConcurrentBag<string> illegalFiles)
        //{
        //    lock (locker)
        //    {
        //        FilesCount++;
        //    }

        //    try
        //    {
        //        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        //        {
        //            var fileContent = streamReader.ReadToEnd();
        //            if (_illegalWords.Any(word => fileContent.Contains(word)))
        //            {
        //                illegalFiles.Add(filePath);
        //            }
        //        }
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        // Handle file access permission issues if needed
        //    }
        //    catch (IOException)
        //    {
        //        // Handle file I/O errors if needed
        //    }
        //}

        #endregion

        /// <summary>
        /// Copies file with illigal words to folder
        /// </summary>
        /// <param name="path">path to the folder</param>
        private void CopyFileToFolder(string path)
        {
            throw new NotImplementedException();
        }

        private async Task ChangeIllegalWords(string path)
        {
            throw new NotImplementedException();
        }

        public async Task CopyFilesAndChnangeIllegalWords(string path)
        {
            throw new NotImplementedException();
        }

        public async Task CreateInfoDoc(string path)
        {
            throw new NotImplementedException();
        }

        public async Task Pause()
        {
            throw new NotImplementedException();
        }

        public async Task Stop()
        {
            throw new NotImplementedException();
        }

        public async Task Continue()
        {
            throw new NotImplementedException();
        }
    }
}
