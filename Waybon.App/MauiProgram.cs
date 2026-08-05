using Microsoft.Extensions.Logging;
using Waybon.App.Services.Implementations;
using Waybon.App.Services.Interfaces;
using Waybon.App.ViewModels;
using Waybon.App.Views;
using CommunityToolkit.Maui;

namespace Waybon.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>();
            builder.UseMauiCommunityToolkit();
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
            builder.Services.AddSingleton<ISessionService, SessionService>();
            builder.Services.AddSingleton<ISigningConfiguration, SigningConfiguration>();

            builder.Services.AddTransient<SigningHandler>();
            builder.Services.AddHttpClient<IAuthService, AuthService>
            (
                client =>
                {
                    client.BaseAddress = new Uri("https://waybon-api.onrender.com/");
                }
            )
            .AddHttpMessageHandler<SigningHandler>();

            builder.Services.AddHttpClient<IGroupService, GroupService>
            (
                client =>
                {
                    client.BaseAddress = new Uri("https://waybon-api.onrender.com/");
                }
            )
            .AddHttpMessageHandler<SigningHandler>();

            // --- ViewModels ---
            builder.Services.AddTransient<LoadingViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<GroupViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            

            // --- Views ---
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}