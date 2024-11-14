using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Store;

namespace WpfApp1.Commands
{
    public class LogoutCommand : CommandBase
    {
        private readonly Action _displayLogout;
        private readonly AuthStore _authStore;
        public override void Execute(object parameter)
        {
            _authStore.UserLoggedIn = null;
            _displayLogout();
            // Todo clear account information 
        }

        public LogoutCommand(Action displayLogout, AuthStore authStore)
        {
            _authStore = authStore;
            _displayLogout = displayLogout;
        }
    }
}
