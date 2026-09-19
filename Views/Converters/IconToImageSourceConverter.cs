using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace ProcessManagerApp.Views
{
    public class IconToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            try
            {
                var icon = (Icon)value;
                var bitmap = icon.ToBitmap();
                
                using (var memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    
                    return bitmapImage;
                }
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
