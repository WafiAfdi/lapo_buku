using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Store;
using WpfApp1.ViewModel;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.Service
{
    public class ParameterNavigationService<TParameter, TViewModel> where TViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TParameter, TViewModel> _createViewModel;
        private readonly Func<NavbarViewModel> _createNavbarViewModel;

        public ParameterNavigationService(NavigationStore navigationStore, Func<TParameter, TViewModel> createViewModel, Func<NavbarViewModel> createdNavbarViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
            _createNavbarViewModel = createdNavbarViewModel;
        }

        public void Navigate(TParameter parameter)
        {
            _navigationStore.CurrentViewModel = new LayoutViewModel(_createNavbarViewModel(), _createViewModel(parameter));
        }
    }
}
