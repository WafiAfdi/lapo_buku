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

namespace WpfApp1.ViewModel.MainView
{
    public class TransaksiViewModel : ViewModelBase
    { 
        public ObservableCollection<UserTransaksi> Users { get; set; } = new ObservableCollection<UserTransaksi>();
        public ObservableCollection<ComboOption> ComboStatus { get; set; }
        public ObservableCollection<ComboOption> ComboPihak { get; set; }
        public ComboOption SelectedComboStatus { get; set; }
        public ComboOption SelectedComboPihak { get; set; }
        private PopupTransaksiViewModel _popupTransaksiViewModel { get; set; }

        public ICommand SearchCommand;

        public string SearchQuery
        { get; set; }


        public TransaksiViewModel()
        {
            ComboStatus = new ObservableCollection<ComboOption> 
            {
                new ComboOption("Semua"),
                new ComboOption("PENDING"), 
                new ComboOption("PROCESS"), 
                new ComboOption("FAILED"), 
                new ComboOption("DONE")
            };

            ComboPihak = new ObservableCollection<ComboOption> 
            { 
                new ComboOption("Semua"),
                new ComboOption("Penawar"),
                new ComboOption("Penerima")
            };

            Users.Add(new UserTransaksi("test", "test", "test", "test", "test", "test", "test"));

            Window _popupTransaksi = new PopupTransaksi();
            _popupTransaksiViewModel = new PopupTransaksiViewModel();
            _popupTransaksi.DataContext = _popupTransaksiViewModel;
            _popupTransaksi.Show();
        }
    }

    public class ComboOption
    {
        public string Name { get; }
        public ComboOption(string name)
        {
            Name = name;
        }
    }

    public class UserTransaksi
    {
        public string IdTransaksi { get; }
        public string BukuPenawar { get; }
        public string BukuPenerima { get; }
        public string Status { get; }
        public string UsernamePenawar { get; }
        public string UsernamePenerima { get; }
        public string Date {  get; }

        public UserTransaksi
        (
            string idTransaksi,
            string bukuPenawar,
            string bukuPenerima,
            string status,
            string usernamePenawar,
            string usernamePenerima,
            string date
        ) 
        {
            IdTransaksi = idTransaksi;
            BukuPenawar = bukuPenawar;
            BukuPenerima = bukuPenerima;
            Status = status;
            UsernamePenawar = usernamePenawar;
            UsernamePenerima = usernamePenerima;
            Date = date;
            
        }
    }
}
