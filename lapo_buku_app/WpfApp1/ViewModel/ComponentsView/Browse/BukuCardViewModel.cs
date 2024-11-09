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
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.ViewModel.ComponentsView.Browse
{
    public class BukuCardViewModel : ViewModelBase
    {
        private readonly BukuModel _bukuCard;

        private readonly ParameterNavigationService<ParameterNavBuku, PageBukuViewModel> _bukuNavigationService;
        private readonly ParameterNavBuku _searchQuery;


        public ICommand NavigatePageBuku { get; }


        public BukuCardViewModel()
        {
            _bukuCard = new BukuModel();
        }

        public BukuCardViewModel(BukuModel buku, ParameterNavigationService<ParameterNavBuku, PageBukuViewModel> bukuNavigationService, ParameterNavBuku searchQuery)
        {
            _bukuCard = buku;
            _bukuNavigationService = bukuNavigationService;
            _searchQuery = searchQuery;
            NavigatePageBuku = new PindaPageBuku(bukuNavigationService, searchQuery);
        }

        public void CardLeftMouseButtonDown()
        {
            _searchQuery.buku = _bukuCard;
            _bukuNavigationService.Navigate(_searchQuery);
        }

        public string Title => _bukuCard.Judul;
        public string Username => _bukuCard.PemilikBuku.Nama;
        public List<string> ListGenre => _bukuCard.Genre;
        public string Deskripsi => _bukuCard.Deskripsi;
        public string ListPenulis => string.Join(", ", _bukuCard.Pengarang.Select(item => item));
        public string DateKepemilikan => _bukuCard.DimilikiSejak.ToString();
    }
}
