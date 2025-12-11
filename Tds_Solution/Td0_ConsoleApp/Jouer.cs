using System;
using System.Collections.Generic;
using System.Linq;

namespace TD1_Morpion
{
    internal class Jouer
    {
        public IPartie partie;
        public List<Joueur> joueurs;
        public IGrille grille;

        public int tour { get; set; } = 0;
        public char symboleActuel { get; private set; }

        private readonly Random _random = new Random();   // <--- random partagé

        public Jouer(IPartie partie, List<Joueur> joueurs, IGrille grille)
        {
            this.partie = partie;
            this.joueurs = joueurs;
            this.grille = grille;
        }

        public string[] jouerTour(List<List<char>> grilleMorpion)
        {
            // Déterminer le joueur courant
            Joueur joueurActuel = (tour % 2 == 0) ? joueurs[0] : joueurs[1];
            symboleActuel = joueurActuel.symboleChoisi;

            if (joueurActuel.estIA)
            {
                // --- Tour de l'IA : choix random d'une case libre ---
                var casesLibres = (from i in Enumerable.Range(0, 3)
                                   from j in Enumerable.Range(0, 3)
                                   where grilleMorpion[i][j] == ' '
                                   select new { i, j }).ToList();

                if (!casesLibres.Any())
                {
                    // Plus aucune case libre
                    return Array.Empty<string>();
                }

                var coup = casesLibres[_random.Next(casesLibres.Count)];

                Console.WriteLine(
                    $"\nTour {tour + 1} - {joueurActuel.nom} (IA, {symboleActuel}) joue en {coup.i} {coup.j}"
                );

                // On renvoie les coordonnées comme si elles venaient de la console
                return new[] { coup.i.ToString(), coup.j.ToString() };
            }
            else
            {
                // --- Tour d'un humain ---
                Console.WriteLine(
                    $"\nTour {tour + 1} - {joueurActuel.nom} ({symboleActuel}). " +
                    "Entrez vos coordonnées (ligne et colonne) séparées par un espace :"
                );

                string? saisie = Console.ReadLine();
                return (saisie ?? string.Empty)
                       .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public bool coupValide(string[] entrees, List<List<char>> grilleMorpion)
        {
            if (entrees.Length != 2 ||
                !int.TryParse(entrees[0], out int ligne) ||
                !int.TryParse(entrees[1], out int colonne) ||
                ligne < 0 || ligne > 2 ||
                colonne < 0 || colonne > 2)
            {
                Console.WriteLine("Coup invalide (format ou coordonnées). Veuillez réessayer.\n");
                return false;
            }

            if (grilleMorpion[ligne][colonne] != ' ')
            {
                Console.WriteLine("Coup invalide : la case est déjà occupée.\n");
                return false;
            }

            // Coup valide → on joue le coup
            grilleMorpion[ligne][colonne] = symboleActuel;
            Console.WriteLine();
            grille.AfficherGrille(grilleMorpion);

            return true;
        }
    }
}
