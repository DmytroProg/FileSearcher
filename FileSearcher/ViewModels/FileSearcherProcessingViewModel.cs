using BusinessDataLogic;
using FileSearcher.Stores;
using FileSearcher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSearcher.ViewModels
{
    public class FileSearcherProcessingViewModel : ViewModelBase
    {
        private int _maxFilesCount = 1;
        private TimeOnly _processingTime;
        private FileSearchFacade _fileSearchFacade;
        private NavigationStore _navigationStore;
        private Dictionary<string, string> _propertyChangedDependencies;
        private Timer timer;

        public Dictionary<string, bool> Tasks { get; set; }

        public int ProcessValue
        {
            get => (_fileSearchFacade.IllegalFilesCount * 100) / _maxFilesCount;
        }

        public int FilesCount
        {
            get
            {
                OnPropertyChanged(nameof(ProcessValue));
                return _fileSearchFacade.IllegalFilesCount;
            }
        }

        public TimeOnly ProcessingTime
        {
            get => _processingTime;
            set
            {
                _processingTime = value;
                OnPropertyChanged();
            }
        }

        public FileSearcherProcessingViewModel(NavigationStore navigationStore, FileSearchOptions fileSearchOptions)
        {
            _fileSearchFacade = new FileSearchFacade(fileSearchOptions);
            _fileSearchFacade.PropertyChanged += _fileSearchFacade_PropertyChanged;
            _propertyChangedDependencies = new Dictionary<string, string>();
            timer = new Timer(OnTimerTick);
            SetPropertyChnagedDependencies();
            _navigationStore = navigationStore;

            Tasks = new Dictionary<string, bool>();
            SetupTasks();

            if(_navigationStore.CurrentViewModel != null)
            {
                StartProcessingFilesAsync();
            }
        }

        public async void StartProcessingFilesAsync()
        {
            timer.Change(0, 1000);

            int filesCount = await _fileSearchFacade.GetAllFilesCount();
            _maxFilesCount = _fileSearchFacade.MaxFilesCount;

            var list = await _fileSearchFacade.GetAllIllegalFiles();

            int a = 1;
        }

        private void SetPropertyChnagedDependencies()
        {
            _propertyChangedDependencies[nameof(_fileSearchFacade.IllegalFilesCount)] = nameof(FilesCount);
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

        private void OnTimerTick(object? obj)
        {
            ProcessingTime = ProcessingTime.Add(TimeSpan.FromSeconds(1));
        }

        public override void Dispose()
        {
            timer?.Dispose();
            _fileSearchFacade.PropertyChanged -= _fileSearchFacade_PropertyChanged;
            base.Dispose();
        }
    }
}
