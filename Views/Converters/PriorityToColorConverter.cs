using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProcessManagerApp.Models;

namespace ProcessManagerApp.Views
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;

            var priority = (ProcessPriority)value;

            switch (priority)
            {
                case ProcessPriority.Idle:
                    return Brushes.LightGray;
                case ProcessPriority.BelowNormal:
                    return Brushes.LightBlue;
                case ProcessPriority.Normal:
                    return Brushes.LightGreen;
                case ProcessPriority.AboveNormal:
                    return Brushes.LightYellow;
                case ProcessPriority.High:
                    return Brushes.LightSalmon;
                case ProcessPriority.Realtime:
                    return Brushes.LightCoral;
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
