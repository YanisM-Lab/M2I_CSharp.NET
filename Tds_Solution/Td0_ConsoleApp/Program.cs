// Construire la grille
static char[,] ConstruireGrille()
{
    //Console.WriteLine("Bienvenue dans le jeu du Morpion !");
    //AfficherGrille();
    char[,] grilleMorpion = new char[3, 3];
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            grilleMorpion[i, j] = ' ';
        }
    }
    return grilleMorpion;
}

// Afficher la grille
static void AfficherGrille(char[,] grille)
{
    Console.WriteLine("  0 1 2");
    for (int i = 0; i < 3; i++)
    {
        Console.Write(i + " ");
        for (int j = 0; j < 3; j++)
        {
            Console.Write(grille[i, j]);
            if (j < 2) Console.Write("|");
        }
        Console.WriteLine();
        if (i < 2) Console.WriteLine("  -----");
    }
}

// Programme principal

char[,] grilleMorpion = ConstruireGrille();
Console.WriteLine("Bienvenue dans le jeu du Morpion !\n\n");
AfficherGrille(grilleMorpion);
Console.WriteLine("Joueur 1 : veuillez choisir votre symbole {X, O}");
char symboleJoueur1 = Console.ReadLine().ToUpper()[0];
char symboleJoueur2 = (symboleJoueur1 == 'X') ? 'O' : 'X';

// gameplay



