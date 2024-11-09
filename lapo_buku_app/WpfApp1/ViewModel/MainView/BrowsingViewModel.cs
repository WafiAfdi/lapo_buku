using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Models;
using WpfApp1.Service;
using WpfApp1.ViewModel.ComponentsView.Browse;

namespace WpfApp1.ViewModel.MainView
{

    public class SearchQuery
    {
        public string query;
        public int pageIndex;
    }

    public class ParameterNavBuku
    {
        public SearchQuery query;
        public BukuModel buku;
    }
    public class BrowsingViewModel : ViewModelBase
    {
        // pagination data
        private int _pageIndex = 1;
        private int _totalPage = 10;

        private readonly ParameterNavigationService<ParameterNavBuku, PageBukuViewModel> _bukuNavigationService;


        private ParameterNavBuku _parameterPasLagiNav;
        public ParameterNavBuku ParameterNavPageBuku
        {
            get { return _parameterPasLagiNav; }
            set { _parameterPasLagiNav = value; }
        }
        // data buku
        public ObservableCollection<BukuCardViewModel> BukuCards { get; } = new ObservableCollection<BukuCardViewModel>();

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (_pageIndex != value)
                {
                    _pageIndex = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PageIndex));

                    OnPropertyChanged(nameof(PageNumbers));  // Update page numbers when PageIndex changes
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

        // Displayed page numbers based on current PageIndex
        public ObservableCollection<int> PageNumbers => new ObservableCollection<int>(
            Enumerable.Range(Math.Max(1, PageIndex - 2), Math.Min(5, TotalPage - Math.Max(1, PageIndex - 2) + 1))
        );



        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }
        public RelayCommand NavigatePageCommand { get; }
        public BrowsingViewModel(ParameterNavigationService<ParameterNavBuku, PageBukuViewModel> bukuNavigationService) 
        {
            _bukuNavigationService = bukuNavigationService;
            NextPageCommand = new RelayCommand(NextPage, (object obj) => PageIndex < TotalPage);
            PreviousPageCommand = new RelayCommand(PreviousPage, (object obj) => PageIndex > 1);
            NavigatePageCommand = new RelayCommand(NavigateToPage, (object obj) => true);

            // Dummy buat buku
            BukuCards.Add(new BukuCardViewModel(new BukuModel { Judul = "Lord of The Rings: Battles", Pengarang = new List<string>{ "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" }}, bukuNavigationService, ParameterNavPageBuku));
            BukuCards.Add(new BukuCardViewModel(new BukuModel { Judul = "The Hobbit", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } }, bukuNavigationService, ParameterNavPageBuku));
            BukuCards.Add(new BukuCardViewModel(new BukuModel { Judul = "The Hobbit 2", Pengarang = new List<string> { "Wafi Afdi Alfaruqhi", "J.R.R. Tolkien" } }, bukuNavigationService, ParameterNavPageBuku));
        }

        private void NextPage(object obj)
        {
            if (PageIndex < TotalPage)
            {
                PageIndex++;
                NextPageCommand.RaiseCanExecuteChanged();
                PreviousPageCommand.RaiseCanExecuteChanged();
            }
        }

        private void PreviousPage(object obj)
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                NextPageCommand.RaiseCanExecuteChanged();
                PreviousPageCommand.RaiseCanExecuteChanged();

            }
        }

        private void NavigateToPage(object parameter)
        {
            if (parameter is int page && page >= 1 && page <= TotalPage)
            {
                PageIndex = page;
            }
        }



    }
}
