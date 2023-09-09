﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessDataLogic
{
    public class FileSearchFacade : INotifyPropertyChanged
    {
        private readonly FileManager fileManager;
        private readonly string _folderPath;
        private readonly IEnumerable<string> _drives;
        private object locker;
        private int _illegalFilesCount;
        private IEnumerable<string> _illegalWords;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int IllegalFilesCount
        {
            get => _illegalFilesCount;
            set
            {
                _illegalFilesCount = value;
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
            IllegalFilesCount = 0;
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
                await Task.Factory.StartNew(() => count += GetFilesCount(name));
            }
            MaxFilesCount = count;
            return count;
        }

        private int GetFilesCount(string path)
        {
            lock (locker)
            {
                int count = 0;
                var directory = new DirectoryInfo(path);
                count += directory.GetFiles().Length;
                foreach (var inner in directory.GetDirectories())
                {
                    try
                    {
                        count += GetFilesCount(inner.FullName);
                    }
                    catch (Exception) { }
                }
                return count;
            }
        }

        /// <summary>
        /// Get the list of file pathes that contain illigal words 
        /// </summary>
        /// <returns>list of file pathes</returns>
        public async Task<List<string>> GetAllIllegalFiles()
        {
            var illigalFiles = new List<string>();
            var tasks = new List<Task>();
            IllegalFilesCount = 0;

            foreach (var name in _drives)
            {
                tasks.Add(GetIllegalFilesAsync(illigalFiles, name));
            }

            await Task.WhenAll(tasks);

            return illigalFiles;
        }

        private async Task GetIllegalFilesAsync(List<string> files, string path)
        {
            var directory = new DirectoryInfo(path);
            
            foreach(var file in directory.GetFiles())
            {
                string text = await fileManager.ReadFileAsync(file.FullName);

                foreach(var word in _illegalWords)
                {
                    IllegalFilesCount++;
                    /* TODO
                     Add Regex
                     */
                    if (text.Contains(word))
                    {
                        files.Add(file.FullName);
                    }
                }
            }

            foreach (var inner in directory.GetDirectories())
            {
                try
                {
                    await GetIllegalFilesAsync(files, inner.FullName);
                }
                catch (Exception) { }
            }
        }

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
