using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion;

public interface IPartie
{
    bool PartieTerminee(List<List<char>> grille, char symbole);
    bool GrillePleine(List<List<char>> grille);
}
