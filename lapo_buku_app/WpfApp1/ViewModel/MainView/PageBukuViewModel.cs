using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.View.Components.Browse;
using WpfApp1.ViewModel.ComponentsView.Browse;

namespace WpfApp1.ViewModel.MainView
{
    public class PageBukuViewModel : ViewModelBase
    {
        private readonly BukuModel ModelPageBuku; // data buku untuk page
        private readonly SearchQuery searchSebelumnya;
        public ObservableCollection<BukuModel> BukuYangBisaDitawarkan { get; } = new ObservableCollection<BukuModel>(); // buku dari pemilik yang bisa ditukar
        private BukuModel _bukuYangTerpilih;
        public BukuModel BukuYangTerpilih
        {
            get => _bukuYangTerpilih;
            set
            {
                _bukuYangTerpilih = value;
                OnPropertyChanged(); // Notify UI of property change
            }
        }

        public ICommand BalikPageBrowse {  get; }
        public ICommand TukarBuku { get; }

        public PageBukuViewModel(BukuModel modelPageBuku, SearchQuery searchBefore) 
        {
            ModelPageBuku = modelPageBuku;
            searchSebelumnya = searchBefore;

            // Sampel Data
            BukuYangBisaDitawarkan.Add(new BukuModel { Judul = "Lord of The Rings: Battles", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } });
            BukuYangBisaDitawarkan.Add(new BukuModel { Judul = "The Hobbit", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } });


        }

        public PageBukuViewModel(ParameterNavBuku parameter)
        {
            ModelPageBuku = new BukuModel { Judul = "Lord of The Rings: Battles", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } };
            searchSebelumnya = new SearchQuery { pageIndex = 1, query = "" };

            // Sampel Data
            BukuYangBisaDitawarkan.Add(new BukuModel { Judul = "Lord of The Rings: Battles", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } });
            BukuYangBisaDitawarkan.Add(new BukuModel { Judul = "The Hobbit", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } });


        }
    }
}
