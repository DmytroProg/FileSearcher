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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace FileSearcher.ViewModels
{
    public class FileSearchSettingsViewModel : ViewModelBase
    {
        private string _selectedFolder;
        private NavigationStore _navigationStore;

        public ObservableCollection<LogicalDriveModel> LogicalDrives { get; }
        public string SelectedFolder {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged();
            }
        }

        public FileSearchSettingsViewModel(NavigationStore navigationStore)
        {
            LogicalDrives = new ObservableCollection<LogicalDriveModel>(); 
            LoadDrives();

            _navigationStore = navigationStore;
            StartCommand = new NavigationCommand(_navigationStore, CreateFileSearcherProcessingViewModel());
        }

        private FileSearcherProcessingViewModel CreateFileSearcherProcessingViewModel()
        {
            return new FileSearcherProcessingViewModel(_navigationStore,
            new FileSearchOptions()
            {
                Drives = LogicalDrives.Select(x => x.DriveName).AsEnumerable(),
                SelectedFolder = SelectedFolder
            });
        }

        /// <summary>
        /// Methods for adding information about logical drives in the collection LogicalDrives
        /// </summary>
        private void LoadDrives()
        {
            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.Name == "C:\\")
                    continue;
                LogicalDrives.Add(new LogicalDriveModel()
                {
                    DriveName = drive.Name,
                    IsChecked = false,
                    DriveFormat = drive.DriveFormat
                });
            }
        }

        public ICommand SelectFolderCommand
        {
            get => new RelayCommand(SelectFolder);
        }

        private void SelectFolder()
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedFolder = folderBrowserDialog.SelectedPath;
            }
        }

        public ICommand StartCommand { get; }
    }
}
