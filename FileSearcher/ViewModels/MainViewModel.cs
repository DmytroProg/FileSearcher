using FileSearcher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSearcher.Views;
using System.Windows.Controls;
using FileSearcher.Stores;

namespace FileSearcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;

            _navigationStore.OnViewModelChanged += _navigationStore_OnViewModelChanged;
        }

        private void _navigationStore_OnViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
