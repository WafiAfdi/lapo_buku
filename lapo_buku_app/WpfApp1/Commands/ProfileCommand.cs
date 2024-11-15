using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Commands
{
    public class ProfileCommand : CommandBase
    {
        private readonly Action _bukaEdit;
        public override void Execute(object parameter)
        {
            _bukaEdit();
        }

        public ProfileCommand(Action bukaEdit) { 
            _bukaEdit = bukaEdit;
        
        }
    }
}
