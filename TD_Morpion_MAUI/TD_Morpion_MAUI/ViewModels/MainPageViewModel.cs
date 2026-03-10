using System.Collections.ObjectModel;
using System.Windows.Input;
using TD_Morpion_MAUI.Models;
using TD_Morpion_MAUI.Services;

namespace TD_Morpion_MAUI.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    private readonly MorpionService _morpionService;
    private string _messagePartie = string.Empty;

    public ObservableCollection<Cellule> Cellules { get; }

    public string JoueurCourantTexte =>
        _morpionService.EtatPartie == EtatPartie.EnCours
            ? $"Tour du joueur : {_morpionService.JoueurCourant}"
            : "Partie terminée";

    public string MessagePartie
    {
        get => _messagePartie;
        set
        {
            if (_messagePartie != value)
            {
                _messagePartie = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand JouerCommande { get; }
    public ICommand ReinitialiserCommande { get; }

    public MainPageViewModel()
    {
        _morpionService = new MorpionService();
        Cellules = new ObservableCollection<Cellule>();

        InitialiserCellules();
        JouerCommande = new Command<Cellule>(JouerCase);
        ReinitialiserCommande = new Command(ReinitialiserPartie);

        MettreAJourAffichage();
    }

    private void InitialiserCellules()
    {
        Cellules.Clear();

        for (int ligne = 0; ligne < 3; ligne++)
        {
            for (int colonne = 0; colonne < 3; colonne++)
            {
                Cellules.Add(new Cellule
                {
                    Ligne = ligne,
                    Colonne = colonne,
                    Valeur = _morpionService.ObtenirValeurCase(ligne, colonne)
                });
            }
        }
    }

    private void JouerCase(Cellule? cellule)
    {
        if (cellule is null)
            return;

        bool coupJoue = _morpionService.Jouer(cellule.Ligne, cellule.Colonne);

        if (!coupJoue)
            return;

        cellule.Valeur = _morpionService.ObtenirValeurCase(cellule.Ligne, cellule.Colonne);

        MettreAJourAffichage();
    }

    private void ReinitialiserPartie()
    {
        _morpionService.InitialiserPartie();

        foreach (var cellule in Cellules)
        {
            cellule.Valeur = _morpionService.ObtenirValeurCase(cellule.Ligne, cellule.Colonne);
        }

        MettreAJourAffichage();
    }

    private void MettreAJourAffichage()
    {
        if (_morpionService.EtatPartie == EtatPartie.Gagnee)
        {
            MessagePartie = $"Le joueur {_morpionService.Gagnant} a gagné !";
        }
        else if (_morpionService.EtatPartie == EtatPartie.Nulle)
        {
            MessagePartie = "Match nul !";
        }
        else
        {
            MessagePartie = "Partie en cours...";
        }

        OnPropertyChanged(nameof(JoueurCourantTexte));
    }
}