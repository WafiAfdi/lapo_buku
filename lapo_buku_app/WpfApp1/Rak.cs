using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class Buku
    {
        public int bukuId { get; set; }
        public string isbn { get; set; }
        public string judul { get; set; }
        public List<string> genre { get; set; }
        public List<string> pengarang { get; set; }
        public string penerbit { get; set; }
        public string idPemilik { get; set; }
        public string deskripsi { get; set; }
        public int ratingPemilik { get; set; }
        public int terbit { get; set; }
        public DateTime dimilikiSejak { get; set; }

        public void FetchData(int userId) { }
    }
    internal class Rak
    {
        public int userId { get; }
        public List<Buku> kumpulanBuku { get; }

        public Rak(int userId)
        {
            this.userId = userId;
        }

        public Buku GetBukuAtPosisi(int posisi)
        {
            return new Buku();
        }

        public void AddBuku(Buku buku) { }

        public void EditBuku(int position, Buku buku) { }

        public void DeleteBuku(int position) { }
    }
}
