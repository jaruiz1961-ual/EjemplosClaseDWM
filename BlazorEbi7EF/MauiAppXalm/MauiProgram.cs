
using BlazorEbi9.Data.DataBase;
using BlazorEbi9.Data.Services;
using BlazorEbi9.RestfullCore.Repositories;
using BlazorEbi9.RestfullCore.Services;
using MauiAppXalm.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BlazorEbi9.Model.IServices;

namespace MauiAppXalm
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
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //if (DeviceInfo.Platform == DevicePlatform.Android)
            //{
            //    builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
            //    var HttpsClientHandS = builder.Services.BuildServiceProvider().GetService<IHttpsClientHandlerService>() as IHttpsClientHandlerService;
            //    builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR(HttpsClientHandS, RestUrl));
            //}
            //else
            //    builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR(new HttpClient(), RestUrl));


            var DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "miSqlitedb");

            builder.Services.AddDbContext<SqLiteDbContext>(options => options.UseSqlite($"FileName={DatabasePath}"));
            builder.Services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync<SqLiteDbContext>>();
            

            builder.Services.AddTransient<IUsuarioServiceAsync, UsuarioServiceAsync>();

            builder.Services.AddSingleton<ListaUsuariosPage>();
            builder.Services.AddTransient<DetalleUsuarioPage>();
            return builder.Build();
        }
    }
}
