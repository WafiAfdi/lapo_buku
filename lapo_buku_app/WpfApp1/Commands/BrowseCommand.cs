using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Commands
{
    public class BrowseCommand : CommandBase
    {
        private readonly Action _updateBrowse;
        private bool canSearch = true;
        public BrowseCommand(Action updateBrowse)
        {
            _updateBrowse = updateBrowse;

        }

        public override void Execute(object parameter)
        {
            canSearch = false;
            _updateBrowse();
            canSearch = true;
        }

        public override bool CanExecute(object parameter)
        {
            return canSearch;
        }
    }

    public class TukarCommand : CommandBase
    {
        private readonly Action _tukarBuku;
        private bool canTukar = true;
        public TukarCommand(Action updateTUkar)
        {
            _tukarBuku = updateTUkar;

        }

        public override void Execute(object parameter)
        {
            canTukar = false;
            _tukarBuku();
            canTukar = true;
        }

        public override bool CanExecute(object parameter)
        {
            return canTukar;
        }
    }
}
