using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

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
        private Boolean _isLoggedIn;
        private NpgsqlConnection _connection;

        public Boolean isLoggedIn { get => _isLoggedIn; }
        public User userLoggedIn { get; }

        public AuthManager(NpgsqlConnection connection)
        {
            _isLoggedIn = false;
            _connection = connection;
        }

        public void Login(string email, string password)
        {
            string query = $"SELECT COUNT(1) FROM user WHERE email = @email AND password = @password";
            var cmd = new NpgsqlCommand(query, _connection);

            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("password", password);

            int result = Convert.ToInt32(cmd.ExecuteScalar());

            if (result > 0)
                _isLoggedIn = true;
            else
                _isLoggedIn = false;
        }

        public Boolean Register(string email, string password)
        {
            return false;
        }
    }
}
