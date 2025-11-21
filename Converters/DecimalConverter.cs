// filename: F:\CabinetDoc Pro\Converters\DecimalConverter.cs

using System;
using System.Globalization;
using System.Windows.Data;

namespace CabinetDocProWpf.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("N2", CultureInfo.InvariantCulture);
            }
            return "0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                stringValue = stringValue.Replace(" ", "").Replace(",", ".");

                if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }
            }
            return 0m;
        }
    }
}
