using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ComboOptionKey
    {
            public string Name { get; }
            public string Key{ get; }
            public ComboOptionKey(string name, string key)
            {
                Name = name;
                Key = key;
            }
    }
    public class ProfileViewModel : ViewModelBase
    {
        private readonly AuthStore _authStore;

        private UserModel test_;
        private Window _popupWindow;
        private NpgsqlConnection _connection;
        public UserModel Test { get { return test_; } set => test_ = value; }

        // untuk select
        private BukuModel _selectedBook;
        public BukuModel SelectedBook { get => _selectedBook; set => _selectedBook = value; }
        public bool CanEditOrDelete => _selectedBook != null;
        public bool isAddBuku = false;

        public ICommand editButtonCommand { get; }
        public ICommand SaveProfileCommand { get; }

        public ObservableCollection<ComboOptionKey> StatusBukuCombo { get; set; } = new ObservableCollection<ComboOptionKey>() { new ComboOptionKey("Bisa ditukar", "OPEN_FOR_TUKAR"), new ComboOptionKey("Hanya koleksi", "KOLEKSI") };
        public ComboOptionKey SelectedComboStatus { get; set; }

        public ProfileViewModel(AuthStore authStore)
        {
            _authStore = authStore;
            test_ = authStore.UserLoggedIn.ShallowCopy();

            editButtonCommand = new ProfileCommand(ubahNama);
            SaveProfileCommand = new SaveEditProfile(ubahNama);

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


    }
}
