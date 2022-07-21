using System;
using System.Windows.Data;

namespace XAT.UI.Utils.Converters;

[ValueConversion(typeof(object), typeof(bool))]
public class NotNullToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                value = null;
            }
        }

        return value != null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
