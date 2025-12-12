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

    public bool PartieTerminee(List<List<char>> grille, char symbole)
    {
        bool ligneGagnante = grille.Any(row => row.All(cell => cell == symbole));

        bool colonneGagnante = Enumerable.Range(0, 3)
                                         .Any(col => Enumerable.Range(0, 3)
                                                             .All(row => grille[row][col] == symbole));

        // Diag (0,0) -> (1,1) -> (2,2)
        bool diag1Gagnante = Enumerable.Range(0, 3)
                                       .All(i => grille[i][i] == symbole);

        // Diag (0,2) -> (1,1) -> (2,0)
        bool diag2Gagnante = Enumerable.Range(0, 3)
                                       .All(i => grille[i][2 - i] == symbole);

        return ligneGagnante || colonneGagnante || diag1Gagnante || diag2Gagnante;
    }

}
