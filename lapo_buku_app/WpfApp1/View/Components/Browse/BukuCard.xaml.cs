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
using WpfApp1.ViewModel.ComponentsView.Browse;
using WpfApp1.ViewModel.MainView;

namespace WpfApp1.View.Components.Browse
{
    /// <summary>
    /// Interaction logic for BukuCard.xaml
    /// </summary>
    public partial class BukuCard : UserControl
    {
        public BukuCard()
        {
            InitializeComponent();
        }

        private void CardLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            // You can access your ViewModel here, for example
            var viewModel = (BukuCardViewModel)DataContext;

            // Call a function from the ViewModel
            viewModel.CardLeftMouseButtonDown();
        }
    }
}
