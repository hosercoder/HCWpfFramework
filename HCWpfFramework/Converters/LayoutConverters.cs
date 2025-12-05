using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HCWpfFramework.Models;

namespace HCWpfFramework.Converters
{
    public static class Converters
    {
        public static readonly IValueConverter LayoutToVisibilityConverter = new LayoutToVisibilityConverterImpl();
    }

    public class LayoutToVisibilityConverterImpl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LayoutType currentLayout && parameter is LayoutType targetLayout)
            {
                return currentLayout == targetLayout ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}