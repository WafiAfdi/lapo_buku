using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Converters
{
    public class PageIndexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Ensure the value and parameter are integers
            if (value is int pageIndex && parameter is int buttonIndex)
            {
                // Return a different color if the pageIndex matches the button index
                return pageIndex == buttonIndex ? Brushes.IndianRed : Brushes.White;
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
