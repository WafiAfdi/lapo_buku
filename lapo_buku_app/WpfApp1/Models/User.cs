using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class UserModel : ModelBase
    {
        public string Nama { get; set; }
        public string Email { get; set; }
        public string Deskripsi { get; set; }
        public string Kota { get; set; }
        public string Provinsi { get; set; }
        public string AlamatJalan { get; set; }
        public string Kecamatan { get; set; }
        public string Nomor_Kontak { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }



    }
}
