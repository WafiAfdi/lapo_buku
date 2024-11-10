using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Commands
{
    public class LogoutCommand : CommandBase
    {
        private readonly Action _displayLogout;
        public override void Execute(object parameter)
        {
            _displayLogout();
            // Todo clear account information 
        }

        public LogoutCommand(Action displayLogout)
        {
            _displayLogout = displayLogout;
        }
    }
}
