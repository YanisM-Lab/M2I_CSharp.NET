using TD_Morpion_MAUI.Api;
using TD_Morpion_MAUI.Api.Models;

namespace TD_Morpion_MAUI;

public partial class MainPage : ContentPage
{
	private readonly IMorpionApiClient apiClient;
	private readonly MorpionApiOptions apiOptions;
	private readonly Button[] cells;
	private GameResponseDto? currentGame;
	private bool isBusy;

	public MainPage(IMorpionApiClient apiClient, MorpionApiOptions apiOptions)
	{
		this.apiClient = apiClient;
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
		_ = StartNewGameAsync();
	}

	private async void OnNewGameClicked(object? sender, EventArgs e)
	{
		await StartNewGameAsync();
	}

	private async void OnCellClicked(object? sender, EventArgs e)
	{
		if (sender is not Button button || button.CommandParameter is not int position)
		{
			return;
		}

		await PlayMoveAsync(position);
	}

	private async Task StartNewGameAsync()
	{
		await RunApiActionAsync(async () =>
		{
			currentGame = await apiClient.CreateGameAsync();
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

	private void RenderGame()
	{
		if (currentGame is null)
		{
			return;
		}

		for (var index = 0; index < cells.Length; index++)
		{
			var symbol = currentGame.Board[index];
			cells[index].Text = symbol == '.' ? string.Empty : symbol.ToString();
			cells[index].IsEnabled = currentGame.Status == "InProgress" && symbol == '.';
		}

		StatusLabel.Text = GetStatusText(currentGame);
		DetailsLabel.Text = GetDetailsText(currentGame);
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
			cell.IsEnabled = !value && currentGame?.Status == "InProgress" && string.IsNullOrEmpty(cell.Text);
		}
	}
}
