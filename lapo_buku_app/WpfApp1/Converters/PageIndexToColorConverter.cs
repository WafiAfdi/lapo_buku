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
    // untuk background
    public class PageIndexToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int pageNumber && values[1] is int pageIndex)
            {
                return pageNumber == pageIndex ? new SolidColorBrush(Colors.IndianRed) : new SolidColorBrush(Colors.Transparent);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // untuk foreground
    public class PageIndexToColorConverter2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int pageNumber && values[1] is int pageIndex)
            {
                return pageNumber == pageIndex ? new SolidColorBrush(Colors.White) : (SolidColorBrush)new BrushConverter().ConvertFrom("#6c7682");
            }

            return (SolidColorBrush)new BrushConverter().ConvertFrom("#6c7682");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
