using MauiMvvmDemo.Pages;
using MauiMvvmDemo.ViewModels.Pages;
using Microsoft.Extensions.Logging;

namespace MauiMvvmDemo;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services.AddScoped<MainPageVM>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
