using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProcessManagerApp.Views
{
    public class CriticalToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            var isCritical = (bool)value;
            var parameterString = parameter as string;

            // Si le paramètre est "Inverse", on inverse la logique
            if (parameterString == "Inverse")
            {
                return isCritical ? Visibility.Collapsed : Visibility.Visible;
            }

            return isCritical ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
