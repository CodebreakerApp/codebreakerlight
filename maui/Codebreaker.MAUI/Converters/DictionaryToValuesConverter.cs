namespace Codebreaker.MAUI.Converters;
public class DictionaryToValuesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string fieldType)
            throw new ArgumentException("The parameter needs to be a string");

        if (value is not IDictionary<string, string[]> valuesDictionary)
            throw new ArgumentException("The value needs to be a dictionary");

        return valuesDictionary[fieldType];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
