using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
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
