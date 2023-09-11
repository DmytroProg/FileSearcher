using FileSearcher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearcher.Stores
{
    public class NavigationService
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<ViewModelBase> _viewModelBase;

        public NavigationService(NavigationStore navigationStore, Func<ViewModelBase> viewModelBase)
        {
            _navigationStore = navigationStore;
            _viewModelBase = viewModelBase;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _viewModelBase.Invoke();
        }
    }
}
