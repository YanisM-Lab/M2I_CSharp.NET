namespace TD_Morpion_MAUI.Navigation;

public sealed class AppNavigator(IServiceProvider services) : IAppNavigator
{
	public void ShowAuthPage()
	{
		SetPage(services.GetRequiredService<AuthPage>());
	}

	public void ShowGamePage()
	{
		SetPage(services.GetRequiredService<MainPage>());
	}

	private static void SetPage(Page page)
	{
		var window = Application.Current?.Windows.FirstOrDefault();
		if (window is not null)
		{
			window.Page = page;
		}
	}
}
