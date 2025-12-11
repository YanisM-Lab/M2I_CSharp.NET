using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion
{
    public interface IPartie
    {
        public bool PartieTerminee(List<List<char>> grille, char symbole);
    }
}
