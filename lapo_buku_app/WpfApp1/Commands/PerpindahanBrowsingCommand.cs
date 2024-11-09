using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Service;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.Commands
{
    public class PindaPageBuku : CommandBase
    {
        private readonly ParameterNavigationService<ParameterNavBuku, PageBukuViewModel> _bukuNavigationService;
        private readonly ParameterNavBuku _parameter;

        public PindaPageBuku(ParameterNavigationService<ParameterNavBuku, PageBukuViewModel> bukuNavigationService, ParameterNavBuku parameter)
        {
            _bukuNavigationService = bukuNavigationService;
            _parameter = parameter;
        }

        public override void Execute(object parameter)
        {
            _bukuNavigationService.Navigate(_parameter);
        }

        
    }

    public class PindahPageBrowse : CommandBase
    {
        private readonly INavigationService<BrowsingViewModel> _pindahBrowse;

        public PindahPageBrowse(INavigationService<BrowsingViewModel> BackToBrowse)
        {
            _pindahBrowse = BackToBrowse;
        }

        public override void Execute(object parameter)
        {
            _pindahBrowse.Navigate();
        }


    }
}
