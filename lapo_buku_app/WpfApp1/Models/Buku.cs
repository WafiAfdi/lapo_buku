using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public enum status_buku
    {
        KOLEKSI,
        OPEN_FOR_TUKAR
    }
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
        public int? Terbit { get; set; }
        public DateTime DimilikiSejak { get; set; }
        public status_buku status_Buku { get; set; }

        private int _rating;
        public int RatingPemilik
        {
            get => _rating;
            set
            {
                if (value < 0)
                {
                    _rating = 0;
                }
                else if (value > 100)
                {
                    _rating = 100;
                }
                else
                {
                    _rating = value;
                }
            }
        }

    }
}
