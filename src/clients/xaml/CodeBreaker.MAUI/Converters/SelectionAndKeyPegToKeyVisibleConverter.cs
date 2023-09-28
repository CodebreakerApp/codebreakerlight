using Codebreaker.ViewModels;
using System.Globalization;

namespace CodeBreaker.MAUI.Converters;

internal class SelectionAndKeyPegToKeyVisibleConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return null;
        ArgumentNullException.ThrowIfNull(parameter);
        
        if (value is SelectionAndKeyPegs selection && int.TryParse(parameter.ToString(), out int ix ))
            return ix < selection.KeyPegs.Length;

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
