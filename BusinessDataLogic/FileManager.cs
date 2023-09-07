using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessDataLogic
{
    internal class FileManager
    {
        public async Task<string> ReadFileAsync(string path)
        {
            using(var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using(var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        public async Task WriteFileAsync(string path, string text)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    await streamWriter.WriteAsync(text);
                }
            }
        }
    }
}
