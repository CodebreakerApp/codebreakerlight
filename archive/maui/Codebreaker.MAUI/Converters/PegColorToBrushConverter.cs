namespace Codebreaker.MAUI.Converters;

public class PegColorToBrushConverter : IValueConverter
{
    private readonly static Brush s_blackBrush = new SolidColorBrush(Colors.Black);
    private readonly static Brush s_whiteBrush = new SolidColorBrush(Colors.White);
    private readonly static Brush s_blueBrush = new SolidColorBrush(Color.FromArgb("#4f6bed"));
    private readonly static Brush s_emptyBrush = new SolidColorBrush(Color.FromArgb("#a0aeb2"));

    public Brush BlackBrush { get; set; } = s_blackBrush;
    public Brush WhiteBrush { get; set; } = s_whiteBrush;
    public Brush BlueBrush { get; set; } = s_blueBrush;
    private Brush EmptyBrush { get; set; } = s_emptyBrush;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        if (value is not string guessPeg)
            return null;

        return guessPeg switch
        {
            "Black" => BlackBrush,
            "White" => WhiteBrush,
            "Blue" => BlueBrush,
            _ => EmptyBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
