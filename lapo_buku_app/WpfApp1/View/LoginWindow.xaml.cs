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
using WpfApp1.View.MainApp.Browsing;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AuthManager _authManager;

        public LoginWindow()
        {
            InitializeComponent();
            _authManager = new AuthManager();
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
