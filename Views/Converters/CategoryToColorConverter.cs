using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProcessManagerApp.Models;

namespace ProcessManagerApp.Views
{
    public class CategoryToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;

            var category = (ProcessCategory)value;

            switch (category)
            {
                case ProcessCategory.System:
                    return Brushes.LightPink;
                case ProcessCategory.Application:
                    return Brushes.LightGreen;
                case ProcessCategory.Background:
                    return Brushes.LightBlue;
                case ProcessCategory.Service:
                    return Brushes.LightYellow;
                case ProcessCategory.Unknown:
                    return Brushes.LightGray;
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
