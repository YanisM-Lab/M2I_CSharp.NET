using TD_Morpion_MAUI.Api;
using TD_Morpion_MAUI.Auth;
using TD_Morpion_MAUI.Navigation;

namespace TD_Morpion_MAUI;

public partial class AuthPage : ContentPage
{
	private readonly IAuthApiClient authApiClient;
	private readonly IAuthSession authSession;
	private readonly IAppNavigator navigator;
	private readonly MorpionApiOptions apiOptions;
	private bool isRegisterMode;
	private bool isBusy;
	private bool hasTriedStoredSession;

	public AuthPage(
		IAuthApiClient authApiClient,
		IAuthSession authSession,
		IAppNavigator navigator,
		MorpionApiOptions apiOptions)
	{
		this.authApiClient = authApiClient;
		this.authSession = authSession;
		this.navigator = navigator;
		this.apiOptions = apiOptions;

		InitializeComponent();
		ApiUrlLabel.Text = $"API: {apiOptions.BaseUrl}";
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (hasTriedStoredSession)
		{
			return;
		}

		hasTriedStoredSession = true;
		if (await authSession.LoadAsync())
		{
			navigator.ShowGamePage();
		}
	}

	private async void OnSubmitClicked(object? sender, EventArgs e)
	{
		if (isBusy)
		{
			return;
		}

		await SubmitAsync();
	}

	private void OnSwitchModeClicked(object? sender, EventArgs e)
	{
		isRegisterMode = !isRegisterMode;
		ErrorLabel.IsVisible = false;
		UserNameEntry.IsVisible = isRegisterMode;
		ModeLabel.Text = isRegisterMode ? "Inscription" : "Connexion";
		SubmitButton.Text = isRegisterMode ? "Creer le compte" : "Se connecter";
		SwitchModeButton.Text = isRegisterMode ? "J'ai deja un compte" : "Creer un compte";
	}

	private async Task SubmitAsync()
	{
		try
		{
			SetBusy(true);
			ErrorLabel.IsVisible = false;

			var email = EmailEntry.Text?.Trim() ?? string.Empty;
			var password = PasswordEntry.Text ?? string.Empty;
			var userName = UserNameEntry.Text?.Trim() ?? string.Empty;

			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
			{
				ShowError("Email et mot de passe sont obligatoires.");
				return;
			}

			if (isRegisterMode && string.IsNullOrWhiteSpace(userName))
			{
				ShowError("Le nom d'utilisateur est obligatoire.");
				return;
			}

			var authResponse = isRegisterMode
				? await authApiClient.RegisterAsync(userName, email, password)
				: await authApiClient.LoginAsync(email, password);

			await authSession.SaveAsync(authResponse);
			navigator.ShowGamePage();
		}
		catch (Exception exception)
		{
			ShowError(exception.Message);
		}
		finally
		{
			SetBusy(false);
		}
	}

	private void ShowError(string message)
	{
		ErrorLabel.Text = message;
		ErrorLabel.IsVisible = true;
	}

	private void SetBusy(bool value)
	{
		isBusy = value;
		SubmitButton.IsEnabled = !value;
		SwitchModeButton.IsEnabled = !value;
		SubmitButton.Text = value
			? "Veuillez patienter..."
			: isRegisterMode ? "Creer le compte" : "Se connecter";
	}
}
