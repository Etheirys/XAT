using System;
using System.Windows.Data;

namespace XAT.Utils.WPF.Converters;

[ValueConversion(typeof(object), typeof(bool))]
public class NotZeroToBoolConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		if (value is int intNum)
		{
			if (intNum != 0)
			{
				return true;
			}
		}

		if (value is long longNum)
		{
			if (longNum != 0)
			{
				return true;
			}
		}

		if (value is short shortNum)
		{
			if (shortNum != 0)
			{
				return true;
			}
		}

		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
