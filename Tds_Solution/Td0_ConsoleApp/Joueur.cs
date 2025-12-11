using System;

namespace TD1_Morpion
{
    public class Joueur : IJoueur
    {
        public string nom { get; set; }
        public char symboleChoisi { get; set; }
        public bool estIA { get; set; }

        public Joueur(string nom, bool estIA = false)
        {
            this.nom = string.IsNullOrWhiteSpace(nom) ? "Joueur" : nom;
            this.symboleChoisi = ' ';
            this.estIA = estIA;
        }

        public void choisirSymbole()
        {
            while (true)
            {
                Console.WriteLine($"{nom}, veuillez choisir votre symbole (X ou O) :");
                string? saisie = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(saisie))
                {
                    char symbole = char.ToUpperInvariant(saisie[0]);
                    if (symbole == 'X' || symbole == 'O')
                    {
                        this.symboleChoisi = symbole;
                        Console.WriteLine($"{nom} jouera avec le symbole '{symbole}'.");
                        return;
                    }
                }

                Console.WriteLine("Symbole invalide. Veuillez choisir un symbole valide : X ou O.\n");
            }
        }
    }
}
