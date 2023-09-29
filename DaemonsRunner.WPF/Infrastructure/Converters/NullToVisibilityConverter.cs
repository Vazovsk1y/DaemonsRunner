using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace DaemonsRunner.WPF.Infrastructure.Converters;

public class NullToVisibilityConverter : IValueConverter
{
	public static NullToVisibilityConverter Instance { get; } = new();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is null)
		{
			return Visibility.Collapsed;
		}

		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
