using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Commands;

namespace WpfApp1.ViewModel.MainView
{
    public class BrowsingViewModel : ViewModelBase
    {
        // pagination data
        private int _pageIndex = 1;
        private int _totalPage = 10;

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
        public ICommand NavigatePageBuku { get; }
        public BrowsingViewModel() 
        {
            NextPageCommand = new RelayCommand(NextPage, (object obj) => PageIndex < TotalPage);
            PreviousPageCommand = new RelayCommand(PreviousPage, (object obj) => PageIndex > 1);
            NavigatePageCommand = new RelayCommand(NavigateToPage, (object obj) => true);
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
