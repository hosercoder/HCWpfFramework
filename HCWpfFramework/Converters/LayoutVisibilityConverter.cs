using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HCWpfFramework.Models;

namespace HCWpfFramework.Converters
{
    /// <summary>
    /// Converter to control layout visibility based on current layout type
    /// </summary>
    public class LayoutVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine($"LayoutVisibilityConverter: Convert called - Value: {value}, Parameter: {parameter}");
            
            if (value is LayoutType currentLayout && parameter is string targetLayoutString)
            {
                // Handle both string and integer parameters
                if (Enum.TryParse<LayoutType>(targetLayoutString, out var targetLayout))
                {
                    var isVisible = currentLayout == targetLayout;
                    var result = isVisible ? Visibility.Visible : Visibility.Collapsed;
                    System.Diagnostics.Debug.WriteLine($"LayoutVisibilityConverter: {currentLayout} == {targetLayout} ? {isVisible} -> {result}");
                    return result;
                }
                
                // Try parsing as integer
                if (int.TryParse(targetLayoutString, out var layoutInt))
                {
                    var intLayout = (LayoutType)layoutInt;
                    var isVisible = currentLayout == intLayout;
                    var result = isVisible ? Visibility.Visible : Visibility.Collapsed;
                    System.Diagnostics.Debug.WriteLine($"LayoutVisibilityConverter: {currentLayout} == {intLayout} ? {isVisible} -> {result}");
                    return result;
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"LayoutVisibilityConverter: No match found, returning Collapsed");
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}