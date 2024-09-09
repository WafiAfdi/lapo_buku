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
        public Boolean isLoggedIn { get; }
        public User userLoggedIn { get; }

        public void Login() { }
        public void Register(string email, string password) { }
    }
}
