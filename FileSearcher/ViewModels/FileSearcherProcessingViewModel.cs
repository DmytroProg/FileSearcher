using BusinessDataLogic;
using FileSearcher.Stores;
using FileSearcher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearcher.ViewModels
{
    public class FileSearcherProcessingViewModel : ViewModelBase
    {
        private FileSearchFacade _fileSearchFacade;
        private NavigationStore _navigationStore;
        private Dictionary<string, string> _propertyChangedDependencies;

        public Dictionary<string, bool> Tasks { get; set; }

        public int FilesCount
        {
            get => _fileSearchFacade.Count;
            private set => _fileSearchFacade.Count = value;
        }

        public FileSearcherProcessingViewModel(NavigationStore navigationStore, FileSearchOptions fileSearchOptions)
        {
            _fileSearchFacade = new FileSearchFacade(fileSearchOptions);
            _fileSearchFacade.PropertyChanged += _fileSearchFacade_PropertyChanged;
            _propertyChangedDependencies = new Dictionary<string, string>();
            SetPropertyChnagedDependencies();
            _navigationStore = navigationStore;

            Tasks = new Dictionary<string, bool>();
            SetupTasks();

            StartCounting();
        }

        private async Task StartCounting()
        {
            await _fileSearchFacade.GetAllFilesCount();
        }

        private void SetPropertyChnagedDependencies()
        {
            _propertyChangedDependencies[nameof(_fileSearchFacade.Count)] = nameof(FilesCount);
        }

        private void _fileSearchFacade_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(_propertyChangedDependencies[e.PropertyName!]);
        }

        private void SetupTasks()
        {
            Tasks["Selecting files"] = true;
            Tasks["Searching files"] = false;
            Tasks["Copying files"] = false;
            OnPropertyChanged(nameof(Tasks));
        }
    }
}
