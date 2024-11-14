using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
using WpfApp1.Service;
using WpfApp1.Store;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for RegisterWindos.xaml
    /// </summary>
    public partial class RegisterWindos : Window
    {
        private readonly Action DisplayMainApp;
        private readonly AuthStore _authStore;
        private readonly NavigationStore _navigationStore;


        private IAuthManager _authManager;
        private Func<NavbarViewModel> _createNavbarViewModel;
        private NpgsqlConnection _connection;
        public RegisterWindos(NavigationStore navigationStore, Func<NavbarViewModel> createNavbarViewModel)
        {
            InitializeComponent();
            _authManager = new WpfApp1.Service.AuthManager();
            _navigationStore = navigationStore;
            _createNavbarViewModel = createNavbarViewModel;
        }

        public RegisterWindos(Action displayMainApp, AuthStore authStore)
        {
            InitializeComponent();
            DisplayMainApp = displayMainApp;
            ConnectToDatabase();
            _authStore = authStore;
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            //
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string email = EmailTextBox.Text;
            string reenterPassword = ReenterPasswordBox.Password;

            if(password != reenterPassword)
            {
                MessageBox.Show("Password dan konfirmasi password tidak sama", "Gagal Registrasi", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            try
            {
                bool isRegistered = _authManager.Register(username, password, email);

                if (isRegistered)
                {
                    MessageBox.Show("Registrasi berhasil", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoginWindow login = new LoginWindow(DisplayMainApp, _authStore);
                    login.Show();

                    this.Close();
                }
                else
                {
                    //MessageBox.Show("Username sudah dipakai", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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
                _authManager = new WpfApp1.Service.AuthManager(connString);
                MessageBox.Show("Database connected successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
