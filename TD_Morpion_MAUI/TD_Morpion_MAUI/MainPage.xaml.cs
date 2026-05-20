using TD_Morpion_MAUI.Api;
using TD_Morpion_MAUI.Api.Models;
using TD_Morpion_MAUI.Auth;
using TD_Morpion_MAUI.Navigation;

namespace TD_Morpion_MAUI;

public partial class MainPage : ContentPage
{
	private readonly IMorpionApiClient apiClient;
	private readonly IAuthSession authSession;
	private readonly IAppNavigator navigator;
	private readonly MorpionApiOptions apiOptions;
	private readonly Button[] cells;
	private readonly List<GameResponseDto> games = [];
	private GameResponseDto? currentGame;
	private bool isBusy;
	private bool isLoadingPicker;
	private bool hasLoaded;

	public MainPage(
		IMorpionApiClient apiClient,
		IAuthSession authSession,
		IAppNavigator navigator,
		MorpionApiOptions apiOptions)
	{
		this.apiClient = apiClient;
		this.authSession = authSession;
		this.navigator = navigator;
		this.apiOptions = apiOptions;

		InitializeComponent();

		cells =
		[
			Cell0,
			Cell1,
			Cell2,
			Cell3,
			Cell4,
			Cell5,
			Cell6,
			Cell7,
			Cell8
		];

		for (var index = 0; index < cells.Length; index++)
		{
			cells[index].CommandParameter = index;
			cells[index].FontSize = 42;
			cells[index].FontAttributes = FontAttributes.Bold;
			cells[index].CornerRadius = 8;
			cells[index].BackgroundColor = Color.FromArgb("#FFFFFF");
			cells[index].TextColor = Color.FromArgb("#202124");
		}

		ApiUrlLabel.Text = $"API: {apiOptions.BaseUrl}";
		UpdateUserLabel();
		RenderGame();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (hasLoaded)
		{
			return;
		}

		hasLoaded = true;
		if (!authSession.IsAuthenticated && !await authSession.LoadAsync())
		{
			navigator.ShowAuthPage();
			return;
		}

		UpdateUserLabel();
		await LoadGamesAsync();
	}

	private async void OnNewGameClicked(object? sender, EventArgs e)
	{
		await StartNewGameAsync();
	}

	private async void OnReloadGamesClicked(object? sender, EventArgs e)
	{
		await LoadGamesAsync();
	}

	private void OnLogoutClicked(object? sender, EventArgs e)
	{
		authSession.Clear();
		navigator.ShowAuthPage();
	}

	private void OnGameSelected(object? sender, EventArgs e)
	{
		if (isLoadingPicker || GamesPicker.SelectedIndex < 0 || GamesPicker.SelectedIndex >= games.Count)
		{
			return;
		}

		currentGame = games[GamesPicker.SelectedIndex];
		RenderGame();
	}

	private async void OnCellClicked(object? sender, EventArgs e)
	{
		if (sender is not Button button || button.CommandParameter is not int position)
		{
			return;
		}

		await PlayMoveAsync(position);
	}

	private async Task LoadGamesAsync()
	{
		await RunApiActionAsync(async () =>
		{
			var userGames = await apiClient.GetGamesAsync();
			games.Clear();
			games.AddRange(userGames.OrderByDescending(game => game.UpdatedAtUtc));
			RefreshGamesPicker();

			currentGame = games.FirstOrDefault();
			if (currentGame is null)
			{
				StatusLabel.Text = "Aucune partie";
				DetailsLabel.Text = "Creez une nouvelle partie pour commencer.";
				RenderGame();
				return;
			}

			GamesPicker.SelectedIndex = 0;
			RenderGame();
		});
	}

	private async Task StartNewGameAsync()
	{
		await RunApiActionAsync(async () =>
		{
			currentGame = await apiClient.CreateGameAsync();
			await LoadGamesAsync();
			SelectCurrentGameInPicker();
			RenderGame();
		});
	}

	private async Task PlayMoveAsync(int position)
	{
		if (currentGame is null || currentGame.Status != "InProgress" || isBusy)
		{
			return;
		}

		if (currentGame.Board[position] != '.')
		{
			return;
		}

		await RunApiActionAsync(async () =>
		{
			currentGame = await apiClient.PlayMoveAsync(currentGame.Id, position);
			var existingIndex = games.FindIndex(game => game.Id == currentGame.Id);
			if (existingIndex >= 0)
			{
				games[existingIndex] = currentGame;
				RefreshGamesPicker();
				SelectCurrentGameInPicker();
			}

			RenderGame();
		});
	}

	private async Task RunApiActionAsync(Func<Task> action)
	{
		try
		{
			SetBusy(true);
			await action();
		}
		catch (UnauthorizedAccessException)
		{
			authSession.Clear();
			navigator.ShowAuthPage();
		}
		catch (Exception exception)
		{
			StatusLabel.Text = "Connexion impossible";
			DetailsLabel.Text = exception.Message;
		}
		finally
		{
			SetBusy(false);
		}
	}

	private void RefreshGamesPicker()
	{
		isLoadingPicker = true;
		GamesPicker.Items.Clear();

		foreach (var game in games)
		{
			GamesPicker.Items.Add($"{GetStatusText(game)} - {game.UpdatedAtUtc:dd/MM HH:mm}");
		}

		isLoadingPicker = false;
	}

	private void SelectCurrentGameInPicker()
	{
		if (currentGame is null)
		{
			GamesPicker.SelectedIndex = -1;
			return;
		}

		var index = games.FindIndex(game => game.Id == currentGame.Id);
		GamesPicker.SelectedIndex = index;
	}

	private void RenderGame()
	{
		if (currentGame is null)
		{
			foreach (var cell in cells)
			{
				cell.Text = string.Empty;
				cell.IsEnabled = false;
			}

			return;
		}

		for (var index = 0; index < cells.Length; index++)
		{
			var symbol = currentGame.Board[index];
			cells[index].Text = symbol == '.' ? string.Empty : symbol.ToString();
			cells[index].IsEnabled = !isBusy && currentGame.Status == "InProgress" && symbol == '.';
		}

		StatusLabel.Text = GetStatusText(currentGame);
		DetailsLabel.Text = GetDetailsText(currentGame);
	}

	private void UpdateUserLabel()
	{
		UserLabel.Text = authSession.CurrentUser is null
			? "Non connecte"
			: $"Connecte: {authSession.CurrentUser.UserName} ({authSession.CurrentUser.Email})";
	}

	private static string GetStatusText(GameResponseDto game)
	{
		return game.Status switch
		{
			"InProgress" => "A vous de jouer",
			"XWins" => "Vous avez gagne",
			"OWins" => "Le bot a gagne",
			"Draw" => "Match nul",
			_ => game.Status
		};
	}

	private static string GetDetailsText(GameResponseDto game)
	{
		if (game.Status == "InProgress")
		{
			return $"Vous jouez {game.HumanSymbol}. Le bot joue {game.BotSymbol}.";
		}

		return game.Winner is null
			? "La partie est terminee sans gagnant."
			: $"Gagnant: {game.Winner}.";
	}

	private void SetBusy(bool value)
	{
		isBusy = value;
		foreach (var cell in cells)
		{
			cell.IsEnabled = !value
				&& currentGame?.Status == "InProgress"
				&& string.IsNullOrEmpty(cell.Text);
		}
	}
}
