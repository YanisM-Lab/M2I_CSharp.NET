using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion.Data.Entities;

public class GameStats
{
    public int PlayedGamesAgainstBot { get; set; }
    public int HumanWinsAgainstBot { get; set; }
    public int BotWinsAgainstBot { get; set; }
    public double HumanBotRatio { get; set; }
}
