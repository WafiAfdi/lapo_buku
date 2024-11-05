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
    public class LayoutNavigationService<TViewModel> : INavigationService<TViewModel> where TViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _createdViewModel;
        private readonly Func<NavbarViewModel> _createNavbarViewModel;


        public LayoutNavigationService(NavigationStore navigationStore, Func<TViewModel> createdViewModel, Func<NavbarViewModel> createNavbarViewModel)
        {
            _navigationStore = navigationStore;
            _createdViewModel = createdViewModel;
            _createNavbarViewModel = createNavbarViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = new LayoutViewModel(_createNavbarViewModel(), _createdViewModel());
        }
    }
}
