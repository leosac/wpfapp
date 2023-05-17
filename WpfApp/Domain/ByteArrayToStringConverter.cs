using System;
using System.Globalization;
using System.Windows.Data;

namespace Leosac.WpfApp.Domain
{
    public class ByteArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is byte[] v)
                return System.Convert.ToHexString(v);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is string v)
                return System.Convert.FromHexString(v);

            return Binding.DoNothing;
        }
    }
}
