﻿using System;
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
    /// Interaction logic for BrowsingView.xaml
    /// </summary>
    public partial class BrowsingView : UserControl
    {
        public BrowsingView()
        {
            InitializeComponent();
        }

        private void SearchBox_Keydown(object sender, KeyEventArgs e)
        {
            // Check if the Enter key was pressed
            if (e.Key == Key.Enter)
            {
                // Execute your command
                ((BrowsingViewModel)DataContext).SearchQuerySQL();
            }
        }
    }
}
