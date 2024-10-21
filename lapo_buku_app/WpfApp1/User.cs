using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Boolean isLoggedIn { get => _isLoggedIn; }
        public User userLoggedIn { get; }

        public AuthManager()
        {
            _isLoggedIn = false;
        }

        public void Login(string email, string password) 
        {
            if (email == "test" && password == "test")
            {
                _isLoggedIn = true;
            }
        }
        public Boolean Register(string email, string password) 
        {
            return false;
        }
    }
}
