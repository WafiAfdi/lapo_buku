using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1;

namespace BukuSearch
{
   class Filter 
    {
        public string genre;
        public string pengarang;
        public string penerbit;
        public int TahunTerbit;

        public List<Buku> SearchByGenre(string genre, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.genre.Contains(genre)).ToList();
        }

        //  mencari buku berdasarkan pengarang
        public List<Buku> SearchByPengarang(string pengarang, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.pengarang.Contains(pengarang)).ToList();
        }

        //  mencari buku berdasarkan penerbit
        public List<Buku> SearchByPenerbit(string penerbit, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.penerbit == penerbit).ToList();
        }

        // mencari buku berdasarkan tahun terbit
        public List<Buku> SearchByTahunTerbit(int tahun, List<Buku> bukuList)
        {
            return bukuList.Where(buku => buku.dimilikiSejak.DayOfYear == tahun).ToList();
        }
    }

    class SearchManager
    {
        private List<string> historySearch;
        public string currentSearch;
        public List<Buku> hasilPencarianBuku;
        public Filter filterBuku;
        public int totalPage;
        public int currentPage;
        public int limitJumlahSearch;

        public void setCurrentSearch (string currentSearch)
        {

        }
        public List<string> getHistorySearch()
        {
            return historySearch;
        }

        public void searchBuku()
        {

        }
    }
}