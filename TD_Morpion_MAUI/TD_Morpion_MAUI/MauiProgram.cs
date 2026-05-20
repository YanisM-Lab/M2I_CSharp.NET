using Microsoft.Extensions.Logging;
using TD_Morpion_MAUI.Api;
using TD_Morpion_MAUI.Auth;
using TD_Morpion_MAUI.Navigation;

namespace TD_Morpion_MAUI;

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

		builder.Services.AddSingleton(new MorpionApiOptions
		{
			BaseUrl = GetApiBaseUrl()
		});
		builder.Services.AddSingleton(sp =>
		{
			var options = sp.GetRequiredService<MorpionApiOptions>();
			return new HttpClient
			{
				BaseAddress = new Uri(options.BaseUrl)
			};
		});
		builder.Services.AddSingleton<IMorpionApiClient, MorpionApiClient>();
		builder.Services.AddSingleton<IAuthApiClient, AuthApiClient>();
		builder.Services.AddSingleton<IAuthSession, AuthSession>();
		builder.Services.AddSingleton<IAppNavigator, AppNavigator>();
		builder.Services.AddTransient<AuthPage>();
		builder.Services.AddTransient<MainPage>();

		return builder.Build();
	}

	private static string GetApiBaseUrl()
	{
#if ANDROID
		return "http://10.0.2.2:5013";
#else
		return "http://localhost:5013";
#endif
	}
}
