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
        private NavigationService _navigationService;

        public NavigationCommand(NavigationStore navigationStore, Func<ViewModelBase> viewModelBase)
        {
           _navigationService = new NavigationService(navigationStore, viewModelBase);
        }

        public NavigationCommand(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Execute(object? parameter)
        {
            _navigationService.Navigate();
        }
    }
}
