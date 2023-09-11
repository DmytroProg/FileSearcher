using FileSearcher.ViewModels.Base;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileSearcher.ViewModels
{
    public class FileSearchIllegalWordsViewModel : ViewModelBase
    {
        private readonly string illegalWordsPath;
        public ObservableCollection<string> IllegalWords { get; set; }

        public FileSearchIllegalWordsViewModel(string illegalWordsPath)
        {
            IllegalWords = new ObservableCollection<string>();
            this.illegalWordsPath = illegalWordsPath;

        }

        public ICommand AddIllegalWordCommand
        {
            get => new RelayCommand<string>(word =>
            {
                IllegalWords.Add(word);
                WriteWordToFile();
            });
        }

        private void WriteWordToFile()
        {
            using (var fileStream = new FileStream(illegalWordsPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    foreach (var word in IllegalWords)
                    {
                        streamWriter.WriteLine(word);
                    }
                }
            }
        }
    }
}
