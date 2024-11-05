using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Service;
using WpfApp1.Store;
using WpfApp1.ViewModel;

namespace WpfApp1.Commands
{
    public class NavigateCommand<TViewModel> : CommandBase
        where TViewModel : ViewModelBase
    {
        private readonly INavigationService<TViewModel> _navigationService;
        public override void Execute(object parameter)
        {
            _navigationService.Navigate();
        }

        public NavigateCommand(INavigationService<TViewModel> navigationService)
        {
            _navigationService = navigationService;

        }
    }
}
