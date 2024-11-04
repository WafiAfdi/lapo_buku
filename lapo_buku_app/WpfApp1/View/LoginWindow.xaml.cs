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
using WpfApp1.View.MainApp.Browsing;
using Npgsql;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AuthManager _authManager;
        private NpgsqlConnection _connection;

        public LoginWindow()
        {
            InitializeComponent();
            ConnectToDatabase();

            _authManager = new AuthManager(_connection);
        }

        private void ConnectToDatabase()
        {
            string host = "localhost";
            string username = "postgres";
            string password = "passwordsql";
            string database = "junpro";
            string port = "5432";

            // Connection string
            string connString = $"Host={host};Username={username};Password={password};Database={database};Port={port}";

            try
            {
                _connection = new NpgsqlConnection(connString);
                _connection.Open();
                MessageBox.Show("Database connected successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindos registerWindos = new RegisterWindos();
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

            BrowsingWindow browsingWindow = new BrowsingWindow();
            browsingWindow.Show();

            this.Close();
        }
    }
}
