using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.View.MainApp;
using WpfApp1.Models;
using System.Windows.Input;
using WpfApp1.Store;
using Npgsql;
using System.Data.Common;
using WpfApp1.Config;

namespace WpfApp1.ViewModel.MainView
{
    public class TransaksiViewModel : ViewModelBase
    {
        private readonly AuthStore _authStore;
        private readonly DbConfig _dbConfig;

        private NpgsqlConnection _connection;

        public ObservableCollection<TransaksiModel> TransaksiList { get; set; } = new ObservableCollection<TransaksiModel>();

        public TransaksiModel SelectedTransaksi { get; set; }
        public void Popup()
        {
            Window popup = new PopupTransaksi();
            PopupTransaksiViewModel popupTransaksiViewModel = new PopupTransaksiViewModel(SelectedTransaksi, _authStore, _connection, popup);
            popup.DataContext = popupTransaksiViewModel;
            popup.ShowDialog();
        }

        public ICommand SearchCommand;

        public string SearchQuery
        { get; set; }


        public TransaksiViewModel(AuthStore authStore, DbConfig dbConfig)
        {
            _authStore = authStore;
            _dbConfig = dbConfig;

            ConnectToDatabase();

            LoadData();
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

        public void LoadData()
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST");
            string username = Environment.GetEnvironmentVariable("DB_USER");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            string database = Environment.GetEnvironmentVariable("DB_NAME");
            string port = Environment.GetEnvironmentVariable("DB_PORT");

            // Connection string
            string _connString = $"Host={_dbConfig.Host};Username={_dbConfig.User};Password={_dbConfig.Password};Database={_dbConfig.Name};Port={_dbConfig.Port.ToString()}";

            using (var connection = new NpgsqlConnection(_connString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        tp.id AS transaksi_id,
                        tp.status AS transaksi_status,
                        tp.created AS transaksi_waktu,
                        tp.is_penjual_konfirmasi AS konfirmasi_penjual,
                        tp.is_pembeli_konfirmasi AS konfirmasi_pembeli,
                        tp.is_penjual_menerima AS menerima_penjual,
                        tp.is_pembeli_menerima AS menerima_pembeli,
                        b1.id AS buku_penawar_id,
                        b1.isbn AS buku_penawar_isbn,
                        b1.judul AS buku_penawar_judul,
                        b1.deskripsi AS buku_penawar_deskripsi,
                        b1.tahun_terbit AS buku_penawar_tahun,
                        b1.penerbit AS buku_penawar_penerbit,
                        b2.id AS buku_penerima_id,
                        b2.isbn AS buku_penerima_isbn,
                        b2.judul AS buku_penerima_judul,
                        b2.deskripsi AS buku_penerima_deskripsi,
                        b2.tahun_terbit AS buku_penerima_tahun,
                        b2.penerbit AS buku_penerima_penerbit,
                        u1.id AS user_penawar_id,
                        u1.username AS user_penawar_username,
                        u2.id AS user_penerima_id,
                        u2.username AS user_penerima_username
                    FROM transaksi_penukaran tp
                    JOIN buku b1 ON tp.buku_penjual = b1.id
                    JOIN buku b2 ON tp.buku_pembeli = b2.id
                    JOIN public.user u1 ON b1.id_pemilik = u1.id
                    JOIN public.user u2 ON b2.id_pemilik = u2.id
                    WHERE u1.username = @Username OR u2.username = @Username"
                    ;

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", _authStore.UserLoggedIn.Username);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bukuPenawar = new BukuModel
                            {
                                BukuID = reader.GetInt32(reader.GetOrdinal("buku_penawar_id")),
                                ISBN = reader.GetString(reader.GetOrdinal("buku_penawar_isbn")),
                                Judul = reader.GetString(reader.GetOrdinal("buku_penawar_judul")),
                                Penerbit = reader.GetString(reader.GetOrdinal("buku_penawar_penerbit")),
                                Deskripsi = reader.GetString(reader.GetOrdinal("buku_penawar_deskripsi")),
                                Terbit = reader.GetInt32(reader.GetOrdinal("buku_penawar_tahun")),
                                PemilikBuku = new UserModel
                                {
                                    Username = reader.GetString(reader.GetOrdinal("user_penawar_username")),
                                    Id = reader.GetInt32(reader.GetOrdinal("user_penawar_id"))
                                }
                            };

                            var bukuPenerima = new BukuModel
                            {
                                BukuID = reader.GetInt32(reader.GetOrdinal("buku_penerima_id")),
                                ISBN = reader.GetString(reader.GetOrdinal("buku_penerima_isbn")),
                                Judul = reader.GetString(reader.GetOrdinal("buku_penerima_judul")),
                                Penerbit = reader.GetString(reader.GetOrdinal("buku_penerima_penerbit")),
                                Deskripsi = reader.GetString(reader.GetOrdinal("buku_penerima_deskripsi")),
                                Terbit = reader.GetInt32(reader.GetOrdinal("buku_penerima_tahun")),
                                PemilikBuku = new UserModel
                                {
                                    Username = reader.GetString(reader.GetOrdinal("user_penerima_username")),
                                    Id = reader.GetInt32(reader.GetOrdinal("user_penerima_id"))
                                }
                            };

                            var transaksi = new TransaksiModel
                            {
                                IdTransaksi = reader.GetInt32(reader.GetOrdinal("transaksi_id")),
                                BukuPenawar = bukuPenawar,
                                BukuPenerima = bukuPenerima,
                                Status = reader.GetString(reader.GetOrdinal("transaksi_status")),
                                WaktuTransaksi = reader.GetDateTime(reader.GetOrdinal("transaksi_waktu")),
                                IsPembeliKonfirmasi = reader.GetBoolean(reader.GetOrdinal("konfirmasi_pembeli")),
                                IsPenjualKonfirmasi = reader.GetBoolean(reader.GetOrdinal("konfirmasi_penjual")),
                                IsPembeliTerima = reader.GetBoolean(reader.GetOrdinal("menerima_pembeli")),
                                IsPenjualTerima = reader.GetBoolean(reader.GetOrdinal("menerima_penjual"))
                            };

                            TransaksiList.Add(transaksi);
                        }
                    }
                }
            }
        }
    }
}
