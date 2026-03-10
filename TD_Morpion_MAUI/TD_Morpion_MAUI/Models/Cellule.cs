using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TD_Morpion_MAUI.Models;

public class Cellule : INotifyPropertyChanged
{
    private Joueur _valeur;

    public int Ligne { get; set; }
    public int Colonne { get; set; }

    public Joueur Valeur
    {
        get => _valeur;
        set
        {
            if (_valeur != value)
            {
                _valeur = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TexteAffiche));
                OnPropertyChanged(nameof(EstVide));
            }
        }
    }

    public string TexteAffiche => Valeur == Joueur.Aucun ? string.Empty : Valeur.ToString();

    public bool EstVide => Valeur == Joueur.Aucun;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}