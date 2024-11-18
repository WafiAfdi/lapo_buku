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
using WpfApp1.Config;
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
        private readonly DbConfig _dbConfig;

        private UserModel test_;
        private Window _popupWindow;
        private NpgsqlConnection _connection;
        public UserModel Test { get { return test_; } set => test_ = value; }

        // untuk select
        private BukuModel _selectedBook;
        public BukuModel SelectedBook { get => _selectedBook; 
            set {
                _selectedBook = value;
                
                OnPropertyChanged(nameof(CanEditOrDelete));

            } 
        }
        public bool CanEditOrDelete => _selectedBook != null;
        public bool IsAddBuku { get; set; }
        public bool IsEditBuku { get => !IsAddBuku; }
        private BukuModel _newBuku;
        public BukuModel NewBuku { get => _newBuku; set => _newBuku = value; }

        public ICommand editButtonCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand AddBukuCommand { get; }
        public ICommand EditBukuCommand{ get; }
        public ICommand DeleteBukuCommand { get; }

        public ObservableCollection<ComboOptionKey> StatusBukuCombo { get; set; }
        public ComboOptionKey SelectedComboStatus { get; set; }

        public ObservableCollection<BukuModel> Books { get; set; } = new ObservableCollection<BukuModel>();

        public ProfileViewModel(AuthStore authStore, DbConfig dbConfig)
        {
            _authStore = authStore;
            test_ = authStore.UserLoggedIn.ShallowCopy();

            editButtonCommand = new ProfileCommand(ubahNama);
            SaveProfileCommand = new SaveEditProfile(UpdateProfile);
            AddBukuCommand = new AddBukuCommand(AddNewBuku);
            EditBukuCommand = new EditBukuCommand(EditBuku);
            DeleteBukuCommand = new DeleteBukuCommand(deleteSelectedBook);

            StatusBukuCombo = new ObservableCollection<ComboOptionKey>() { new ComboOptionKey("Bisa ditukar", "OPEN_FOR_TUKAR"), new ComboOptionKey("Hanya koleksi", "KOLEKSI") };
            _dbConfig = dbConfig;
            ConnectToDatabase();
            GetUserInformation();
            FetchBooksByUserID();
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
            string host = _dbConfig.Host;
            string username = _dbConfig.User;
            string password = _dbConfig.Password;
            string database = _dbConfig.Name;
            string port = _dbConfig.Port.ToString();

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

        public void AddNewBuku()
        {
            using (var transaction = _connection.BeginTransaction()) 
            try
            {
                // Insert the book and get the generated ID
                var insertBookQuery = @"
                INSERT INTO buku (id_pemilik, isbn, judul, penerbit, deskripsi, tahun_terbit, rating_buku, status)
                VALUES (@idPemilik, @isbn, @judul, @penerbit, @deskripsi, @tahunTerbit, @ratingBuku, @statusBuku::status_buku)
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
                        getGenreCommand.Parameters.AddWithValue("nama", genre.Trim());
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
                            getWriterCommand.Parameters.AddWithValue("nama", writer.Trim());
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
                Books.Add(_newBuku);
                MessageBox.Show(
                     "Buku berhasil ditambah"
                 );
                _popupWindow.Close();
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
            OnPropertyChanged(nameof(IsEditBuku));
            OnPropertyChanged(nameof(IsAddBuku));
            _popupWindow.ShowDialog();

        }

        public void ShowEditWindow()
        {
            IsAddBuku = false;
            _newBuku = _selectedBook.Clone();
            if (_newBuku.status_Buku == status_buku.KOLEKSI)
            {
                SelectedComboStatus = StatusBukuCombo[1];
            } else
            {
                SelectedComboStatus = StatusBukuCombo[0];
            }
            _popupWindow = new AddEditBook()
            {
                DataContext = this,
                ResizeMode = ResizeMode.NoResize,
                WindowState = WindowState.Normal,
            };
            OnPropertyChanged(nameof(IsEditBuku));
            OnPropertyChanged(nameof(IsAddBuku));
            _popupWindow.ShowDialog();

        }

        private void EditBuku()
        {
            using (var transaction = _connection.BeginTransaction())
                try
                {
                    // Insert the book and get the generated ID
                    var updateBookQuery = @"
                        UPDATE buku
                        SET id_pemilik = @idPemilik,
                            isbn = @isbn,
                            judul = @judul,
                            penerbit = @penerbit,
                            deskripsi = @deskripsi,
                            tahun_terbit = @tahunTerbit,
                            rating_buku = @ratingBuku,
                            status = @statusBuku::status_buku,
                            last_updated = NOW()
                        WHERE id = @idBuku;";
                    int bookId = _newBuku.BukuID;

                    using (var command = new NpgsqlCommand(updateBookQuery, _connection))
                    {
                        command.Parameters.AddWithValue("idPemilik", _authStore.UserLoggedIn.Id);
                        command.Parameters.AddWithValue("isbn", _newBuku.ISBN);
                        command.Parameters.AddWithValue("judul", _newBuku.Judul);
                        command.Parameters.AddWithValue("penerbit", _newBuku.Penerbit);
                        command.Parameters.AddWithValue("deskripsi", _newBuku.Deskripsi ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("tahunTerbit", _newBuku.Terbit);
                        command.Parameters.AddWithValue("ratingBuku", _newBuku.RatingPemilik);
                        command.Parameters.AddWithValue("statusBuku", SelectedComboStatus.Key);
                        command.Parameters.AddWithValue("idBuku", _newBuku.BukuID);

                        command.ExecuteNonQuery();
                    }

                    // Delete existing relationships in genre_buku and buku_ditulis
                    var deleteGenreBookQuery = "DELETE FROM genre_buku WHERE id_buku = @idBuku;";
                    var deleteBookWriterQuery = "DELETE FROM buku_ditulis WHERE id_buku = @idBuku;";

                    using (var deleteCommand = new NpgsqlCommand(deleteGenreBookQuery, _connection))
                    {
                        deleteCommand.Parameters.AddWithValue("idBuku", bookId);
                        deleteCommand.ExecuteNonQuery();
                    }

                    using (var deleteCommand = new NpgsqlCommand(deleteBookWriterQuery, _connection))
                    {
                        deleteCommand.Parameters.AddWithValue("idBuku", bookId);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Re-insert genres
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

                        using (var getGenreCommand = new NpgsqlCommand(getGenreIdQuery, _connection))
                        {
                            getGenreCommand.Parameters.AddWithValue("nama", genre.Trim());
                            var result = getGenreCommand.ExecuteScalar();
                            if (result != null)
                            {
                                genreId = (int)result;
                            }
                            else
                            {
                                using (var insertGenreCommand = new NpgsqlCommand(insertGenreQuery, _connection))
                                {
                                    insertGenreCommand.Parameters.AddWithValue("nama", genre);
                                    genreId = (int)insertGenreCommand.ExecuteScalar();
                                }
                            }
                        }

                        using (var insertGenreBookCommand = new NpgsqlCommand(insertGenreBookQuery, _connection))
                        {
                            insertGenreBookCommand.Parameters.AddWithValue("idBuku", bookId);
                            insertGenreBookCommand.Parameters.AddWithValue("idGenre", genreId);
                            insertGenreBookCommand.ExecuteNonQuery();
                        }
                    }

                    // Re-insert writers
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

                        using (var getWriterCommand = new NpgsqlCommand(getWriterIdQuery, _connection))
                        {
                            getWriterCommand.Parameters.AddWithValue("nama", writer.Trim());
                            var result = getWriterCommand.ExecuteScalar();
                            if (result != null)
                            {
                                writerId = (int)result;
                            }
                            else
                            {
                                using (var insertWriterCommand = new NpgsqlCommand(insertWriterQuery, _connection))
                                {
                                    insertWriterCommand.Parameters.AddWithValue("nama", writer);
                                    writerId = (int)insertWriterCommand.ExecuteScalar();
                                }
                            }
                        }

                        using (var insertBookWriterCommand = new NpgsqlCommand(insertBookWriterQuery, _connection))
                        {
                            insertBookWriterCommand.Parameters.AddWithValue("idBuku", bookId);
                            insertBookWriterCommand.Parameters.AddWithValue("idPenulis", writerId);
                            insertBookWriterCommand.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                    Books.Remove(_selectedBook);
                    Books.Add(_newBuku.Clone());
                    MessageBox.Show(
                         "Buku berhasil diubah"
                     );
                    _popupWindow.Close();
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

        public string AlamatLengkap
        {
            get => $"{Test.Kota}, {Test.Provinsi}, {Test.AlamatJalan}";
        }


        public void FetchBooksByUserID()
        {

            
                try
                {
                    string GetBookByUserID = @"
                    WITH BookData AS (
                    SELECT b.id, b.isbn, b.judul, b.penerbit, b.deskripsi, b.tahun_terbit, b.status, b.created, b.rating_buku,
                           COALESCE(string_agg(DISTINCT p.nama, ', '), 'Unknown') AS writers,
                           COALESCE(string_agg(DISTINCT g.nama, ', '), 'None') AS genres,
                            pemilik.id AS id_pemilik, pemilik.username, pemilik.email, pemilik.kota, pemilik.provinsi , pemilik.deskripsi AS deskripsi_diri
                    FROM buku b
                    LEFT JOIN buku_ditulis bd ON b.id = bd.id_buku
                    LEFT JOIN penulis p ON bd.id_penulis = p.id
                    LEFT JOIN genre_buku gb ON b.id = gb.id_buku
                    LEFT JOIN genre g ON gb.id_genre = g.id
                    LEFT JOIN public.user pemilik ON pemilik.id = b.id_pemilik
                    WHERE pemilik.id = @UserID
                    GROUP BY b.id, pemilik.id
                    )
                    SELECT * FROM BookData
                    ORDER BY id";

                    var command = new NpgsqlCommand(GetBookByUserID, _connection);
                    command.Parameters.AddWithValue("@UserID", _authStore.UserLoggedIn.Id);

                    using (var reader = command.ExecuteReader())
                    {
                        Books.Clear();
                        while (reader.Read())
                        {
                            Books.Add(new BukuModel
                            {
                                BukuID = reader.GetInt32(reader.GetOrdinal("id")),
                                ISBN = reader.GetString(reader.GetOrdinal("isbn")),
                                Judul = reader.GetString(reader.GetOrdinal("judul")),
                                GenreKomaKotor = reader.GetString(reader.GetOrdinal("genres")),
                                PengarangKomaKotor = reader.GetString(reader.GetOrdinal("writers")),
                                Penerbit = reader.GetString(reader.GetOrdinal("penerbit")),
                                Terbit = reader.IsDBNull(reader.GetOrdinal("tahun_terbit")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("tahun_terbit")),
                                RatingPemilik = reader.GetInt32(reader.GetOrdinal("rating_buku")),
                                DimilikiSejak = reader.GetDateTime(reader.GetOrdinal("created")),
                                status_Buku = reader.GetString(reader.GetOrdinal("status")) == "KOLEKSI" ? status_buku.KOLEKSI : status_buku.OPEN_FOR_TUKAR,
                                Deskripsi = reader.IsDBNull(reader.GetOrdinal("deskripsi")) ? "" : reader.GetString(reader.GetOrdinal("deskripsi"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching books: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }                     
        }

        public void deleteSelectedBook()
        {
            if (_selectedBook == null)
            {
                MessageBox.Show("Tidak ada buku yang dipilih untuk dihapus.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
            $"Apakah Anda yakin ingin menghapus buku \"{_selectedBook.Judul}\"?",
            "Konfirmasi Penghapusan",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
             );

            if (result != MessageBoxResult.Yes)
            {
                return; // Batalkan jika pengguna memilih "No"
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Hapus hubungan genre_buku
                    var deleteGenreBookQuery = "DELETE FROM genre_buku WHERE id_buku = @idBuku;";
                    using (var deleteGenreCommand = new NpgsqlCommand(deleteGenreBookQuery, _connection))
                    {
                        deleteGenreCommand.Parameters.AddWithValue("idBuku", _selectedBook.BukuID);
                        deleteGenreCommand.ExecuteNonQuery();
                    }

                    // Hapus hubungan buku_ditulis
                    var deleteBookWriterQuery = "DELETE FROM buku_ditulis WHERE id_buku = @idBuku;";
                    using (var deleteWriterCommand = new NpgsqlCommand(deleteBookWriterQuery, _connection))
                    {
                        deleteWriterCommand.Parameters.AddWithValue("idBuku", _selectedBook.BukuID);
                        deleteWriterCommand.ExecuteNonQuery();
                    }

                    // Hapus buku dari tabel utama
                    var deleteBookQuery = "DELETE FROM buku WHERE id = @idBuku;";
                    using (var deleteBookCommand = new NpgsqlCommand(deleteBookQuery, _connection))
                    {
                        deleteBookCommand.Parameters.AddWithValue("idBuku", _selectedBook.BukuID);
                        deleteBookCommand.ExecuteNonQuery();
                    }

                    // Commit transaksi
                    transaction.Commit();

                    // Perbarui koleksi buku di memori
                    Books.Remove(_selectedBook);
                    MessageBox.Show("Buku berhasil dihapus.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reset selected book
                    SelectedBook = null;
                }
                catch (Exception ex)
                {
                    // Rollback transaksi jika terjadi error
                    transaction.Rollback();
                    MessageBox.Show(
                        $"Terjadi kesalahan saat menghapus buku: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }


    }
}
