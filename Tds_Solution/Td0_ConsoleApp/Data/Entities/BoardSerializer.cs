using System;
using System.Collections.Generic;
using System.Text;

using System.Text.Json;

namespace TD1_Morpion.Data;

public static class BoardSerializer
{
    public static string Serialize(List<List<char>> board)
    {
        return JsonSerializer.Serialize(board);
    }

    public static List<List<char>> Deserialize(string json)
    {
        return JsonSerializer.Deserialize<List<List<char>>>(json)
               ?? throw new InvalidOperationException("Impossible de désérialiser la grille.");
    }
}
