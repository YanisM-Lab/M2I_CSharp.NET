using System;
using System.Collections.Generic;
using System.Text;

namespace TD1_Morpion;

internal class Jouer
{
    public IPartie partie;
    public List<Joueur> joueurs;
    public IGrille grille;

    public Jouer(IPartie partie, List<Joueur> joueurs, IGrille grille)
    {
        this.partie = partie;
        this.joueurs = joueurs;
        this.grille = grille;
    }

    public void jouerTour()
    {
        int tour = 0;
        EtatPartie etatPartie = EtatPartie.EnCours;
        List<List<char>> grilleMorpion = grille.ConstruireGrille();

        while (etatPartie == EtatPartie.EnCours)
        {
            char symboleActuel = (tour % 2 == 0) ? joueurs[0].symboleChoisi : joueurs[1].symboleChoisi;
            Console.WriteLine($"\nTour du joueur {(tour % 2) + 1} ({symboleActuel}). Entrez vos coordonnées (ligne et colonne) séparées par un espace :");

            string[] entrees = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (entrees.Length != 2 ||
                !int.TryParse(entrees[0], out int ligne) ||
                !int.TryParse(entrees[1], out int colonne) ||
                ligne < 0 || ligne > 2 || colonne < 0 || colonne > 2 ||
                grilleMorpion[ligne][colonne] != ' ')
            {
                Console.WriteLine("Coup invalide. Veuillez réessayer.\n");
                continue;
            }

            Console.WriteLine("\n");
            grilleMorpion[ligne][colonne] = symboleActuel;
            grille.AfficherGrille(grilleMorpion);

            if (partie.PartieTerminee(grilleMorpion, symboleActuel))
            {
                etatPartie = EtatPartie.Gagne;
                Console.WriteLine($"\nLe joueur {(tour % 2) + 1} ({symboleActuel}) a gagné !\n");
            }
            else if (tour == 8) // 9 cases jouées (0 à 8)
            {
                etatPartie = EtatPartie.Nul;
                Console.WriteLine("\nLa partie est nulle !\n");
            }
            else
            {
                tour++;
            }
        }

    }
}
