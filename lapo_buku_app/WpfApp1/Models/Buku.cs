using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class BukuModel : ModelBase
    {
        public int BukuID { get; set; }
        public string ISBN { get; set; }
        public string Judul { get; set; }
        public List<string> Genre { get; set; }
        public List<string> Pengarang { get; set; }
        public string Penerbit { get; set; }
        public string IdPemilik { get; set; }
        public UserModel PemilikBuku { get; set; }
        public string Deskripsi { get; set; }
        public int RatingPemilik { get; set; }
        public int Terbit { get; set; }
        public DateTime DimilikiSejak { get; set; }

    }
}
