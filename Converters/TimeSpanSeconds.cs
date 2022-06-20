using System;
using System.Globalization;
using System.Windows.Data;

namespace Salon.Converters
{
    class TimeSpanSeconds : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int i ? TimeSpan.FromSeconds(i) : new TimeSpan();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TimeSpan t ? t.TotalSeconds : 0;
        }
    }
}
