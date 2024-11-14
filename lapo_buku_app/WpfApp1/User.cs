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
    

        public async void Login(string email, string password)
        {
            _isLoggedIn = await LoginAsyncQuery(email, password);
        }

        public async Task<bool> LoginAsyncQuery(string email, string password)
        {

            string query = "SELECT id, email, username FROM public.user WHERE email = @Email AND password = @Password";
            var cmd = new NpgsqlCommand(query, _connection);

            // Add parameters to prevent SQL injection
            cmd.Parameters.AddWithValue("Email", email);
            cmd.Parameters.AddWithValue("Password", password);

            DataTable dataTable = new DataTable();

            var reader = await cmd.ExecuteReaderAsync();

            dataTable.Load(reader);

            if (dataTable.Rows.Count > 0)
            {
                // Store data in accountStore if needed
                if(!_authStore.IsLoggedIn)
                {
                    _authStore.UserLoggedIn = new Models.UserModel();
                }
                _authStore.UserLoggedIn.Id = int.Parse(dataTable.Rows[0]["id"].ToString());
                _authStore.UserLoggedIn.Email = dataTable.Rows[0]["email"].ToString();
                _authStore.UserLoggedIn.Username = dataTable.Rows[0]["username"].ToString();

                return true;
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
    }
}
