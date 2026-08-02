using Microsoft.Extensions.Logging;
using Waybon.App.Services.Implementations;
using Waybon.App.Services.Interfaces;
using Waybon.App.ViewModels;
using Waybon.App.Views;

namespace Waybon.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>();
            builder.ConfigureFonts
            (
                fonts =>
                {
                    fonts.AddFont("PTSerif-Regular.ttf", "PTSerifRegular");
                    fonts.AddFont("PTSerif-Bold.ttf", "PTSerifBold");
                }
            );

            // --- Singletons ---
            builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();

            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://waybon-api.onrender.com/");
            });

            // --- ViewModels ---
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            // --- Vistas ---
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ProfilePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}