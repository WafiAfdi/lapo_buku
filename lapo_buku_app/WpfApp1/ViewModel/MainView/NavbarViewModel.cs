using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Service;
using WpfApp1.Store;

namespace WpfApp1.ViewModel.MainView
{
    public class NavbarViewModel : ViewModelBase
    {
        private readonly AuthStore _authStore;

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateProfileCommand { get; }
        public ICommand NavigateTransaksiCommand { get; }
        public ICommand LogoutCommand {  get; }

        public NavbarViewModel(
            INavigationService<BrowsingViewModel> browserNavigationService,
            INavigationService<ProfileViewModel> profileNavigationService,
            INavigationService<TransaksiViewModel> transaksiNavigationService,
            Action displayLogout,
            AuthStore authStore
        ) 
        {
            NavigateHomeCommand = new NavigateCommand<BrowsingViewModel>(browserNavigationService);
            NavigateProfileCommand = new NavigateCommand<ProfileViewModel>(profileNavigationService);  
            NavigateTransaksiCommand = new NavigateCommand<TransaksiViewModel>(transaksiNavigationService);
            _authStore = authStore;
            LogoutCommand = new LogoutCommand(displayLogout, authStore);
        }

    }
}
