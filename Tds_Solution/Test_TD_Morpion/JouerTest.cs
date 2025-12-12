using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using TD1_Morpion;
using Xunit;

public class JouerTest
{
    private List<List<char>> CreerGrilleVide()
    {
        return new List<List<char>>
        {
            new List<char> { ' ', ' ', ' ' },
            new List<char> { ' ', ' ', ' ' },
            new List<char> { ' ', ' ', ' ' }
        };
    }

    [Fact]
    public void IA_Joue_Un_Coup_Valide()
    {
        // ARRANGE
        var grille = new Mock<IGrille>();

        var joueurHumain = new Joueur("Humain");
        var joueurIA = new Joueur("IA", estIA: true);

        var joueurs = new List<Joueur> { joueurHumain, joueurIA };

        Jouer jouer = new Jouer(joueurs, grille.Object);

        List<List<char>> grilleMorpion = CreerGrilleVide();

        jouer.tour = 1; // c'est l'IA qui joue

        // ACT
        string[] result = jouer.jouerTour(grilleMorpion);

        // ASSERT
        Assert.Equal(2, result.Length);
        Assert.True(int.TryParse(result[0], out int i));
        Assert.True(int.TryParse(result[1], out int j));
        Assert.InRange(i, 0, 2);
        Assert.InRange(j, 0, 2);
    }

    [Fact]
    public void Humain_Saisie_Coup_Correct()
    {
        // ARRANGE
        var grille = new Mock<IGrille>();

        var joueurHumain = new Joueur("Humain");
        var joueurIA = new Joueur("IA", estIA: true);

        var joueurs = new List<Joueur> { joueurHumain, joueurIA };

        Jouer jouer = new Jouer(joueurs, grille.Object);

        List<List<char>> grilleMorpion = CreerGrilleVide();

        jouer.tour = 0; //c'est l'humain qui joue

        // Simuler saisie utilisateur
        Console.SetIn(new StringReader("1 2"));

        // ACT
        var result = jouer.jouerTour(grilleMorpion);

        // ASSERT
        Assert.Equal(new[] { "1", "2" }, result);
    }

    [Fact]
    public void CoupValide_Refuse_Coup_En_Dehors_Grille()
    {
        var grille = new Mock<IGrille>();

        var joueurs = new List<Joueur>
        {
            new Joueur("J1"),
            new Joueur("J2")
        };

        Jouer jouer = new Jouer(joueurs, grille.Object);
        jouer.symboleActuel = 'X';

        var grid = CreerGrilleVide();

        // ACT
        bool resultat = jouer.coupValide(new[] { "10", "5" }, grid);

        // ASSERT
        Assert.False(resultat);
    }

    [Fact]
    public void CoupValide_Refuse_Case_Deja_Occupee()
    {
        var grille = new Mock<IGrille>();

        var joueurs = new List<Joueur>
        {
            new Joueur("J1"),
            new Joueur("J2")
        };

        Jouer jouer = new Jouer(joueurs, grille.Object);
        jouer.symboleActuel = 'X';

        var grid = CreerGrilleVide();
        grid[1][1] = 'O'; // case déjà occupée

        bool resultat = jouer.coupValide(new[] { "1", "1" }, grid);

        Assert.False(resultat);
    }

    [Fact]
    public void CoupValide_Accepte_Coup_Valide()
    {
        var grilleMock = new Mock<IGrille>();

        // mock de l'affichage pour éviter que la console soit appelée
        grilleMock.Setup(g => g.AfficherGrille(It.IsAny<List<List<char>>>()));

        var joueurs = new List<Joueur>
        {
            new Joueur("J1"),
            new Joueur("J2")
        };

        Jouer jouer = new Jouer(joueurs, grilleMock.Object);
        jouer.symboleActuel = 'X';

        var grid = CreerGrilleVide();

        bool resultat = jouer.coupValide(new[] { "0", "2" }, grid);

        Assert.True(resultat);
        Assert.Equal('X', grid[0][2]);
    }

    [Fact]
    public void SymboleActuel_Correct_A_Chaque_Tour()
    {
        var grille = new Mock<IGrille>();

        var j1 = new Joueur("J1") { symboleChoisi = "X"[0] };
        var j2 = new Joueur("J2") { symboleChoisi = "O"[0] };

        var joueurs = new List<Joueur> { j1, j2 };
        var jouer = new Jouer(joueurs, grille.Object);

        var grid = CreerGrilleVide();

        // Tour 0 → joueur 1
        jouer.tour = 0;
        Console.SetIn(new StringReader("1 1"));
        jouer.jouerTour(grid);
        Assert.Equal('X', jouer.symboleActuel);

        // Tour 1 → joueur 2
        jouer.tour = 1;
        Console.SetIn(new StringReader("2 2"));
        jouer.jouerTour(grid);
        Assert.Equal('O', jouer.symboleActuel);
    }
}
