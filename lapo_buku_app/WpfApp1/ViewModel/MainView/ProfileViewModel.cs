using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
        public bool IsAddBuku { get; set; }
        public bool IsEditBuku { get => !IsAddBuku; }
        private BukuModel _newBuku;

        public ICommand editButtonCommand { get; }
        public ICommand SaveProfileCommand { get; }

        public ObservableCollection<ComboOptionKey> StatusBukuCombo { get; set; }
        public ComboOptionKey SelectedComboStatus { get; set; }

        public ProfileViewModel(AuthStore authStore)
        {
            _authStore = authStore;
            test_ = authStore.UserLoggedIn.ShallowCopy();

            editButtonCommand = new ProfileCommand(ubahNama);
            SaveProfileCommand = new SaveEditProfile(UpdateProfile);

            StatusBukuCombo = new ObservableCollection<ComboOptionKey>() { new ComboOptionKey("Bisa ditukar", "OPEN_FOR_TUKAR"), new ComboOptionKey("Hanya koleksi", "KOLEKSI") };

            ConnectToDatabase();
            GetUserInformation();
        }

        public void ubahNama()
        {
            _popupWindow = new EditProfile()
            {
                DataContext = this,
                WindowState = WindowState.Normal,
                ResizeMode = ResizeMode.NoResize,

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

        private void AddNewBuku()
        {
            using (var transaction = _connection.BeginTransaction()) 
            try
            {
                // Insert the book and get the generated ID
                var insertBookQuery = @"
                INSERT INTO buku (id_pemilik, isbn, judul, penerbit, deskripsi, tahun_terbit, rating_buku, status)
                VALUES (@idPemilik, @isbn, @judul, @penerbit, @deskripsi, @tahunTerbit, @ratingBuku, @statusBuku)
                RETURNING id;";
                int bookId;

                using (var command = new NpgsqlCommand(insertBookQuery, _connection))
                {
                    command.Parameters.AddWithValue("idPemilik", _authStore.UserLoggedIn.Id);
                    command.Parameters.AddWithValue("isbn", _newBuku.ISBN);
                    command.Parameters.AddWithValue("judul", _newBuku.Judul);
                    command.Parameters.AddWithValue("penerbit", _newBuku.Penerbit);
                    command.Parameters.AddWithValue("deskripsi", _newBuku.Deskripsi ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("tahunTerbit", _newBuku.Terbit);
                    command.Parameters.AddWithValue("ratingBuku", _newBuku.RatingPemilik);
                    command.Parameters.AddWithValue("statusBuku", SelectedComboStatus.Key);

                    bookId = (int)command.ExecuteScalar();
                }

                // Prepare queries for genre and genre_buku
                var getGenreIdQuery = "SELECT id FROM genre WHERE nama = @nama;";
                var insertGenreQuery = "INSERT INTO genre (nama) VALUES (@nama) RETURNING id;";
                var insertGenreBookQuery = @"
                INSERT INTO genre_buku (id_buku, id_genre)
                VALUES (@idBuku, @idGenre)
                ON CONFLICT DO NOTHING;";

                List<string> genres = _newBuku.GenreKomaKotor.Split(',').ToList();

                foreach (var genre in genres)
                {
                    int genreId;

                    // Check if genre exists
                    using (var getGenreCommand = new NpgsqlCommand(getGenreIdQuery, _connection))
                    {
                        getGenreCommand.Parameters.AddWithValue("nama", genre);
                        var result = getGenreCommand.ExecuteScalar();
                        if (result != null)
                        {
                            genreId = (int)result;
                        }
                        else
                        {
                            // Insert new genre
                            using (var insertGenreCommand = new NpgsqlCommand(insertGenreQuery, _connection))
                            {
                                insertGenreCommand.Parameters.AddWithValue("nama", genre);
                                genreId = (int)insertGenreCommand.ExecuteScalar();
                            }
                        }
                    }

                    // Insert into genre_buku
                    using (var insertGenreBookCommand = new NpgsqlCommand(insertGenreBookQuery, _connection))
                    {
                        insertGenreBookCommand.Parameters.AddWithValue("idBuku", bookId);
                        insertGenreBookCommand.Parameters.AddWithValue("idGenre", genreId);
                        insertGenreBookCommand.ExecuteNonQuery();
                    }

                    var getWriterIdQuery = "SELECT id FROM penulis WHERE nama = @nama;";
                    var insertWriterQuery = "INSERT INTO penulis (nama) VALUES (@nama) RETURNING id;";
                    var insertBookWriterQuery = @"
                    INSERT INTO buku_ditulis (id_buku, id_penulis)
                    VALUES (@idBuku, @idPenulis)
                    ON CONFLICT DO NOTHING;";

                    List<string> writers = _newBuku.PengarangKomaKotor.Split(',').ToList();

                    foreach (var writer in writers)
                    {
                        int writerId;

                        // Check if the writer already exists
                        using (var getWriterCommand = new NpgsqlCommand(getWriterIdQuery, _connection))
                        {
                            getWriterCommand.Parameters.AddWithValue("nama", writer);
                            var result = getWriterCommand.ExecuteScalar();
                            if (result != null)
                            {
                                // If exists, get the writer ID
                                writerId = (int)result;
                            }
                            else
                            {
                                // If not, insert the writer and get the new ID
                                using (var insertWriterCommand = new NpgsqlCommand(insertWriterQuery, _connection))
                                {
                                    insertWriterCommand.Parameters.AddWithValue("nama", writer);
                                    writerId = (int)insertWriterCommand.ExecuteScalar();
                                }
                            }
                        }

                        // Insert the relationship into buku_ditulis
                        using (var insertBookWriterCommand = new NpgsqlCommand(insertBookWriterQuery, _connection))
                        {
                            insertBookWriterCommand.Parameters.AddWithValue("idBuku", bookId);
                            insertBookWriterCommand.Parameters.AddWithValue("idPenulis", writerId);
                            insertBookWriterCommand.ExecuteNonQuery();
                        }
                    }
                }

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex) 
            {
                // Rollback the transaction on error
                transaction.Rollback();
                MessageBox.Show(
                    $"Telah terjadi error : ${ex.Message}",          // The message to display
                    "Error",               // The title of the message box
                    MessageBoxButton.OK,   // The buttons to include (OK in this case)
                    MessageBoxImage.Error  // The icon to display (Error icon)
                );
            }
        }

        public void ShowAddWindow()
        {
            IsAddBuku = true;
            _newBuku = new BukuModel();
            _popupWindow = new AddEditBook() { 
                DataContext = this,
                ResizeMode = ResizeMode.NoResize,
                WindowState = WindowState.Normal,
            };
            _popupWindow.ShowDialog();

        }

        private void EditBuku()
        {

        }

        public string AlamatLengkap
        {
            get => $"{Test.Kota}, {Test.Provinsi}, {Test.AlamatJalan}";
        }

    }
}
