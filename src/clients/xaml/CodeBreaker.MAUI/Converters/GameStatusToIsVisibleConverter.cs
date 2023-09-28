using Codebreaker.ViewModels;
using System.Globalization;

namespace CodeBreaker.MAUI.Converters;

public class GameStatusToIsVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        static bool GetStartVisibility(GameMode gameMode) =>
            gameMode is not (GameMode.Started or GameMode.MoveSet);

        static bool GetRunningVisibility(GameMode gameMode) =>
            !(gameMode == GameMode.NotRunning);

        static bool GetCancelVisibility(GameMode gameMode) =>
            (gameMode is GameMode.Started or GameMode.MoveSet);

        string uiCategory = parameter?.ToString() ?? throw new InvalidOperationException("Pass a parameter to this converter");


        if (value is GameMode gameMode)
        {
            return uiCategory switch
            {
                "Start" => GetStartVisibility(gameMode),
                "Running" => GetRunningVisibility(gameMode),
                "Cancelable" => GetCancelVisibility(gameMode),
                _ => throw new InvalidOperationException("Invalid parameter value")
            };
        }
        else
        {
            throw new InvalidOperationException("GameStatusToVisibilityConverter received an incorrect value type");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
