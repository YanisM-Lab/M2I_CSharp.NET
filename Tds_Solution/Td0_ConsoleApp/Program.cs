using System;
using System.Collections.Generic;
using TD1_Morpion;

namespace Td0_ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IPartie partie = new Partie();
            IGrille grille = new Grille();

            Console.WriteLine("Voulez-vous jouer contre l'ordinateur ? (O/N)");
            string? reponse = Console.ReadLine();
            bool contreIA = !string.IsNullOrWhiteSpace(reponse) &&
                            char.ToUpperInvariant(reponse[0]) == 'O';

            string? joueur1Nom;
            string? joueur2Nom;

            Console.WriteLine("Nom du joueur 1 :");
            joueur1Nom = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(joueur1Nom)) joueur1Nom = "Joueur 1";

            List<Joueur> joueurs;

            if (contreIA)
            {
                // Mode joueur vs IA
                joueur2Nom = "Ordinateur";

                joueurs = new List<Joueur>
                {
                    new Joueur(joueur1Nom, estIA: false),
                    new Joueur(joueur2Nom, estIA: true)
                };
            }
            else
            {
                // Mode joueur vs joueur
                Console.WriteLine("Nom du joueur 2 :");
                joueur2Nom = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(joueur2Nom)) joueur2Nom = "Joueur 2";

                joueurs = new List<Joueur>
                {
                    new Joueur(joueur1Nom, estIA: false),
                    new Joueur(joueur2Nom, estIA: false)
                };
            }

            joueurs[0].choisirSymbole();
            joueurs[1].symboleChoisi = (joueurs[0].symboleChoisi == 'X') ? 'O' : 'X';

            Console.WriteLine($"{joueurs[0].nom} jouera {joueurs[0].symboleChoisi}");
            Console.WriteLine($"{joueurs[1].nom} jouera {joueurs[1].symboleChoisi}\n");

            // Initialisation de la grille
            List<List<char>> grilleMorpion = grille.ConstruireGrille();
            grille.AfficherGrille(grilleMorpion);

            Jouer jouer = new Jouer(partie, joueurs, grille);

            EtatPartie etatPartie = EtatPartie.EnCours;
            string[] coordonneesSaisies;

            while (etatPartie == EtatPartie.EnCours)
            {
                coordonneesSaisies = jouer.jouerTour(grilleMorpion);

                if (!jouer.coupValide(coordonneesSaisies, grilleMorpion))
                {
                    // L'IA choisit toujours une case libre
                    continue;
                }

                if (partie.PartieTerminee(grilleMorpion, jouer.symboleActuel))
                {
                    etatPartie = EtatPartie.Gagne;

                    int indexJoueurGagnant = jouer.tour % 2;
                    Joueur gagnant = joueurs[indexJoueurGagnant];

                    Console.WriteLine($"\nLe joueur {gagnant.nom} ({gagnant.symboleChoisi}) a gagné !\n");
                }
                else if (jouer.tour == 8) // 9 cases jouées (0 à 8)
                {
                    etatPartie = EtatPartie.Nul;
                    Console.WriteLine("\nLa partie est nulle !\n");
                }
                else
                {
                    jouer.tour++;
                }//
            }

            Console.WriteLine("Fin de la partie.");
        }
    }
}
