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
        private bool isPaused;
        private IEnumerable<string> _illegalWords;
        private CancellationTokenSource cancellationTokenSource;


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
            cancellationTokenSource = new CancellationTokenSource();
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

            await Task.Run(() =>
            {
                Parallel.ForEach(_drives, name => count += GetFilesCount(name));
            });

            MaxFilesCount = count;
            return count;
        }

        private int GetFilesCount(string path)
        {
            int count = 0;
            count += Directory.GetFiles(path).Length;
            foreach (var inner in Directory.EnumerateDirectories(path))
            {
                try
                {
                    count += GetFilesCount(inner);
                }
                catch (Exception) { }
            }
            return count;
        }

        /// <summary>
        /// Get the list of file pathes that contain illigal words 
        /// </summary>
        /// <returns>list of file pathes</returns>
        public async Task<List<string>> GetAllIllegalFiles()
        {
            var illegalFiles = new List<string>();
            FilesCount = 0;

            var tasks = new List<Task>();

            try
            {
                foreach (var name in _drives)
                {
                    tasks.Add(GetIllegalFilesAsync(illegalFiles, name, cancellationTokenSource.Token));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Handle exceptions as needed
            }

            return illegalFiles;
        }

        private async Task GetIllegalFilesAsync(List<string> files, string driveName, CancellationToken cancellationToken)
        {
            foreach (var file in Directory.EnumerateFiles(driveName))
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    while (isPaused)
                    {
                        await Task.Delay(100);
                    }
                    await Task.Factory.StartNew(() => CheckFileForIllegalWordsAsync(files, file, cancellationToken));
                }
                catch (OperationCanceledException) { FilesCount = -1; return; }
                catch (Exception) { }
            }

            foreach (var directory in Directory.EnumerateDirectories(driveName))
            {
                try
                {
                    await GetIllegalFilesAsync(files, directory, cancellationToken);
                }
                catch (Exception) { }
            }
        }

        private void CheckFileForIllegalWordsAsync(List<string> files, string path, CancellationToken cancellationToken)
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
                                try
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                                catch(OperationCanceledException) { FilesCount = -1; return; }
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

        /// <summary>
        /// Copies file with illigal words to folder
        /// </summary>
        /// <param name="path">path to the folder</param>
        private void CopyFileToFolder(string path)
        {
            FileInfo fileInfo = new(path);
            File.Copy(fileInfo.FullName, Path.Combine(_folderPath, fileInfo.Name));
        }

        private async Task ChangeIllegalWords(string path)
        {
            throw new NotImplementedException();
        }

        public async Task CopyFilesAndChanngeIllegalWords(List<string> files)
        {
            await Task.Run(() =>
            {
                foreach (var file in files)
                {
                    try
                    {
                        CopyFileToFolder(file);
                    }
                    catch (Exception) { }
                }
            });
        }

        public async Task CreateInfoDoc(string path)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        public void Continue()
        {
            isPaused = false;
        }
    }
}
