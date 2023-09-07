using FileSearcher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileSearcher.Stores
{
    public class NavigationStore
    {
        private ViewModelBase _currentViewModel = null!;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnViewModelChanged?.Invoke();
            }
        }

        public event Action? OnViewModelChanged;
    }
}
