using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion
{
    internal interface IPartie
    {
        bool PartieTerminee(List<List<char>> grille, char symbole);
    }
}
