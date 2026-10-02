using System;
using System.Globalization;
using System.Windows.Data;

namespace ProcessManagerApp.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Oui" : "Non";
            }
            return "Non";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return strValue.Equals("Oui", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
