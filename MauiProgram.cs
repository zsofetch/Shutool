using Microsoft.Extensions.Logging;
using Shutool.Services;
using Shutool.ViewModels;
using Shutool.Views;
using Supabase;

namespace Shutool
{
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

            // 1. Define your Supabase Credentials
            var url = "url"; // Replace with your URL
            var key = "key";    // Replace with your Key

            // 2. Configure Supabase Options
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            // 3. Register the Supabase Client as a Singleton
            builder.Services.AddSingleton(provider => new Client(url, key, options));

            // 4. Register your Custom Service
            builder.Services.AddSingleton<SupabaseService>();

            // (Optional) Register Pages and ViewModels here later
            // builder.Services.AddTransient<LoginPage>();
            // builder.Services.AddTransient<LoginViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<SignUpPage>();

            builder.Services.AddTransient<RiderMainViewModel>();
            builder.Services.AddTransient<RiderMainPage>();
#endif
            builder.Services.AddTransient<DriverPendingViewModel>();
            builder.Services.AddTransient<DriverPendingPage>();

            builder.Services.AddTransient<RiderHistoryViewModel>();
            builder.Services.AddTransient<RiderHistoryPage>();

            builder.Services.AddTransient<DriverHistoryViewModel>();
            builder.Services.AddTransient<DriverHistoryPage>();

            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ProfilePage>();

            builder.Services.AddTransient<AdminManageViewModel>();
            builder.Services.AddTransient<AdminManagePage>();

            builder.Services.AddTransient<AdminRequestLogViewModel>();
            builder.Services.AddTransient<AdminRequestLogPage>();
            return builder.Build();
        }
    }
}