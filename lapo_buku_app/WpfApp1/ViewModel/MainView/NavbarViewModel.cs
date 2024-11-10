using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Service;

namespace WpfApp1.ViewModel.MainView
{
    public class NavbarViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateProfileCommand { get; }
        public ICommand LogoutCommand {  get; }

        public NavbarViewModel(
            INavigationService<BrowsingViewModel> browserNavigationService,
            INavigationService<ProfileViewModel> profileNavigationService,
            Action displayLogout
        ) {
            NavigateHomeCommand = new NavigateCommand<BrowsingViewModel>(browserNavigationService);
            NavigateProfileCommand = new NavigateCommand<ProfileViewModel>(profileNavigationService);  
            LogoutCommand = new LogoutCommand(displayLogout);
        }    
    }
}
