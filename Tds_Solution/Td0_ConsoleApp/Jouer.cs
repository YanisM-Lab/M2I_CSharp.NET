using System;
using System.Collections.Generic;
using System.Linq;

namespace TD1_Morpion;

public class Jouer
{
    public List<Joueur> joueurs;
    public IGrille grille;

    public int tour { get; set; } = 0;
    public char symboleActuel { get; set; }

    private readonly Random _random = new();

    public Jouer(List<Joueur> joueurs, IGrille grille)
    {
        this.joueurs = joueurs;
        this.grille = grille;
    }

    public Joueur ObtenirJoueurActuel()
    {
        return (tour % 2 == 0) ? joueurs[0] : joueurs[1];
    }

    public string[] jouerTour(List<List<char>> grilleMorpion)
    {
        Joueur joueurActuel = ObtenirJoueurActuel();
        symboleActuel = joueurActuel.symboleChoisi;

        if (joueurActuel.estIA)
        {
            var casesLibres = (from i in Enumerable.Range(0, 3)
                               from j in Enumerable.Range(0, 3)
                               where grilleMorpion[i][j] == ' '
                               select new { i, j }).ToList();

            if (!casesLibres.Any())
            {
                return Array.Empty<string>();
            }

            var coup = casesLibres[_random.Next(casesLibres.Count)];

            Console.WriteLine($"\nTour {tour + 1} - {joueurActuel.nom} (IA, {symboleActuel}) réfléchit...");
            Task.Delay(3000).Wait();
            Console.WriteLine($"{joueurActuel.nom} joue en {coup.i} {coup.j}");

            return new[] { coup.i.ToString(), coup.j.ToString() };
        }

        Console.WriteLine(
            $"\nTour {tour + 1} - {joueurActuel.nom} ({symboleActuel}). " +
            "Entrez vos coordonnées (ligne et colonne) séparées par un espace :"
        );

        string? saisie = Console.ReadLine();
        return (saisie ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
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

        grilleMorpion[ligne][colonne] = symboleActuel;
        Console.WriteLine();
        grille.AfficherGrille(grilleMorpion);

        return true;
    }

    public static (int ligne, int colonne) ParserCoordonnees(string[] entrees)
    {
        return (int.Parse(entrees[0]), int.Parse(entrees[1]));
    }
}