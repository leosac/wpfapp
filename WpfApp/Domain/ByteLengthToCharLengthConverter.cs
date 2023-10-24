using System;
using System.Globalization;
using System.Windows.Data;

namespace Leosac.WpfApp.Domain
{
    public class ByteLengthToCharLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            var length = System.Convert.ToUInt32(value);
            return length * 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            var length = System.Convert.ToUInt32(value);
            return (uint)(length / 2);
        }
    }
}
