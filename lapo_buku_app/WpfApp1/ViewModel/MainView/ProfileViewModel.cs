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
            SaveProfileCommand = new SaveEditProfile(UpdateProfile);

            ConnectToDatabase();
            GetUserInformation();
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

        public void GetUserInformation()
        {
            try
            {
                string queryGetUserInformation = "SELECT deskripsi, kota, provinsi, nomor_kontak, alamat_jalan FROM public.user WHERE id=@UserID;";

                var command = new NpgsqlCommand(queryGetUserInformation, _connection);

                // Menambahkan parameter untuk UserID
                command.Parameters.AddWithValue("@UserID", _authStore.UserLoggedIn.Id);

                // Menjalankan query dan mengambil data
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Menyimpan hasil query ke dalam properti atau model
                        Test.Deskripsi = reader.IsDBNull(reader.GetOrdinal("deskripsi")) ? "Belum ada deskripsi" : reader.GetString(reader.GetOrdinal("deskripsi"));
                        Test.Kota = reader.IsDBNull(reader.GetOrdinal("kota")) ? "Belum ada deskripsi" : reader.GetString(reader.GetOrdinal("kota"));
                        Test.Provinsi = reader.IsDBNull(reader.GetOrdinal("provinsi")) ? "Belum ada deskripsi" : reader.GetString(reader.GetOrdinal("provinsi"));
                        Test.Nomor_Kontak = reader.IsDBNull(reader.GetOrdinal("nomor_kontak")) ? "Belum ada deskripsi" : reader.GetString(reader.GetOrdinal("nomor_kontak"));
                        Test.AlamatJalan = reader.IsDBNull(reader.GetOrdinal("alamat_jalan")) ? "Belum ada deskripsi" : reader.GetString(reader.GetOrdinal("alamat_jalan"));
                    }
                    else
                    {
                        MessageBox.Show("Data tidak ditemukan.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                OnPropertyChanged(nameof(Test));
            }
            catch (Exception ex)
            {
                // Menampilkan pesan error
                MessageBox.Show(
                    $"Terjadi kesalahan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateProfile()
        {
            try
            {
                string queryUpdateProfile = "UPDATE public.user SET deskripsi=@Deskripsi, kota=@Kota, provinsi=@Provinsi, alamat_jalan=@AlamatJalan, nomor_kontak=@NomorKontak WHERE id=@UserID;";

                var command = new NpgsqlCommand(queryUpdateProfile, _connection);

                command.Parameters.AddWithValue("@Deskripsi", test_.Deskripsi);
                command.Parameters.AddWithValue("@Kota", test_.Kota);
                command.Parameters.AddWithValue("@Provinsi", test_.Provinsi);
                command.Parameters.AddWithValue("@AlamatJalan", test_.AlamatJalan);
                command.Parameters.AddWithValue("@UserID", _authStore.UserLoggedIn.Id);
                command.Parameters.AddWithValue("@NomorKontak", test_.Nomor_Kontak);

                command.ExecuteNonQuery();

                OnPropertyChanged(nameof(Test));


                _popupWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Telah terjadi error : ${ex.Message}",          // The message to display
                    "Error",               // The title of the message box
                    MessageBoxButton.OK,   // The buttons to include (OK in this case)
                    MessageBoxImage.Error  // The icon to display (Error icon)
                );
            }
            
        }

        public string AlamatLengkap
        {
            get => $"{Test.Kota}, {Test.Provinsi}, {Test.AlamatJalan}";
        }

    }
}
