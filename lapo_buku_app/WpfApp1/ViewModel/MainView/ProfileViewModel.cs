using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Models;
using WpfApp1.View.MainApp.Profile;

namespace WpfApp1.ViewModel.MainView
{
    public class ProfileViewModel : ViewModelBase
    {
        private UserModel test_;
        private Window _popupWindow;
        public UserModel Test { get { return test_; } set => test_ = value; }

        public ICommand editButtonCommand;

        public ProfileViewModel()
        {
            test_ = new UserModel() { Username = "ZEKI", Email = "hezekielsitepu@mail.ugm.ac.id", Deskripsi = "Masukkan Deskripsi Disini"};
            editButtonCommand = new ProfileCommand(ubahNama);
        }

        public void ubahNama()
        {
            Window _popupWindow = new EditProfile()
            {
                DataContext = this
            };
            _popupWindow.ShowDialog();
        }


    }
}
