using FileSearcher.Stores;
using FileSearcher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearcher.Commands
{
    public class NavigationCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<ViewModelBase> _viewModelBase;

        public NavigationCommand(NavigationStore navigationStore, Func<ViewModelBase> viewModelBase)
        {
            _navigationStore = navigationStore;
            _viewModelBase = viewModelBase;
        }

        public override void Execute(object? parameter)
        {
            _navigationStore.CurrentViewModel = _viewModelBase.Invoke();
        }
    }
}
