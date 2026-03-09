using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion.Data.Entities;

public class Move
{
    public int Id { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public int TurnNumber { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public char Symbol { get; set; }

    public int Row { get; set; }
    public int Column { get; set; }

    public DateTime PlayedAtUtc { get; set; }
}
