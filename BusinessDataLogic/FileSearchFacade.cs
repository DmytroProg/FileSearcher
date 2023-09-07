using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessDataLogic
{
    public class FileSearchFacade
    {
        private readonly FileManager fileManager;

        public FileSearchFacade()
        {
            fileManager = new FileManager();
        }

        /// <summary>
        /// Get the count of text files in the disk
        /// </summary>
        /// <param name="path">path to the disk to search for files</param>
        /// <returns>count of files</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> GetFilesCount(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the list of file pathes that contain illigal words 
        /// </summary>
        /// <param name="illigalFiles"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task GetIlligalFiles(List<string> illigalFiles, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies file with illigal words to folder
        /// </summary>
        /// <param name="path">path to the folder</param>
        private void CopyFileToFolder(string path)
        {
            throw new NotImplementedException();
        }

        private async Task ChangeIlligalWords(string path)
        {
            throw new NotImplementedException();
        }

        public async Task CopyFilesAndChnangeIlligalWords(string path)
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
