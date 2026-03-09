using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion;

public interface IGrille
{
    List<List<char>> ConstruireGrille();
    void AfficherGrille(List<List<char>> grille);
}
