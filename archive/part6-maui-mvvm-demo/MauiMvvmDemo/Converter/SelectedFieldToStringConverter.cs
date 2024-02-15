using MauiMvvmDemo.ViewModels.Models;
using System.Globalization;

namespace MauiMvvmDemo.Converter;

public class SelectedFieldToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IEnumerable<SelectedField> fields)
            return Enumerable.Empty<string>();

        return fields.Select(field => field.Color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
