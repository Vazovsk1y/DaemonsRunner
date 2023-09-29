using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace DaemonsRunner.WPF.Infrastructure.Converters;

public class CollectionCountToVisibilityConverter : IValueConverter
{
	public static CollectionCountToVisibilityConverter Instance { get; } = new CollectionCountToVisibilityConverter();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is int count && count > 0)
		{
			return Visibility.Collapsed; 
		}
		else
		{
			return Visibility.Visible; 
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
