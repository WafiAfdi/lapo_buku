using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Models;
using WpfApp1.Store;
using WpfApp1.View.MainApp.Profile;

namespace WpfApp1.ViewModel.MainView
{
    public class PopupAddEditBook : ViewModelBase
    {
        public UserModel PihakLain { get; set; }
        public BukuModel BukuPihakLain { get; set; }
        public BukuModel BukuUser { get; set; }

        public PopupAddEditBook()
        {
            BukuUser = new BukuModel();
            BukuPihakLain = new BukuModel();
            PihakLain = new UserModel();

            BukuUser.Judul = "Lord of The Rings";
            PihakLain.Nama = "Andi";
        }
    }
}