using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shakalizator.Converters
{
    class ByteToBitmapImageConverter : IValueConverter
    {
        public int DecodePixelHeight { get; set; }

        public int DecodePixelWidth { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try {
                if (targetType != typeof(ImageSource))
                    return null;

                byte[] data = (byte[])value;

                using (MemoryStream ms = new MemoryStream(data)) {
                    BitmapImage img = new BitmapImage();

                    img.BeginInit();
                    img.DecodePixelHeight = DecodePixelHeight;
                    img.DecodePixelWidth = DecodePixelWidth;
                    img.StreamSource = ms;
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();

                    return img;
                }

            }
            catch {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try {
                if (targetType != typeof(byte[]))
                    return null;

                BitmapImage img = (BitmapImage)value;

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                using (MemoryStream ms = new MemoryStream()) {
                    encoder.Save(ms);
                    return ms.ToArray();
                }
            }
            catch {
                return null;
            }
        }
    }
}
