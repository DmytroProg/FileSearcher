using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessDataLogic
{
    internal class FileManager
    {
        public string ReadFile(string path)
        {
            var stringBuilder = new StringBuilder();

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string? line = streamReader.ReadLine();
                        stringBuilder.AppendLine(line);
                    }
                }
            }

            return stringBuilder.ToString();
        }

        public void WriteFile(string path, string text)
        {
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    streamWriter.Write(text);
                }
            }
        }
    }
}
