using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Npgsql;
using WpfApp1.Models;

namespace WpfApp1.Commands
{
    public class TerimaCommand : CommandBase
    {
        private NpgsqlConnection _connection;
        private Window _window;
        private int _idTransaksi;

        public TerimaCommand(int idTransaksi, NpgsqlConnection connection, Window window) 
        {
            _connection = connection;
            _idTransaksi = idTransaksi;
            _window = window;
        }

        public override void Execute(object parameter)
        {
            TerimaTransaksi();
            _window.Close();
        }

        private void TerimaTransaksi()
        {
            try
            {
                // Query untuk mengupdate status transaksi
                string updateQuery = @"
                        UPDATE public.transaksi_penukaran
                        SET status = 'PROCESS', last_updated = NOW(), is_penjual_konfirmasi = true
                        WHERE id = @idTransaksi;
                    ";

                using (var command = new NpgsqlCommand(updateQuery, _connection))
                {
                    // Parameter untuk query
                    command.Parameters.AddWithValue("@idTransaksi", _idTransaksi);

                    // Eksekusi query
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class KonfirmasiCommand : CommandBase
    {
        private readonly TransaksiModel _transaksi;
        private NpgsqlConnection _connection;
        private bool _isPenjual { get; }
        private int _idTransaksi { get; }
        private Window _window;

        public KonfirmasiCommand(int idTransaksi, NpgsqlConnection connection, bool isPenjual, Window window, TransaksiModel transaksi)
        {

            _idTransaksi = idTransaksi;
            _connection = connection;
            _isPenjual = isPenjual;
            _window = window;
            _transaksi = transaksi;
        }

        public override void Execute(object parameter)
        {
            Konfirmasi();
            _window.Close();
        }

        private void Konfirmasi()
        {
            // Query untuk mengupdate status transaksi
            string updateQuery;

            if(_isPenjual)
            {
                updateQuery = @"
                        UPDATE public.transaksi_penukaran
                        SET is_penjual_menerima = true, last_updated = NOW(), date_konfirmasi_penjual_menerima = NOW()
                        WHERE id = @idTransaksi;
                    ";
            }
            else
            {
                updateQuery = @"
                        UPDATE public.transaksi_penukaran
                        SET is_pembeli_menerima = true, last_updated = NOW(), date_konfirmasi_pembeli_menerima = NOW()
                        WHERE id = @idTransaksi;
                    ";
            }

            try
            {
                using (var command = new NpgsqlCommand(updateQuery, _connection))
                {
                    // Parameter untuk query
                    command.Parameters.AddWithValue("@idTransaksi", _idTransaksi);

                    // Eksekusi query
                    int rowsAffected = command.ExecuteNonQuery();
                }

                updateQuery = @"
                    -- Pastikan transaksi valid dan status menjadi DONE
                    UPDATE public.transaksi_penukaran
                    SET status = 'DONE', 
                        last_updated = NOW(), 
                        date_transaksi_selesai = NOW()
                    WHERE id = @idTransaksi
                      AND is_penjual_menerima = true
                      AND is_pembeli_menerima = true
                      AND status != 'DONE';

                    -- Tukar kepemilikan buku hanya jika status sudah DONE
                    UPDATE public.buku 
                    SET id_pemilik = @id_penjual 
                    WHERE id = @id_buku_pembeli
                      AND EXISTS (
                          SELECT 1 
                          FROM public.transaksi_penukaran 
                          WHERE id = @idTransaksi AND status = 'DONE'
                      );

                    UPDATE public.buku 
                    SET id_pemilik = @id_pembeli 
                    WHERE id = @id_buku_penjual
                      AND EXISTS (
                          SELECT 1 
                          FROM public.transaksi_penukaran 
                          WHERE id = @idTransaksi AND status = 'DONE'
                      );
                ";

                using (var command = new NpgsqlCommand(updateQuery, _connection))
                {
                    // Parameter untuk query

                    command.Parameters.AddWithValue("@idTransaksi", _idTransaksi);
                    command.Parameters.AddWithValue("@id_buku_pembeli", _transaksi.BukuPenerima.BukuID);
                    command.Parameters.AddWithValue("@id_buku_penjual", _transaksi.BukuPenawar.BukuID);
                    command.Parameters.AddWithValue("@id_pembeli", _transaksi.BukuPenerima.PemilikBuku.Id);
                    command.Parameters.AddWithValue("@id_penjual", _transaksi.BukuPenawar.PemilikBuku.Id);

                    // Eksekusi query
                    int rowsAffected = command.ExecuteNonQuery();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class TolakCommand : CommandBase
    {
        private NpgsqlConnection _connection;
        private int _idTransaksi;
        private Window _window;

        public TolakCommand(int idTransaksi, NpgsqlConnection connection, Window window)
        {
            _idTransaksi = idTransaksi;
            _connection = connection;
            _window = window;
        }

        public override void Execute(object parameter)
        {
            TolakTransaksi();
            _window.Close();
        }

        private void TolakTransaksi()
        {
            try
            {
                // Query untuk mengupdate status transaksi
                string updateQuery = @"
                        UPDATE public.transaksi_penukaran
                        SET status = 'DONE', last_updated = NOW()
                        WHERE id = @idTransaksi;
                    ";

                using (var command = new NpgsqlCommand(updateQuery, _connection))
                {
                    // Parameter untuk query
                    command.Parameters.AddWithValue("@idTransaksi", _idTransaksi);

                    // Eksekusi query
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
