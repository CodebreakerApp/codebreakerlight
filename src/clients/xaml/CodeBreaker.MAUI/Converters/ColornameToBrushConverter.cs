using System.Globalization;
using static CodeBreaker.Shared.Colors;

namespace CodeBreaker.MAUI.Converters;

public class ColornameToBrushConverter : IValueConverter
{
    private readonly static Brush s_blackBrush = new SolidColorBrush(Colors.Black);
    private readonly static Brush s_whiteBrush = new SolidColorBrush(Colors.White);
    private readonly static Brush s_redBrush = new SolidColorBrush(Color.FromArgb("#d13438"));
    private readonly static Brush s_greenBrush = new SolidColorBrush(Color.FromArgb("#00ad56"));
    private readonly static Brush s_blueBrush = new SolidColorBrush(Color.FromArgb("#4f6bed"));
    private readonly static Brush s_yellowBrush = new SolidColorBrush(Color.FromArgb("#fce100"));
    private readonly static Brush s_emptyBrush = new SolidColorBrush(Color.FromArgb("#a0aeb2"));

    public Brush BlackBrush { get; set; } = s_blackBrush;
    public Brush WhiteBrush { get; set; } = s_whiteBrush;
    public Brush RedBrush { get; set; } = s_redBrush;
    public Brush GreenBrush { get; set; } = s_greenBrush;
    public Brush BlueBrush { get; set; } = s_blueBrush;
    public Brush YellowBrush { get; set; } = s_yellowBrush;
    private Brush EmptyBrush { get; set; } = s_emptyBrush;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // with .NET MAUI (contrary to other XAML technologies,
        // the converter is invoked many times,
        // often with null values, but also with the MainPage passed as value
        if (value is null)
            return null;

        if (value is not string guessPeg)
            return null;

        return guessPeg switch
        {
            Black => BlackBrush,
            White => WhiteBrush,
            Red => RedBrush,
            Green => GreenBrush,
            Blue => BlueBrush,
            Yellow => YellowBrush,
            _ => EmptyBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
