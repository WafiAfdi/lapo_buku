using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

    public class SearchQuery
    {
        public string query = "";
        public int pageIndex = 1;
    }

    public class ParameterNavBuku
    {
        public SearchQuery query;
        public BukuModel buku;
    }
    public class BrowsingViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<NavbarViewModel> _createNavbarViewModel;
        private readonly AuthStore _authStore;


        private NpgsqlConnection _connection;
        private string _connString;
        private string _lastQuery;


        // pagination data
        private int _totalPage = 10;
        private bool _pageChanged = false;



        private ParameterNavBuku _parameterPasLagiNav;
        public ParameterNavBuku ParameterNavPageBuku
        {
            get { return _parameterPasLagiNav; }
            set { _parameterPasLagiNav = value; }
        }
        // data buku

        private ObservableCollection<BukuCardViewModel> _bukuCards;
        public ObservableCollection<BukuCardViewModel> BukuCards
        {
            get => _bukuCards;
            set
            {
                _bukuCards = value;
                OnPropertyChanged(nameof(BukuCards));
            }
        }

        public int PageIndex
        {
            get => ParameterNavPageBuku.query.pageIndex;
            set
            {
                if (ParameterNavPageBuku.query.pageIndex != value)
                {
                    ParameterNavPageBuku.query.pageIndex = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PageIndex));

                    OnPropertyChanged(nameof(PageNumbers));  // Update page numbers when PageIndex changes
                    SearchQuerySQL();
                }
            }
        }

        public int TotalPage
        {
            get => _totalPage;
            set
            {
                if (_totalPage != value)
                {
                    _totalPage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PageNumbers));  // Update page numbers when TotalPage changes
                }
            }
        }

        public string SearchQuery
        {
            get => ParameterNavPageBuku.query.query;
            set
            {
                if (ParameterNavPageBuku.query.query != value)
                {
                    ParameterNavPageBuku.query.query = value;
                    OnPropertyChanged(nameof(SearchQuery));
                }
            }
        }

        // Displayed page numbers based on current PageIndex
        public ObservableCollection<int> PageNumbers => new ObservableCollection<int>(
            Enumerable.Range(Math.Max(1, PageIndex - 2), Math.Min(5, TotalPage - Math.Max(1, PageIndex - 2) + 1))
        );



        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }
        public RelayCommand NavigatePageCommand { get; }
        public BrowseCommand BrowseCommand { get; }

        public BrowsingViewModel(NavigationStore navigationStore, Func<NavbarViewModel> CreateNavbarViewModel, AuthStore authStore) 
        {

            NextPageCommand = new RelayCommand(NextPage, (object obj) => PageIndex < TotalPage);
            PreviousPageCommand = new RelayCommand(PreviousPage, (object obj) => PageIndex > 1);
            NavigatePageCommand = new RelayCommand(NavigateToPage, (object obj) => true);
            BrowseCommand = new BrowseCommand(SearchQuerySQL);

            ParameterNavPageBuku = new ParameterNavBuku() { 
                query = new SearchQuery() { pageIndex = 1, query = ""}
            
            };
            
            _navigationStore = navigationStore;
            _createNavbarViewModel = CreateNavbarViewModel;
            _authStore = authStore;
            _bukuCards = new ObservableCollection<BukuCardViewModel>() { };

            // Dummy buat buku
            //BukuCards.Add(
            //    new BukuCardViewModel(new BukuModel
            //    {
            //        Judul = "Lord of The Rings: Battles",
            //        Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" },
            //        PemilikBuku = new UserModel() { Nama = "MrHouse24", Deskripsi = "fjdksfjdslfds", Kota = "Batam", Provinsi = "Kepri" },
            //        Deskripsi = "Buku yang bagus",
            //        DimilikiSejak = new DateTime(),
            //        Genre = new List<string> { "Action", "Fantasi" },
            //        ISBN = "3213213",
            //        Penerbit = "Ubisoft",
            //        RatingPemilik = 60,
            //        Terbit = 2004
            //    },
            //    CreatePageBukuNavigationService(),
            //    ParameterNavPageBuku)
            //    );

            ConnectToDatabase();

            // Load data
            SearchQuerySQL();
        }

        private void NextPage(object obj)
        {
            if (PageIndex < TotalPage)
            {
                PageIndex++;
                NextPageCommand.RaiseCanExecuteChanged();
                PreviousPageCommand.RaiseCanExecuteChanged();
                _pageChanged = true;
            }
        }

        private void PreviousPage(object obj)
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                NextPageCommand.RaiseCanExecuteChanged();
                PreviousPageCommand.RaiseCanExecuteChanged();
                _pageChanged = true;

            }
        }

        private void NavigateToPage(object parameter)
        {
            if (parameter is int page && page >= 1 && page <= TotalPage)
            {
                PageIndex = page;
                _pageChanged = true;
            }
        }

        private INavigationService<BrowsingViewModel> CreateBackToBrowseNavigationService()
        {
            return new LayoutNavigationService<BrowsingViewModel>(_navigationStore, () => this, _createNavbarViewModel);

        }
        private ParameterNavigationService<ParameterNavBuku, PageBukuViewModel> CreatePageBukuNavigationService()
        {
            return new ParameterNavigationService<ParameterNavBuku, PageBukuViewModel>(_navigationStore, (parameter) => new PageBukuViewModel(parameter, CreateBackToBrowseNavigationService(), _authStore), _createNavbarViewModel);

        }

        public void SearchQuerySQL()
        {
            if(_connection.State != System.Data.ConnectionState.Open)
            {
                //
            }

            if(_connection.State != System.Data.ConnectionState.Open)
            {
                return;
            } else
            {
                bool returnToPage1 = true;
                if(_lastQuery == SearchQuery)
                {
                    returnToPage1 = false;
                }
                _lastQuery = SearchQuery;

                // lanjut kerja
                string querySearch = @"WITH BookData AS (
                    SELECT b.id, b.isbn, b.judul, b.penerbit, b.deskripsi, b.tahun_terbit, b.status, b.created,
                           COALESCE(string_agg(DISTINCT p.nama, ', '), 'Unknown') AS writers,
                           COALESCE(string_agg(DISTINCT g.nama, ', '), 'None') AS genres,
                            pemilik.id AS id_pemilik, pemilik.username, pemilik.email 
                    FROM buku b
                    LEFT JOIN buku_ditulis bd ON b.id = bd.id_buku
                    LEFT JOIN penulis p ON bd.id_penulis = p.id
                    LEFT JOIN genre_buku gb ON b.id = gb.id_buku
                    LEFT JOIN genre g ON gb.id_genre = g.id
                    LEFT JOIN public.user pemilik ON pemilik.id = b.id_pemilik
                    WHERE (b.judul ILIKE '%' || @searchQuery || '%')
                    GROUP BY b.id, pemilik.id
                    )
                    SELECT * FROM BookData
                    ORDER BY id
                    LIMIT @pageSize OFFSET @offset;

                    -- Get total count for pagination
                    SELECT CEIL(COUNT(DISTINCT b.id) * 1.0 / @pageSize) AS total_pages
                    FROM buku b
                    LEFT JOIN buku_ditulis bd ON b.id = bd.id_buku
                    LEFT JOIN penulis p ON bd.id_penulis = p.id
                    LEFT JOIN genre_buku gb ON b.id = gb.id_buku
                    LEFT JOIN genre g ON gb.id_genre = g.id
                    WHERE (b.judul ILIKE '%' || @searchQuery || '%');

                "
                ;


                var command = new NpgsqlCommand(querySearch, _connection);

                command.Parameters.AddWithValue("searchQuery", SearchQuery);
                command.Parameters.AddWithValue("pageSize", 1);
                command.Parameters.AddWithValue("offset", PageIndex - 1);


                var reader = command.ExecuteReader();

                BukuCards.Clear();

                while(reader.Read()) 
                {
                    string gabunganPengarang = reader.GetString(8);
                    List<string> listPengarang = new List<string>(gabunganPengarang.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    // Trim any leading or trailing spaces from each name
                    for (int i = 0; i < listPengarang.Count; i++)
                    {
                        listPengarang[i] = listPengarang[i].Trim();
                    }

                    string gabunganGenre = reader.GetString(9);
                    List<string> listGenre = new List<string>(gabunganGenre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    // Trim any leading or trailing spaces from each name
                    for (int i = 0; i < listGenre.Count; i++)
                    {
                        listGenre[i] = listGenre[i].Trim();
                    }
                    BukuCards.Add(new BukuCardViewModel(new BukuModel
                    {
                        BukuID = reader.GetInt32(0),
                        ISBN = reader.GetString(1),
                        Judul = reader.GetString(2),
                        Penerbit = reader.GetString(3),
                        Deskripsi = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Terbit = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                        status_Buku = reader.GetString(6) == "OPEN_FOR_TUKAR" ? status_buku.OPEN_FOR_TUKAR : status_buku.KOLEKSI,
                        Pengarang = listPengarang, // Aggregated writers
                        Genre = listGenre,  // Aggregated genres
                        PemilikBuku = new UserModel() { Nama = reader.GetString(11), Id = reader.GetInt32(10), Email = reader.GetString(12) },
                        DimilikiSejak = reader.GetDateTime(7)
                    },
                    CreatePageBukuNavigationService(),
                    ParameterNavPageBuku)
                    );



                }

                // Read the total count for pagination
                if (reader.NextResult() && reader.Read())
                {
                    TotalPage = reader.GetInt32(0);
                }

                if(returnToPage1)
                {
                    PageIndex = 1;
                }

                reader.Close();

            }

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

        private async Task InitializeDataAsync()
        {
            try
            {
                //await SearchQuerySQL();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





    }
}
