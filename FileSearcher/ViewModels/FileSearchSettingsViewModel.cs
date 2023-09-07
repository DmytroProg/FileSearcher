using BusinessDataLogic;
using FileSearcher.Models;
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
        private FileSearchFacade facade;
        private string _selectedFolder;

        public ObservableCollection<LogicalDriveModel> LogicalDrives { get; }
        public string SelectedFolder {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged();
            }
        }

        public FileSearchSettingsViewModel()
        {
            facade = new FileSearchFacade();
            LogicalDrives = new ObservableCollection<LogicalDriveModel>();
            LoadDrives();
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
    }
}
