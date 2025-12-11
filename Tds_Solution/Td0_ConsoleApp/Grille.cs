using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion;

public class Grille : IGrille
{
    public Grille() { }

    public List<List<char>> ConstruireGrille()
    {
       return Enumerable.Range(0, 3)
                         .Select(_ => Enumerable.Repeat(' ', 3).ToList())
                         .ToList();
    }

    public void AfficherGrille(List<List<char>> grille)
    {
        int limiteGrilleLigne = 3;
        int limiteGrilleColonne = 3;
        Console.WriteLine("  0 1 2");
        for (int i = 0; i < limiteGrilleLigne; i++)
        {
            Console.Write(i + " ");
            for (int j = 0; j < limiteGrilleColonne; j++)
            {
                Console.Write(grille[i][j]);
                if (j < 2) Console.Write("|");
            }
            Console.WriteLine();
            if (i < 2) Console.WriteLine("  -----");
        }
        Console.WriteLine("\n");
    }

}
