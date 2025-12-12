using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion;

public interface IGrille
{
    public List<List<char>> ConstruireGrille();
    public void AfficherGrille(List<List<char>> grille);
    public bool PartieTerminee(List<List<char>> grille, char symbole);
}
