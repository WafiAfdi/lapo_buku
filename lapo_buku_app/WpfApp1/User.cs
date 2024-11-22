using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using WpfApp1.Store;
using System.Security.Cryptography;

namespace WpfApp1
{
    internal class User
    {
        public int userId { get; }
        public Rak rak { get; }

        public string email
        {
            get { return email; }
            set
            {

            }
        }

        public string nama
        {
            get { return nama; }
            set
            {

            }
        }

        public string deskripsi
        {
            get { return deskripsi; }
            set
            {

            }
        }

        public User(int userId) { }
    }

    internal class AuthManager
    {
        private readonly AuthStore _authStore;
        private Boolean _isLoggedIn;
        private NpgsqlConnection _connection;

        public Boolean isLoggedIn { get => _isLoggedIn; }
        public User userLoggedIn { get; }

        public AuthManager(NpgsqlConnection connection, AuthStore authStore)
        {
            _isLoggedIn = false;
            _connection = connection;
            _authStore = authStore;
        }
    

        public async Task Login(string email, string password)
        {
            _isLoggedIn = await LoginAsyncQuery(email, password);
        }

        public async Task<bool> LoginAsyncQuery(string email, string password)
        {

            string query = "SELECT id, email, username, password FROM public.user WHERE email = @Email;";
            var cmd = new NpgsqlCommand(query, _connection);

            // Add parameters to prevent SQL injection
            cmd.Parameters.AddWithValue("Email", email);
            //cmd.Parameters.AddWithValue("Password", password);

            DataTable dataTable = new DataTable();

            var reader = await cmd.ExecuteReaderAsync();

            dataTable.Load(reader);

            if (dataTable.Rows.Count > 0)
            {
                string storedHashedPassword = dataTable.Rows[0]["password"].ToString();
                string inputPassword = password; // Provided by the user

                if (VerifyPassword(inputPassword, storedHashedPassword))
                {
                    // Store data in accountStore if needed
                    if (!_authStore.IsLoggedIn)
                    {
                        _authStore.UserLoggedIn = new Models.UserModel();
                    }

                    _authStore.UserLoggedIn.Id = int.Parse(dataTable.Rows[0]["id"].ToString());
                    _authStore.UserLoggedIn.Email = dataTable.Rows[0]["email"].ToString();
                    _authStore.UserLoggedIn.Username = dataTable.Rows[0]["username"].ToString();

                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Boolean Register(string email, string password)
        {
            return false;
        }

        bool VerifyPassword(string password, string storedHash)
        {
            // Convert the base64-encoded hash to bytes
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract the salt size (assumes 16-byte salt)
            const int saltSize = 16;
            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            // Extract the hash size (assumes 32-byte key)
            const int keySize = 32;
            byte[] storedKey = new byte[keySize];
            Array.Copy(hashBytes, saltSize, storedKey, 0, keySize);

            // Re-hash the input password using the same salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] inputKey = pbkdf2.GetBytes(keySize);

                // Compare the keys
                for (int i = 0; i < keySize; i++)
                {
                    if (storedKey[i] != inputKey[i])
                        return false;
                }
            }

            return true;
        }
    }
}
