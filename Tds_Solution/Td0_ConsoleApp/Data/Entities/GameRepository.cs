using Microsoft.EntityFrameworkCore;
using TD1_Morpion.Data.Entities;

namespace TD1_Morpion.Data;

public class GameRepository
{
    private readonly MorpionDbContext _context;

    public GameRepository(MorpionDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateGameAsync(
        string player1Name,
        string player2Name,
        char player1Symbol,
        char player2Symbol,
        bool isPlayer2Bot,
        List<List<char>> board,
        int currentTurn,
        string currentPlayerName)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Player1Name = player1Name,
            Player2Name = player2Name,
            Player1Symbol = player1Symbol,
            Player2Symbol = player2Symbol,
            IsPlayer2Bot = isPlayer2Bot,
            BoardJson = BoardSerializer.Serialize(board),
            CurrentTurn = currentTurn,
            CurrentPlayerName = currentPlayerName,
            Status = "EnCours",
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return game.Id;
    }

    public async Task SaveMoveAndStateAsync(
        Guid gameId,
        int turnNumber,
        string playerName,
        char symbol,
        int row,
        int column,
        List<List<char>> board,
        string status,
        string currentPlayerName,
        string? winnerName)
    {
        var game = await _context.Games.FindAsync(gameId);
        if (game is null)
            throw new InvalidOperationException("Partie introuvable.");

        _context.Moves.Add(new Move
        {
            GameId = gameId,
            TurnNumber = turnNumber,
            PlayerName = playerName,
            Symbol = symbol,
            Row = row,
            Column = column,
            PlayedAtUtc = DateTime.UtcNow
        });

        game.BoardJson = BoardSerializer.Serialize(board);
        game.CurrentTurn = turnNumber;
        game.CurrentPlayerName = currentPlayerName;
        game.Status = status;
        game.WinnerName = winnerName;

        if (status is "Gagne" or "Nul")
        {
            game.FinishedAtUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Game?> GetLatestUnfinishedGameAsync(string playerName)
    {
        string normalizedName = playerName.Trim();

        return await _context.Games
            .Where(g => (g.Player1Name == normalizedName || g.Player2Name == normalizedName) && g.Status == "EnCours")
            .OrderByDescending(g => g.CreatedAtUtc)
            .FirstOrDefaultAsync();
    }

    public async Task<GameStats> GetStatsAsync(string humanPlayerName)
    {
        string normalizedName = humanPlayerName.Trim();

        int playedGames = await _context.Games.CountAsync(g =>
            g.Player1Name == normalizedName &&
            g.IsPlayer2Bot &&
            g.Status != "EnCours");

        int humanWins = await _context.Games.CountAsync(g =>
            g.Player1Name == normalizedName &&
            g.IsPlayer2Bot &&
            g.Status == "Gagne" &&
            g.WinnerName == g.Player1Name);

        int botWins = await _context.Games.CountAsync(g =>
            g.Player1Name == normalizedName &&
            g.IsPlayer2Bot &&
            g.Status == "Gagne" &&
            g.WinnerName == g.Player2Name);

        double ratio = botWins == 0
            ? humanWins
            : (double)humanWins / botWins;

        return new GameStats
        {
            PlayedGamesAgainstBot = playedGames,
            HumanWinsAgainstBot = humanWins,
            BotWinsAgainstBot = botWins,
            HumanBotRatio = ratio
        };
    }
}
