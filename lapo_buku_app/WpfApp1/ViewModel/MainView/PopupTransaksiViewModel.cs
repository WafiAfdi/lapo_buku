using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using WpfApp1.Models;

namespace WpfApp1.ViewModel.MainView
{
    public class PopupTransaksiViewModel : ViewModelBase
    {
        public UserModel PihakLain {  get; set; }
        public BukuModel BukuPihakLain { get; set; }
        public BukuModel BukuUser { get; set; }
        public Boolean IsStatusPending { get; set; } = false;

        public PopupTransaksiViewModel() 
        {
            SetData();
            SetStatus();
        }

        private void SetData()
        {

        }

        private void SetStatus()
        {

        }
    }
}
