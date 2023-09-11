using FileSearcher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSearcher.Views;
using System.Windows.Controls;
using FileSearcher.Stores;
using System.Windows.Input;
using FileSearcher.Commands;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;

namespace FileSearcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly string illedalWordsPath;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore, string illegalWordsPath)
        {
            _navigationStore = navigationStore;
            this.illedalWordsPath = illegalWordsPath;
            _navigationStore.OnViewModelChanged += _navigationStore_OnViewModelChanged;

        }

        public ICommand OpenIllegalWordsViewModel {
            get => new RelayCommand(() => {
                    if (CurrentViewModel is FileSearcherProcessingViewModel)
                        return;
                    new NavigationCommand(_navigationStore, () => new FileSearchIllegalWordsViewModel(illedalWordsPath)).Execute(null);
                });
        }

        public ICommand OpenSettingsViewModel
        {
            get => new RelayCommand(() =>
            {
                if (CurrentViewModel is FileSearcherProcessingViewModel)
                    return;
                new NavigationCommand(_navigationStore, () => new FileSearchSettingsViewModel(_navigationStore, illedalWordsPath)).Execute(null);
            });
        }


        private void _navigationStore_OnViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            _navigationStore.OnViewModelChanged -= _navigationStore_OnViewModelChanged;

            base.Dispose();
        }
    }
}
