using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion
{
    internal class Partie : IPartie
    {
        public Partie() { }

        static bool PartieTerminee(List<List<char>> grille, char symbole)
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
}
