using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.ViewModel;

namespace WpfApp1.Service
{
    public interface INavigationService<TViewModel> where TViewModel : ViewModelBase
    {
        void Navigate();
    }
}
