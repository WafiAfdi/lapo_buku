using System;
using System.Collections.Generic;
using System.Data.Common;
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
using Npgsql;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly Action DisplayMainApp;
        private readonly AuthStore _authStore;

        private AuthManager _authManager;
        private NpgsqlConnection _connection;
        private NavigationStore _navigationStore;
        private Func<NavbarViewModel> _createNavbarViewModel;

        public LoginWindow(NavigationStore navigationStore, Func<NavbarViewModel> createNavbarViewModel)
        {
            InitializeComponent();
            ConnectToDatabase();
            _authManager = new AuthManager(_connection, _authStore);
            _navigationStore = navigationStore;
            _createNavbarViewModel = createNavbarViewModel;
            

        }

        public LoginWindow(Action displayMainApp, AuthStore authStore)
        {
            InitializeComponent();
            ConnectToDatabase();
            _authManager = new AuthManager(_connection, authStore);
            DisplayMainApp = displayMainApp;
            _authStore = authStore;
        }

        private void ConnectToDatabase()
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST");
            string username = Environment.GetEnvironmentVariable("DB_USER");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            string database = Environment.GetEnvironmentVariable("DB_NAME");
            string port = Environment.GetEnvironmentVariable("DB_PORT");

            // Connection string
            string connString = $"Host={host};Username={username};Password={password};Database={database};Port={port}";

            try
            {
                _connection = new NpgsqlConnection(connString);
                _connection.Open();
                //MessageBox.Show("Database connected successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindos registerWindos = new RegisterWindos(DisplayMainApp, _authStore);
            Application.Current.MainWindow = registerWindos;
            registerWindos.Show();

            this.Close();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            _authManager.Login(emailLabel.Text, passwordLabel.Password);

            if (!_authManager.isLoggedIn)
            {
                MessageBox.Show("Login Gagal");
                return;
            }

            DisplayMainApp();

            _connection.Close();


            this.Close();
        }

        ~LoginWindow()
        {

        }
    }
}
