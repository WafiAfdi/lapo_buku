using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Models;
using WpfApp1.Store;
using WpfApp1.View.MainApp.Profile;

namespace WpfApp1.ViewModel.MainView
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly AuthStore _authStore;

        private UserModel test_;
        private Window _popupWindow;
        private NpgsqlConnection _connection;
        public UserModel Test { get { return test_; } set => test_ = value; }

        public ICommand editButtonCommand;

        public ProfileViewModel(AuthStore authStore)
        {
            _authStore = authStore;
            test_ = authStore.UserLoggedIn.ShallowCopy();
            editButtonCommand = new ProfileCommand(ubahNama);
            ConnectToDatabase();
        }

        public void ubahNama()
        {
            _popupWindow = new EditProfile()
            {
                DataContext = this
            };
            _popupWindow.ShowDialog();
        }

        private void ConnectToDatabase()
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST");
            string username = Environment.GetEnvironmentVariable("DB_USER");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            string database = Environment.GetEnvironmentVariable("DB_NAME");
            string port = Environment.GetEnvironmentVariable("DB_PORT");

            // Connection string
            string _connString = $"Host={host};Username={username};Password={password};Database={database};Port={port}";

            try
            {
                _connection = new NpgsqlConnection(_connString);
                _connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateProfile(int userId, string Deskripsi, string Kota, string Provinsi, string AlamatJalan)
        {
            string queryUpdateProfile = "UPDATE user SET deskripsi=@Deskripsi, kota=@Kota, provinsi=@Provinsi, alamat_jalan=@AlamatJalan WHERE id=@UserID;";

            var command = new NpgsqlCommand(queryUpdateProfile, _connection);

            command.Parameters.AddWithValue("@Deskripsi", test_.Deskripsi);
            command.Parameters.AddWithValue("@Kota", test_.Kota);
            command.Parameters.AddWithValue("@Provinsi", test_.Provinsi);
            command.Parameters.AddWithValue("@AlamatJalan", test_.AlamatJalan);
            command.Parameters.AddWithValue("@UserID", _authStore.UserLoggedIn.Id);

            command.ExecuteNonQuery();

            OnPropertyChanged(nameof(Test));
        }


    }
}
