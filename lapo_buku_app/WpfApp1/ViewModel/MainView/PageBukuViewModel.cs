using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly BukuModel ModelPageBuku; // data buku untuk page
        private readonly SearchQuery searchSebelumnya;
        private readonly INavigationService<BrowsingViewModel> _backToBrowseNavigationService;
        private readonly AuthStore _authStore;
        public ObservableCollection<BukuModel> BukuYangBisaDitawarkan { get; } = new ObservableCollection<BukuModel>(); // buku dari pemilik yang bisa ditukar
        private BukuModel _bukuYangTerpilih;


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
        public ICommand TukarBuku { get; }

        public PageBukuViewModel(ParameterNavBuku parameter, INavigationService<BrowsingViewModel> BackToBrowseNavService, AuthStore authStore)
        {
            ModelPageBuku = parameter.buku;
            searchSebelumnya = parameter.query;

            _backToBrowseNavigationService = BackToBrowseNavService;
            _authStore = authStore;
            BalikPageBrowse = new PindahPageBrowse(BackToBrowseNavService);

            // Sampel Data
            BukuYangBisaDitawarkan.Add(new BukuModel { Judul = "Lord of The Rings: Battles", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } });
            BukuYangBisaDitawarkan.Add(new BukuModel { Judul = "The Hobbit", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } });


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


        public bool CanTukar => _bukuYangTerpilih != null;


    }
}
