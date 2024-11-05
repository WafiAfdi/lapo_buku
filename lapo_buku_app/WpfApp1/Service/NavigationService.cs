using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Store;
using WpfApp1.ViewModel;

namespace WpfApp1.Service
{
    public class NavigationService<TViewModel> : INavigationService<TViewModel> where TViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _createdViewModel;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> createdViewModel)
        {
            _navigationStore = navigationStore;
            _createdViewModel = createdViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _createdViewModel();
        }
    }
}
