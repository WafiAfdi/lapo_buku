using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Models;
using WpfApp1.Service;
using WpfApp1.Store;
using WpfApp1.View.Components.Browse;
using WpfApp1.ViewModel.ComponentsView.Browse;

namespace WpfApp1.ViewModel.MainView
{
    public class PageBukuViewModel : ViewModelBase
    {
        private readonly SearchQuery searchSebelumnya;
        private readonly INavigationService<BrowsingViewModel> _backToBrowseNavigationService;
        private readonly AuthStore _authStore;
        public ObservableCollection<BukuModel> BukuYangBisaDitawarkan { get; } = new ObservableCollection<BukuModel>(); // buku dari pemilik yang bisa ditukar
        
        private BukuModel _bukuYangTerpilih;
        private BukuModel ModelPageBuku; // data buku untuk page

        private NpgsqlConnection _connection;
        private string _connString;

        private bool _isShowAlasan = false;
        public bool IsShowAlasan
        {
            set
            {
                _isShowAlasan = value;
                OnPropertyChanged(nameof(IsShowAlasan)); // Notify UI of property change

            }
            get => _isShowAlasan;
        }

        private string _stringAlasan = "";
        public string StringAlasan
        {
            set 
            {
                _stringAlasan = value;
                OnPropertyChanged(nameof(StringAlasan)); // Notify UI of property change

            }
            get => _stringAlasan;
        }


        public BukuModel BukuYangTerpilih
        {
            get => _bukuYangTerpilih;
            set
            {
                _bukuYangTerpilih = value;
                OnPropertyChanged(nameof(BukuYangTerpilih)); // Notify UI of property change
                OnPropertyChanged(nameof(CanTukar)); // Notify UI of property change

            }
        }

        public ICommand BalikPageBrowse {  get; }
        public ICommand TukarBukuCommand { get; }

        public PageBukuViewModel(ParameterNavBuku parameter, INavigationService<BrowsingViewModel> BackToBrowseNavService, AuthStore authStore)
        {
            ModelPageBuku = parameter.buku;
            searchSebelumnya = parameter.query;

            _backToBrowseNavigationService = BackToBrowseNavService;
            _authStore = authStore;

            BalikPageBrowse = new PindahPageBrowse(BackToBrowseNavService);
            TukarBukuCommand = new TukarCommand(TukarBukuQuery);


            ConnectToDatabase();
            IsNotMyBookOrisNotInTransaction();
            GetUserBook();

        }

        public string Title => ModelPageBuku.Judul;
        public List<string> ListGenre => ModelPageBuku.Genre;
        public string Deskripsi => ModelPageBuku.Deskripsi;
        public string ListPenulis => string.Join(", ", ModelPageBuku.Pengarang.Select(item => item));
        public string TahunTerbit => ModelPageBuku.Terbit.ToString();
        public string Penerbit => ModelPageBuku.Penerbit;
        public string DateKepemilikan => ModelPageBuku.DimilikiSejak.ToString();
        public string ISBN => ModelPageBuku.ISBN;

        public string Username => ModelPageBuku.PemilikBuku.Nama;
        public string DeskripsiPemilik => ModelPageBuku.PemilikBuku.Deskripsi;
        public string Kota => ModelPageBuku.PemilikBuku.Kota;
        public string Provinsi => ModelPageBuku.PemilikBuku.Provinsi;
        public string RatingPemilik => ModelPageBuku.RatingPemilik.ToString();


        public bool CanTukar => _bukuYangTerpilih != null && ModelPageBuku.status_Buku == status_buku.OPEN_FOR_TUKAR && IsNotMyBookOrisNotInTransaction();


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

        private bool IsNotMyBookOrisNotInTransaction()
        {
            if (ModelPageBuku.status_Buku == status_buku.KOLEKSI)
            {
                StringAlasan = "Buku ini hanya tampilan koleksi";
                IsShowAlasan = true;
            } else {
                StringAlasan = "";
                IsShowAlasan = false;
            }


            if(_authStore.UserLoggedIn?.Id == ModelPageBuku.PemilikBuku.Id)
            {
                StringAlasan = "Buku ini punyamu";
                IsShowAlasan = true;
                return false;
            } else
            {
                StringAlasan = "";
                IsShowAlasan = false;
            }

            if (_bukuYangTerpilih == null)
            {
                StringAlasan = "Belum milih buku";
                IsShowAlasan = true;
                return false;
            } else
            {
                StringAlasan = "";
                IsShowAlasan = false;
            }

            string query = @"
                SELECT buku_penjual, buku_pembeli FROM public.transaksi_penukaran 
                WHERE (buku_penjual = @id OR buku_pembeli = @id) AND status = 'PROCESS';

            ";

            var command = new NpgsqlCommand(query, _connection);

            command.Parameters.AddWithValue("id", ModelPageBuku.BukuID);


            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                StringAlasan = "Buku ini sedang dalam proses transaksi";
                IsShowAlasan = true;
                return false;
            }

