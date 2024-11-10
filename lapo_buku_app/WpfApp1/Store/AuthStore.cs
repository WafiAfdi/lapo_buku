using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.Store
{
    public class AuthStore
    {
        // TODO Gab untuk simpan data user
        public UserModel UserLoggedIn { get; set; }

        public bool isLoggedIn => UserLoggedIn != null;
    }
}
