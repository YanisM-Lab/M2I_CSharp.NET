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
    Console.WriteLine("\n");
}

// Conditions de fin de partie

static bool PartieTerminee(char[,] grille, char symbole)
{
    for(int i = 0; i < 3; i++)
    {
        if (grille[i, 0] == symbole && grille[i, 1] == symbole && grille[i, 2] == symbole)
            return true;
        if (grille[0, i] == symbole && grille[1, i] == symbole && grille[2, i] == symbole)
            return true;
    }
    if (grille[0, 0] == symbole && grille[1, 1] == symbole && grille[2, 2] == symbole)
        return true;
    if (grille[0, 2] == symbole && grille[1, 1] == symbole && grille[2, 0] == symbole) 
        return true;
    return false;
}


// Programme principal

char[,] grilleMorpion = ConstruireGrille();
Console.WriteLine("Bienvenue dans le jeu du Morpion !\n\n");
AfficherGrille(grilleMorpion);

// Choix des symboles

Console.WriteLine("Joueur 1 : veuillez choisir votre symbole {X, O}");
char symboleJoueur1 = Console.ReadLine().ToUpper()[0];
char symboleJoueur2 = (symboleJoueur1 == 'X') ? 'O' : 'X';

if (symboleJoueur1 != 'X' && symboleJoueur1 != 'O')
{
    Console.WriteLine("Symbole invalide. Veuillez choisir un symbole valide : {X, O}\n\n");
    return;
}

// Gameplay

int tour = 0;

// Enumération des états de la partie


EtatPartie etatPartie = EtatPartie.EnCours;

while (etatPartie == EtatPartie.EnCours)
{
    char symboleActuel = (tour % 2 == 0) ? symboleJoueur1 : symboleJoueur2;
    Console.WriteLine($"\nTour du joueur {(tour % 2) + 1} ({symboleActuel}). Entrez vos coordonnées (ligne et colonne) séparées par un espace :");
    string[] entrees = Console.ReadLine().Split(' ');
    int ligne = int.Parse(entrees[0]);
    int colonne = int.Parse(entrees[1]);
    if (ligne < 0 || ligne > 2 || colonne < 0 || colonne > 2 || grilleMorpion[ligne, colonne] != ' ')
    {
        Console.WriteLine("Coup invalide. Veuillez réessayer.\n");
        continue;
    }
    Console.WriteLine("\n");
    grilleMorpion[ligne, colonne] = symboleActuel;
    AfficherGrille(grilleMorpion);
    if (PartieTerminee(grilleMorpion, symboleActuel))
    {
        etatPartie = EtatPartie.Gagne;
        Console.WriteLine($"\nLe joueur {(tour % 2) + 1} ({symboleActuel}) a gagné !\n");
    }
    else if (tour == 8)
    {
        etatPartie = EtatPartie.Nul;
        Console.WriteLine("\nLa partie est nulle !\n");
    }
    else
    {
        tour++;
    }
}

public enum EtatPartie
{
    EnCours,
    Gagne,
    Nul
}

