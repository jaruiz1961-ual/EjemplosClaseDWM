using BlazorEbi7.RestfullCore.Services;
using Microsoft.Extensions.Logging;
using BlazorEbi7.RestfullCore.Repositories;
using BlazorEbi7.Model.IServices;

namespace BlazorEbi7.MauiBlazorApp
{
    public static class MauiProgram
    {
        public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static string Scheme = "https"; // or http
        public static string Port = "5001";
        public static string RestUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/";
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

           

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
                var HttpsClientHandS = builder.Services.BuildServiceProvider().GetService<IHttpsClientHandlerService>() as IHttpsClientHandlerService;
                builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR(HttpsClientHandS, RestUrl));
            }
            else
                builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR(new HttpClient(),RestUrl));

                return builder.Build();
        }
    }
}
