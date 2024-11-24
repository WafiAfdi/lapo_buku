using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Automation.Peers;
using WpfApp1.Models;
using WpfApp1.Store;

namespace WpfApp1.ViewModel.MainView
{
    public class PopupTransaksiViewModel : ViewModelBase
    {
        public UserModel PihakLain {  get; set; }
        public BukuModel BukuPihakLain { get; set; }
        public BukuModel BukuUser { get; set; }
        public Boolean IsStatusPending { get; set; } = false;

        public PopupTransaksiViewModel(TransaksiModel transaksiModel, AuthStore authStore, NpgsqlConnection connection) 
        {
            string query = @"
                    SELECT 
                        id, username, email, deskripsi, kota, provinsi, alamat_jalan, kecamatan, nomor_kontak, created, last_update
                    FROM public.user
                    WHERE username = @username";

            string username;

            // User sebagai penawar
            if (authStore.UserLoggedIn.Username == transaksiModel.BukuPenawar.PemilikBuku.Username)
            {
                BukuUser = transaksiModel.BukuPenawar;
                BukuPihakLain = transaksiModel.BukuPenerima;
                username = transaksiModel.BukuPenerima.PemilikBuku.Username;
            }

            // User sebagai penerima
            else
            {
                BukuUser = transaksiModel.BukuPenerima;
                BukuPihakLain = transaksiModel.BukuPenawar;
                username = transaksiModel.BukuPenawar.PemilikBuku.Username;
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
