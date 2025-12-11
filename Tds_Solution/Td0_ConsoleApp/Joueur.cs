using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion
{
    public class Joueur : IJoueur
    {
        public int numeroJoueur;
        public char symboleChoisi;
        
        public Joueur(int numeroJoueur)
        {
            this.numeroJoueur = numeroJoueur;
            this.symboleChoisi = ' ';
        }
        public void choisirSymbole()
        {
            Console.WriteLine("Veuillez choisir votre symbole {X, O}");
            try
            {
                this.symboleChoisi = Console.ReadLine().ToUpper()[0];
            }
            catch
            {
                Console.WriteLine("Symbole invalide. Veuillez choisir un symbole valide : {X, O}\n\n");
            }
        }
    }
}
