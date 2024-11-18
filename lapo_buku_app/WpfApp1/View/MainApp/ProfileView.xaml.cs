using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.View.MainApp
{
    /// <summary>
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();

        }

        private void Open_Add(object sender, RoutedEventArgs e)
        {

            ((ProfileViewModel)DataContext).ShowAddWindow();


        }

        private void Open_Edit(object sender, RoutedEventArgs e)
        {

            ((ProfileViewModel)DataContext).ShowEditWindow();


        }

        private void deleteClick(object sender, RoutedEventArgs e)
        {

            ((ProfileViewModel)DataContext).deleteSelectedBook();


        }
    }
}
