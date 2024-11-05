using DotNetEnv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Service;
using WpfApp1.Store;
using WpfApp1.View;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly NavigationStore _navigationStore;

        public App()
        {
            _navigationStore = new NavigationStore();


        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Env.Load();  // Loads .env file



            MainWindow = new LoginWindow(_navigationStore, CreateNavBarViewModel);
            _navigationStore.CurrentViewModel = new BrowsingViewModel();

            MainWindow.Show();
            base.OnStartup(e);
        }

        private INavigationService<BrowsingViewModel> CreateBrowsingNavService()
        {
            return new LayoutNavigationService<BrowsingViewModel>(_navigationStore, () => new BrowsingViewModel(), CreateNavBarViewModel);
        }

        private INavigationService<ProfileViewModel> CreateProfileNavigationService()
        {
            return new LayoutNavigationService<ProfileViewModel>(_navigationStore, () => new ProfileViewModel(), CreateNavBarViewModel);

        }

        private NavbarViewModel CreateNavBarViewModel()
        {
            return new NavbarViewModel(CreateBrowsingNavService(), CreateProfileNavigationService());
        }
    }
}
