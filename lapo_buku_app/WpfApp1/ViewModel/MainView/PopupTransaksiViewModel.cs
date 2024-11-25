using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Automation.Peers;
using WpfApp1.Commands;
using WpfApp1.Models;
using WpfApp1.Store;
using System.Windows;


namespace WpfApp1.ViewModel.MainView
{
    public class PopupTransaksiViewModel : ViewModelBase
    {
        private Window _window;
        public UserModel PihakLain { get; set; }
        public BukuModel BukuPihakLain { get; set; }
        public BukuModel BukuUser { get; set; }

        public Boolean ButtonPenerima { get; set; } = false;
        public Boolean ButtonPenawar { get; set; } = false;
        public Boolean ButtonKonfirmasi { get; set; } = false;

        public TerimaCommand TerimaCommand { get; }
        public TolakCommand TolakCommand { get; }
        public KonfirmasiCommand KonfirmasiCommand { get; }

        public PopupTransaksiViewModel(TransaksiModel transaksiModel, AuthStore authStore, NpgsqlConnection connection, Window window) 
        {
            _window = window;
            TerimaCommand = new TerimaCommand(transaksiModel.IdTransaksi, connection, _window);
            TolakCommand = new TolakCommand(transaksiModel.IdTransaksi, connection, _window);
            
            string query = @"
                    SELECT 
                        id, username, email, deskripsi, kota, provinsi, alamat_jalan, kecamatan, nomor_kontak, created, last_update
                    FROM public.user
                    WHERE username = @username";

            string username;

            if(transaksiModel.Status == "PROCESS")
            {
                ButtonKonfirmasi = true;
            }

            // User sebagai penawar
            if (authStore.UserLoggedIn.Username == transaksiModel.BukuPenawar.PemilikBuku.Username)
            {
                KonfirmasiCommand = new KonfirmasiCommand(transaksiModel.IdTransaksi, connection, true, _window, transaksiModel);

                BukuUser = transaksiModel.BukuPenawar;
                BukuPihakLain = transaksiModel.BukuPenerima;
                username = transaksiModel.BukuPenerima.PemilikBuku.Username;

                if(transaksiModel.Status == "PENDING")
                {
                    ButtonPenawar = true;
                }
                if(transaksiModel.IsPenjualTerima) 
                {
                    ButtonKonfirmasi = false;
                }
            }

            // User sebagai penerima
            else
            {
                KonfirmasiCommand = new KonfirmasiCommand(transaksiModel.IdTransaksi, connection, false, _window, transaksiModel);

                BukuUser = transaksiModel.BukuPenerima;
                BukuPihakLain = transaksiModel.BukuPenawar;
                username = transaksiModel.BukuPenawar.PemilikBuku.Username;

                if( transaksiModel.Status == "PENDING")
                {
                    ButtonPenerima = true;
                }
                if( transaksiModel.IsPembeliTerima)
                {
                    ButtonKonfirmasi = false;
                }
            }

            using (var cmd = new NpgsqlCommand(query, connection))
            {
                // Menambahkan parameter untuk username
                cmd.Parameters.AddWithValue("username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        PihakLain = new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Deskripsi = reader.IsDBNull(reader.GetOrdinal("deskripsi")) ? "-" : reader.GetString(reader.GetOrdinal("deskripsi")),
                            Kota = reader.IsDBNull(reader.GetOrdinal("kota")) ? "-" : reader.GetString(reader.GetOrdinal("kota")),
                            Provinsi = reader.IsDBNull(reader.GetOrdinal("provinsi")) ? "-" : reader.GetString(reader.GetOrdinal("provinsi")),
                            AlamatJalan = reader.IsDBNull(reader.GetOrdinal("alamat_jalan")) ? "-" : reader.GetString(reader.GetOrdinal("alamat_jalan")),
                            Kecamatan = reader.IsDBNull(reader.GetOrdinal("kecamatan")) ? "-" : reader.GetString(reader.GetOrdinal("kecamatan")),
                            Nomor_Kontak = reader.IsDBNull(reader.GetOrdinal("nomor_kontak")) ? "-" : reader.GetString(reader.GetOrdinal("nomor_kontak")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created")),
                            LastUpdated = reader.GetDateTime(reader.GetOrdinal("last_update"))
                        };
                    }
                }
            }
        }
    }
}
