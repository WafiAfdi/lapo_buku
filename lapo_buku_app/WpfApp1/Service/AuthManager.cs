using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        private NpgsqlConnection _connection;

        public AuthManager(string connectionString)
        {
            _connectionString = connectionString;
            try
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
                MessageBox.Show("Database connected successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                // Cek jika ada
                using (var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.user  WHERE username = @username", _connection))
                {
                    checkCmd.Parameters.AddWithValue("@username", username);
                    var count = (long)checkCmd.ExecuteScalar();
                    if (count > 0)
                        return false; // Username sudah ada
                }

                // hashing password
                string hashedPassword = HashPassword(password);

                // Insert baru
                using (var cmd = new NpgsqlCommand("INSERT INTO public.user (username, password, email) VALUES (@username, @password, @email)", _connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
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

                using (var cmd = new NpgsqlCommand("SELECT password FROM user WHERE username = @username", _connection))
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

        private string HashPassword(string password)
        {
            const int saltSize = 16; // 16 bytes salt
            const int keySize = 32;  // 32 bytes hash
            const int iterations = 10000;

            // Generate a random salt
            byte[] salt = new byte[saltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] hash = pbkdf2.GetBytes(keySize);

                // Combine salt and hash into a single byte array
                byte[] hashBytes = new byte[saltSize + keySize];
                Array.Copy(salt, 0, hashBytes, 0, saltSize);
                Array.Copy(hash, 0, hashBytes, saltSize, keySize);

                // Convert the hash bytes to a Base64 string for storage
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
