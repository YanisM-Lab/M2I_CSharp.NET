using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion;

public class Partie : IPartie
{
    public bool PartieTerminee(List<List<char>> grille, char symbole)
    {
        bool ligneGagnante = grille.Any(row => row.All(cell => cell == symbole));

        bool colonneGagnante = Enumerable.Range(0, 3)
            .Any(col => Enumerable.Range(0, 3)
                .All(row => grille[row][col] == symbole));

        bool diag1Gagnante = Enumerable.Range(0, 3)
            .All(i => grille[i][i] == symbole);

        bool diag2Gagnante = Enumerable.Range(0, 3)
            .All(i => grille[i][2 - i] == symbole);

        return ligneGagnante || colonneGagnante || diag1Gagnante || diag2Gagnante;
    }

    public bool GrillePleine(List<List<char>> grille)
    {
        return grille.SelectMany(ligne => ligne).All(cellule => cellule != ' ');
    }
}
