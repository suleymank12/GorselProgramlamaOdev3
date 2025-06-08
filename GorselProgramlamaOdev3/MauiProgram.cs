using Microsoft.Extensions.Logging;

namespace GorselProgramlamaOdev3;

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
        // Servisleri kaydet
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<AnaSayfa>();
        builder.Services.AddSingleton<DovizKurlari>();
        builder.Services.AddSingleton<Haberler>();
        builder.Services.AddSingleton<HaberDetay>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
