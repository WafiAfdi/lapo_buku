using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1;

namespace Transaksi
{
    enum StatusTransaksi
    {
        PROCESS,
        DONE,
        FAILED,
        PENDING,
        DISPUTED
    }

    abstract class TransaksiAbstrak
    {
        protected string id_;
        protected DateTime? transaksi_dibuat_, transaksi_terselesaikan_, transaksi_terkonfirmasi;
        protected bool penawar_setuju_, pembeli_setuju_, is_terbayar_, penawar_menerima_, pembeli_menerima_;
        protected StatusTransaksi status_ = StatusTransaksi.PROCESS;


        public abstract void KonfirmasiTransaksiDariPembeli(bool pembeli_deal);
        public abstract void GagalkanTransaksi();
        public abstract void KonfirmasiTransaksiDariPenjual(bool penjual_deal);
        public abstract void TransaksiSelesaiDariPembeli();
        public abstract void TransaksiSelesaiDariPenjual();

        public string ID
        {
            get { return id_; }
        }

    }

    class Transaksi_Peminjaman : TransaksiAbstrak
    {
        private int nomor_pembayaran;


        private User pembeli_;
        private User penjual_;

        private int total_harga_;
        private List<Buku> buku_yang_ditawar;

        


        public override void KonfirmasiTransaksiDariPembeli(bool pembeli_deal) 
        {

        }
        public override void GagalkanTransaksi()
        {

        }
        public override void KonfirmasiTransaksiDariPenjual(bool penjual_deal)
        {

        }
        public override void TransaksiSelesaiDariPembeli()
        {

        }
        public override void TransaksiSelesaiDariPenjual()
        {

        }
    }

    class Transaksi_Penukar : TransaksiAbstrak
    {

        private User pembeli_;
        private User penjual_;

        private int total_harga_;
        private List<Buku> buku_yang_ditawar_pembeli;
        private List<Buku> buku_yang_ditawar_penjual;


        public override void KonfirmasiTransaksiDariPembeli(bool pembeli_deal)
        {

        }
        public override void GagalkanTransaksi()
        {

        }
        public override void KonfirmasiTransaksiDariPenjual(bool penjual_deal)
        {

        }
        public override void TransaksiSelesaiDariPembeli()
        {

        }
        public override void TransaksiSelesaiDariPenjual()
        {

        }
    }

    class Transaksi_Manager
    {
        private User pemilik_data_;
        private List<TransaksiAbstrak> histori_transaksi_;

        private int total_transaksi_;

        public TransaksiAbstrak FindTransaksi(string id) 
        {
            foreach (var transaction in histori_transaksi_)
            {
                if (transaction.ID == id)
                {
                    return transaction;  // Found the transaction with the matching ID
                }
            }
            return null;
        }

        public List<TransaksiAbstrak> HistoriTransaksi
        {
            get { return histori_transaksi_; }
        }

        public void DeleteTransaksi(string id)
        {
            foreach (var transaction in histori_transaksi_)
            {
                if (transaction.ID == id)
                {
                    histori_transaksi_.Remove(transaction);
                    return;  // Found the transaction with the matching ID
                }
            }
        }

        public void AddTransaksi(TransaksiAbstrak transaksi)
        {
            histori_transaksi_.Add(transaksi);
        }
    }
}
