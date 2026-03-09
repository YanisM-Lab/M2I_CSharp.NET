using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion.Data.Entities;

public class Game
{
    public Guid Id { get; set; }

    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;

    public char Player1Symbol { get; set; }
    public char Player2Symbol { get; set; }

    public bool IsPlayer2Bot { get; set; }

    public string BoardJson { get; set; } = string.Empty;
    public int CurrentTurn { get; set; }
    public string CurrentPlayerName { get; set; } = string.Empty;

    public string Status { get; set; } = "EnCours";
    public string? WinnerName { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? FinishedAtUtc { get; set; }

    public List<Move> Moves { get; set; } = new();
}