            reader.Close();

            return true;
        }

        private void GetUserBook()
        {
            string queryGetBuku = @"
                SELECT buku.id, buku.judul FROM public.buku WHERE id_pemilik = 1;
            "
            ;

            var command = new NpgsqlCommand(queryGetBuku, _connection);


            var reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    BukuYangBisaDitawarkan.Add(new BukuModel()
                    {
                        BukuID = reader.GetInt32(0),
                        Judul = reader.GetString(1),
                    });
                }
            }

            reader.Close();
        }

        private void TukarBukuQuery()
        {
            bool bukuPenjualMasihAda = false;
            bool bukuPembeliMasihAda = false;
            bool bukuBelumDiproses = false;
            bool bukuBelumAdaTransaksi = false;
            string queryGetBuku = @"
                SELECT id FROM public.buku WHERE id_pemilik = @id_pembeli AND id = @id_buku_pembeli;

                SELECT id FROM public.buku WHERE id_pemilik = @id_penjual AND id = @id_buku_penjual;

                SELECT buku_penjual, buku_pembeli FROM public.transaksi_penukaran 
                WHERE (
                    buku_penjual = @id_buku_penjual OR buku_penjual = @id_buku_pembeli
                    OR buku_pembeli = @id_buku_penjual OR buku_pembeli = @id_buku_pembeli
                ) 
                AND status = 'PROCESS';

                SELECT buku_penjual, buku_pembeli FROM public.transaksi_penukaran 
                WHERE (buku_penjual = @id_buku_penjual AND buku_pembeli = @id_buku_pembeli AND pembeli_id = @id_pembeli AND penjual_id = @id_penjual) 
                AND status != 'DONE';
                
                
            "
            ;

            string queryInsertTransaksi = @"INSERT INTO public.transaksi_penukaran (pembeli_id, penjual_id, buku_penjual, buku_pembeli)
                VALUES (@id_pembeli, @id_penjual, @id_buku_penjual, @id_buku_pembeli);";

            var command = new NpgsqlCommand(queryGetBuku, _connection);
            command.Parameters.AddWithValue("id_penjual", ModelPageBuku.PemilikBuku.Id);
            command.Parameters.AddWithValue("id_pembeli", _authStore.UserLoggedIn.Id);
            command.Parameters.AddWithValue("id_buku_pembeli", _bukuYangTerpilih.BukuID);
            command.Parameters.AddWithValue("id_buku_penjual", ModelPageBuku.BukuID);


            using (var reader = command.ExecuteReader()) 
            {

                while (reader.Read())
                {
                    // Cek jika masih ada
                    if (reader.HasRows)
                    {
                        bukuPembeliMasihAda = true;
                    }
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            bukuPenjualMasihAda = true;
                        }
                    }

                }

                if (reader.NextResult())
                {
                    if (reader.HasRows)
                    {

                    }
                    else
                    {
                        bukuBelumDiproses = true;
                    }


                }

                if (reader.NextResult())
                {
                    if (reader.HasRows)
                    {

                    }
                    else
                    {
                        bukuBelumAdaTransaksi = true;
                    }


                }

                reader.Close();
            };

            

            if(bukuPembeliMasihAda && bukuPenjualMasihAda && bukuBelumDiproses && bukuBelumAdaTransaksi)
            {
                // insert
                using (var command2 = new NpgsqlCommand(queryInsertTransaksi, _connection))
                {

                    command2.Parameters.AddWithValue("id_penjual", ModelPageBuku.PemilikBuku.Id);
                    command2.Parameters.AddWithValue("id_pembeli", _authStore.UserLoggedIn.Id);
                    command2.Parameters.AddWithValue("id_buku_pembeli", _bukuYangTerpilih.BukuID);
                    command2.Parameters.AddWithValue("id_buku_penjual", ModelPageBuku.BukuID);

                    command2.ExecuteNonQuery();
                }
                var itemToRemove = BukuYangBisaDitawarkan.FirstOrDefault(item => item.BukuID == _bukuYangTerpilih.BukuID);
                if (itemToRemove != null)
                {
                    BukuYangBisaDitawarkan.Remove(itemToRemove);
                }
                MessageBox.Show("Transaksi berhasil diproses, mohon menunggu pihak yang ditawari untuk menerima atau menolak tawaran");
                return;

            } else
            {
                if(!bukuBelumDiproses)
                {
                    MessageBox.Show("Buku ini lagi dalam proses pengiriman, tidak bisa ditukar");
                    return;
                } else if(!bukuBelumAdaTransaksi)
                {
                    MessageBox.Show("Buku ini sudah pernah diproseskan");
                    return;
                }
                MessageBox.Show("Telah terjadi kesalahan : Buku pihak penawar atau pihak penukar sudah tidak dapat diproses lagi");
                return;
            }
        }



    }
}
