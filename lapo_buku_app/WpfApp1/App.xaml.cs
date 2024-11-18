using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Commands;
using WpfApp1.Config;
using WpfApp1.Models;
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
        private readonly AuthStore _authStore;  
        private readonly Action _displayMainApp;
        private readonly DbConfig _dbConfig;
        public Action DisplayLogout;

        public App()
        {
            _navigationStore = new NavigationStore();
            _authStore = new AuthStore();
            _authStore.UserLoggedIn = new UserModel();
            _displayMainApp = DisplayMainAppFromLogin;
            _dbConfig= ConfigLoader.LoadConfig();

            _dbConfig.Host.ToString();


        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Env.Load();  // Loads .env file

            

            //MainWindow = new LoginWindow(_navigationStore, CreateNavBarViewModel);
            //WpfApp1.View.MainApp.MainWindow mainWindow = new WpfApp1.View.MainApp.MainWindow()
            //{
            //    DataContext = new MainViewModel(_navigationStore)
            //};
            //MainWindow = mainWindow;
            ////_navigationStore.CurrentViewModel = new BrowsingViewModel();
            //_navigationStore.CurrentViewModel = new LayoutViewModel(CreateNavBarViewModel(), new BrowsingViewModel(_navigationStore,CreateNavBarViewModel, _authStore));
            MainWindow = new LoginWindow(_displayMainApp, _authStore, _dbConfig);
            MainWindow.Show();
            base.OnStartup(e);
        }

        private INavigationService<BrowsingViewModel> CreateBrowsingNavService()
        {
            
            return new LayoutNavigationService<BrowsingViewModel>(_navigationStore, () => new BrowsingViewModel(_navigationStore, CreateNavBarViewModel, _authStore, _dbConfig), CreateNavBarViewModel);
        }

        private INavigationService<ProfileViewModel> CreateProfileNavigationService()
        {
            return new LayoutNavigationService<ProfileViewModel>(_navigationStore, () => new ProfileViewModel(_authStore, _dbConfig), CreateNavBarViewModel);

        }

        private INavigationService<TransaksiViewModel> CreateTransaksiNavigationService()
        {
            return new LayoutNavigationService<TransaksiViewModel>(_navigationStore, () => new TransaksiViewModel(), CreateNavBarViewModel);
        }

        private NavbarViewModel CreateNavBarViewModel()
        {
            return new NavbarViewModel(CreateBrowsingNavService(), CreateProfileNavigationService(), CreateTransaksiNavigationService(), DisplayLoginWindow, _authStore);
        }

        private void DisplayLoginWindow()
        {
            MainWindow = new LoginWindow(_displayMainApp, _authStore, _dbConfig);
            MainWindow.Show();
            DisplayLogout?.Invoke();
        }

        private void DisplayMainAppFromLogin()
        {
            WpfApp1.View.MainApp.MainWindow mainWindow = new WpfApp1.View.MainApp.MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            MainWindow = mainWindow;
            //_navigationStore.CurrentViewModel = new BrowsingViewModel();
            _navigationStore.CurrentViewModel = new LayoutViewModel(CreateNavBarViewModel(), new BrowsingViewModel(_navigationStore, CreateNavBarViewModel, _authStore, _dbConfig));
            MainWindow.Show();
        }
    }
}
