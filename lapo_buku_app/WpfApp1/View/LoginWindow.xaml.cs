using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Store;
using WpfApp1.View.MainApp;
using WpfApp1.View.MainApp.Browsing;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AuthManager _authManager;
        private NavigationStore _navigationStore;
        private Func<NavbarViewModel> _createNavbarViewModel;

        public LoginWindow(NavigationStore navigationStore, Func<NavbarViewModel> createNavbarViewModel)
        {
            InitializeComponent();
            _authManager = new AuthManager();
            _navigationStore = navigationStore;
            _createNavbarViewModel = createNavbarViewModel;
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindos registerWindos = new RegisterWindos(_navigationStore, _createNavbarViewModel);
            Application.Current.MainWindow = registerWindos;
            registerWindos.Show();

            this.Close();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            _authManager.Login(emailLabel.Text, passwordLabel.Text);

            if (!_authManager.isLoggedIn)
            {
                MessageBox.Show("Invalid input");
                return;
            }

            _navigationStore.CurrentViewModel = new LayoutViewModel(_createNavbarViewModel(), new BrowsingViewModel());

            WpfApp1.View.MainApp.MainWindow mainWindow = new WpfApp1.View.MainApp.MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();


            this.Close();
        }

        ~LoginWindow()
        {

        }
    }
}
