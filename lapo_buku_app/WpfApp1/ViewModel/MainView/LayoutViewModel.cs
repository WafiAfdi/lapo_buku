using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModel.MainView
{
    public class LayoutViewModel : ViewModelBase
    {
        public NavbarViewModel NavigationViewModel { get; }

        public ViewModelBase ContentViewModel { get; }

        public LayoutViewModel(NavbarViewModel navigationViewModel, ViewModelBase contentViewModel)
        {
            NavigationViewModel = navigationViewModel;
            ContentViewModel = contentViewModel;
        }


    }
}
