using TD_Morpion_MAUI.Models;

namespace TD_Morpion_MAUI.Services;

public class MorpionService
{
    private readonly Joueur[,] _plateau;

    public Joueur JoueurCourant { get; private set; }
    public EtatPartie EtatPartie { get; private set; }
    public Joueur Gagnant { get; private set; }

    public MorpionService()
    {
        _plateau = new Joueur[3, 3];
        InitialiserPartie();
    }

    public void InitialiserPartie()
    {
        for (int ligne = 0; ligne < 3; ligne++)
        {
            for (int colonne = 0; colonne < 3; colonne++)
            {
                _plateau[ligne, colonne] = Joueur.Aucun;
            }
        }

        JoueurCourant = Joueur.X;
        EtatPartie = EtatPartie.EnCours;
        Gagnant = Joueur.Aucun;
    }

    public bool Jouer(int ligne, int colonne)
    {
        if (EtatPartie != EtatPartie.EnCours)
            return false;

        if (_plateau[ligne, colonne] != Joueur.Aucun)
            return false;

        _plateau[ligne, colonne] = JoueurCourant;

        if (VerifierVictoire(JoueurCourant))
        {
            EtatPartie = EtatPartie.Gagnee;
            Gagnant = JoueurCourant;
            return true;
        }

        if (VerifierMatchNul())
        {
            EtatPartie = EtatPartie.Nulle;
            return true;
        }

        ChangerJoueur();
        return true;
    }

    public Joueur ObtenirValeurCase(int ligne, int colonne)
    {
        return _plateau[ligne, colonne];
    }

    private void ChangerJoueur()
    {
        JoueurCourant = JoueurCourant == Joueur.X ? Joueur.O : Joueur.X;
    }

    private bool VerifierVictoire(Joueur joueur)
    {
        for (int ligne = 0; ligne < 3; ligne++)
        {
            if (_plateau[ligne, 0] == joueur &&
                _plateau[ligne, 1] == joueur &&
                _plateau[ligne, 2] == joueur)
            {
                return true;
            }
        }

        for (int colonne = 0; colonne < 3; colonne++)
        {
            if (_plateau[0, colonne] == joueur &&
                _plateau[1, colonne] == joueur &&
                _plateau[2, colonne] == joueur)
            {
                return true;
            }
        }

        if (_plateau[0, 0] == joueur &&
            _plateau[1, 1] == joueur &&
            _plateau[2, 2] == joueur)
        {
            return true;
        }

        if (_plateau[0, 2] == joueur &&
            _plateau[1, 1] == joueur &&
            _plateau[2, 0] == joueur)
        {
            return true;
        }

        return false;
    }

    private bool VerifierMatchNul()
    {
        for (int ligne = 0; ligne < 3; ligne++)
        {
            for (int colonne = 0; colonne < 3; colonne++)
            {
                if (_plateau[ligne, colonne] == Joueur.Aucun)
                {
                    return false;
                }
            }
        }

        return true;
    }
}