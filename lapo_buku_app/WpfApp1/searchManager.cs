using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BukuSearch
{
+   class Filter 
    {
        public string genre;
        public string pengarang;
        public string penerbit;
        public int TahunTerbit;

        public List<Buku> SearchByGenre(string genre, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.Genre == genre).ToList();
        }

        //  mencari buku berdasarkan pengarang
        public List<Buku> SearchByPengarang(string pengarang, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.Pengarang == pengarang).ToList();
        }

        //  mencari buku berdasarkan penerbit
        public List<Buku> SearchByPenerbit(string penerbit, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.Penerbit == penerbit).ToList();
        }

        // mencari buku berdasarkan tahun terbit
        public List<Buku> SearchByTahunTerbit(int tahun, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.TahunTerbit == tahun).ToList();
        }
    }

    class searchManager
    {
        private List<currentSearch> historySearch;
        public string currentSearch;
        public list<Buku> hasilPencarianBuku;
        public Filter filterBuku;
        public int totalPage;
        public int currentPage;
        public int limitJumlahSearch;

        public void setCurrentSearch (string currentSearch)
        {

        }
        public void getHistorySearch()
        {
            foreach (var search in HistorySearch)
            {
                return search;
            }
        }

        public void searchBuku()
        {

        }
    }
}