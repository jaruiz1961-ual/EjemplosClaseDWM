using BlazorEbi7.Model.Interfaces;
using BlazorEbi7.RestfullCore.Services;
using Microsoft.Extensions.DependencyInjection;

using TodoREST.Views;
using BlazorEbi7.RestfullCore.Repositories;


namespace TodoREST;

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

		builder.Services.AddHttpClient();

		builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();

		var ff = builder.Services.BuildServiceProvider().GetService<IHttpsClientHandlerService>() as IHttpsClientHandlerService;


        builder.Services.AddScoped<IUsuarioServiceAsync>(sp => new UsuarioServiceR

        (ff,"https://localhost:44394/api/", "users"));


		return builder.Build();
	}
}
