using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Service
{
    public interface IAuthManager
    {
        bool Register(string username, string password, string email);
        bool Authenticate(string username, string password);
    }

    public class AuthManager : IAuthManager
    {
        private readonly string _connectionString;

        public AuthManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AuthManager()
        {
            // TO DO hubungkan pakai .env
            _connectionString = $"Host=localhost;" +
                                $"Port=5432;" +
                                $"Database=Junpro;" +
                                $"Username=postgres;" +
                                $"Password=password";
        }

        // Registers user baru
        public bool Register(string username, string password, string email)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {

                MessageBox.Show("Tolong isi seluruh input", "Gagal registrasi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                // Cek jika ada
                using (var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.user  WHERE username = @username", connection))
                {
                    checkCmd.Parameters.AddWithValue("@username", username);
                    var count = (long)checkCmd.ExecuteScalar();
                    if (count > 0)
                        return false; // Username sudah ada
                }

                // Insert baru
                using (var cmd = new NpgsqlCommand("INSERT INTO public.user (username, password, email) VALUES (@username, @password, @email)", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@email", email);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Database error: {ex.Message}");
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }

        // Autentikasi
        public bool Authenticate(string username, string password)
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using (var cmd = new NpgsqlCommand("SELECT password FROM user WHERE username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    var storedPassword = cmd.ExecuteScalar()?.ToString();

                    return storedPassword == password; // In a real app, compare hashed passwords
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan", "Login Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
