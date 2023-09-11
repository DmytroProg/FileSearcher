using BusinessDataLogic;
using FileSearcher.Commands;
using FileSearcher.Models;
using FileSearcher.Stores;
using FileSearcher.ViewModels.Base;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileSearcher.ViewModels
{
    public class FileSearcherProcessingViewModel : ViewModelBase
    {
        private int _maxFilesCount = 1;
        private bool isPaused;
        private bool isRunning;
        private TimeOnly _processingTime;
        private FileSearchFacade _fileSearchFacade;
        private NavigationStore _navigationStore;
        private Visibility _bottomPanelVisibility;
        
        private Dictionary<string, string> _propertyChangedDependencies;
        private Timer timer;

        public ObservableCollection<TaskModel> Tasks { get; set; }

        public int ProcessValue
        {
            get => (_fileSearchFacade.FilesCount * 100) / _maxFilesCount;
        }

        public Visibility BottompanelVisibility
        {
            get => _bottomPanelVisibility;
            set
            {
                _bottomPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public int FilesCount
        {
            get
            {
                OnPropertyChanged(nameof(ProcessValue));
                return _fileSearchFacade.FilesCount;
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

        public ICommand PauseCommand
        {
            get => new RelayCommand(() =>
            {
                if (!isRunning)
                    return;
                isPaused = true;
                _fileSearchFacade.Pause();
            });
        }

        public ICommand CancelCommand
        {
            get => new RelayCommand(() =>
            {
                if (!isRunning)
                    return;
                var result = MessageBox.Show("Are you sure you want to cancel the process?", "Message", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    _fileSearchFacade.Stop();
                    GoBackCommand.Execute(null);
                }
            });
        }

        public ICommand ResumeCommand
        {
            get => new RelayCommand(() => {
                if (!isRunning)
                    return;
                isPaused = false;
                _fileSearchFacade.Continue();
            });
        }

        public ICommand GoBackCommand { get; }

        public FileSearcherProcessingViewModel(NavigationStore navigationStore, NavigationService goBackService, FileSearchOptions fileSearchOptions)
        {
            _fileSearchFacade = new FileSearchFacade(fileSearchOptions);
            _fileSearchFacade.PropertyChanged += _fileSearchFacade_PropertyChanged;
            _propertyChangedDependencies = new Dictionary<string, string>();
            timer = new Timer(OnTimerTick);
            SetPropertyChnagedDependencies();
            _navigationStore = navigationStore;
            BottompanelVisibility = Visibility.Hidden;

            GoBackCommand = new NavigationCommand(goBackService);

            Tasks = new ObservableCollection<TaskModel>();
            SetupTasks();

            if(_navigationStore.CurrentViewModel != null)
            {
                StartProcessingFilesAsync();
            }
        }

        public async void StartProcessingFilesAsync()
        {
            isRunning = true;
            timer.Change(0, 1000);

            int filesCount = await _fileSearchFacade.GetAllFilesCount();
            _maxFilesCount = _fileSearchFacade.MaxFilesCount;
            Tasks[0] = new TaskModel() { IsComplited = true, TaskName = Tasks[0].TaskName };

            var list = await _fileSearchFacade.GetAllIllegalFiles();
            Tasks[1] = new TaskModel() { IsComplited = true, TaskName = Tasks[1].TaskName };

            await _fileSearchFacade.CopyFilesAndChanngeIllegalWords(list);
            Tasks[2] = new TaskModel() { IsComplited = true, TaskName = Tasks[2].TaskName };

            timer.Dispose();
            isRunning = false;
            MessageBox.Show("The files are checked and copied to the folder", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            BottompanelVisibility = Visibility.Visible;
        }

        private void SetPropertyChnagedDependencies()
        {
            _propertyChangedDependencies[nameof(_fileSearchFacade.FilesCount)] = nameof(FilesCount);
        }

        private void _fileSearchFacade_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(_propertyChangedDependencies[e.PropertyName!]);
        }

        private void SetupTasks()
        {
            Tasks.Add(new TaskModel() { IsComplited = false, TaskName = "Selecting files" });
            Tasks.Add(new TaskModel() { IsComplited = false, TaskName = "Searching files" });
            Tasks.Add(new TaskModel() { IsComplited = false, TaskName = "Copying files" });
        }

        private void OnTimerTick(object? obj)
        {
            if (!isPaused)
            {
                ProcessingTime = ProcessingTime.Add(TimeSpan.FromSeconds(1));
            }
        }

        public override void Dispose()
        {
            timer?.Dispose();
            _fileSearchFacade.PropertyChanged -= _fileSearchFacade_PropertyChanged;
            base.Dispose();
        }
    }
}




