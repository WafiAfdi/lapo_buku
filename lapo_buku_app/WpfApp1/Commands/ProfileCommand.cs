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

    public class SaveEditProfile : CommandBase
    {
        private readonly Action _saveEdit;
        public override void Execute(object parameter)
        {
            _saveEdit();
        }

        public SaveEditProfile(Action saveEdit)
        {
            _saveEdit = saveEdit;

        }
    }

    public class AddBukuCommand : CommandBase
    {
        private readonly Action _addBuku;
        public override void Execute(object parameter)
        {
            _addBuku();
        }

        public AddBukuCommand(Action saveEdit)
        {
            _addBuku = saveEdit;

        }
    }

    public class EditBukuCommand : CommandBase
    {
        private readonly Action _editBuku;
        public override void Execute(object parameter)
        {
            _editBuku();
        }

        public EditBukuCommand(Action saveEdit)
        {
            _editBuku = saveEdit;

        }
    }

    public class DeleteBukuCommand :CommandBase
    {
        private readonly Action _deleteBuku;
        public override void Execute(object parameter)
        {
            _deleteBuku();
        }

        public DeleteBukuCommand(Action deleteBook)
        {
            _deleteBuku = deleteBook;
        }
    }
}
