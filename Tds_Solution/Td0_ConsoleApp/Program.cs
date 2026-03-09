using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TD1_Morpion;
using TD1_Morpion.Data;
using TD1_Morpion.Data.Entities;

namespace Td0_ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        ServiceProvider serviceProvider = ConfigureServices();
        using IServiceScope scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MorpionDbContext>();
        await dbContext.Database.MigrateAsync();

        var repository = new GameRepository(dbContext);
        IGrille grilleService = new Grille();
        IPartie partieService = new Partie();

        Console.WriteLine("Nom du joueur humain :");
        string joueurHumain = LireNomAvecValeurParDefaut("Joueur 1");

        bool quitter = false;

        while (!quitter)
        {
            Console.WriteLine();
            Console.WriteLine("=== MENU ===");
            Console.WriteLine("1. Nouvelle partie");
            Console.WriteLine("2. Reprendre une partie en cours");
            Console.WriteLine("3. Voir les statistiques");
            Console.WriteLine("4. Quitter");

            string? choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    await LancerNouvellePartieAsync(joueurHumain, grilleService, partieService, repository);
                    break;

                case "2":
                    await ReprendrePartieAsync(joueurHumain, grilleService, partieService, repository);
                    break;

                case "3":
                    await AfficherStatistiquesAsync(joueurHumain, repository);
                    break;

                case "4":
                    quitter = true;
                    break;

                default:
                    Console.WriteLine("Choix invalide.");
                    break;
            }
        }

        Console.WriteLine("Au revoir.");
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        string db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "morpiondb";
        string user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "morpion";
        string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "morpionpwd";
        string port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";

        string connectionString =
            $"Host=localhost;Port={port};Database={db};Username={user};Password={password}";

        services.AddDbContext<MorpionDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services.BuildServiceProvider();
    }

    private static async Task LancerNouvellePartieAsync(
        string joueurHumain,
        IGrille grilleService,
        IPartie partieService,
        GameRepository repository)
    {
        Console.WriteLine("Voulez-vous jouer contre l'ordinateur ? (O/N)");
        string? reponse = Console.ReadLine();
        bool contreIA = !string.IsNullOrWhiteSpace(reponse) &&
                        char.ToUpperInvariant(reponse[0]) == 'O';

        List<Joueur> joueurs;

        if (contreIA)
        {
            joueurs = new List<Joueur>
            {
                new Joueur(joueurHumain, estIA: false),
                new Joueur("Ordinateur", estIA: true)
            };
        }
        else
        {
            Console.WriteLine("Nom du joueur 2 :");
            string joueur2Nom = LireNomAvecValeurParDefaut("Joueur 2");

            joueurs = new List<Joueur>
            {
                new Joueur(joueurHumain, estIA: false),
                new Joueur(joueur2Nom, estIA: false)
            };
        }

        joueurs[0].choisirSymbole();
        joueurs[1].symboleChoisi = joueurs[0].symboleChoisi == 'X' ? 'O' : 'X';

        Console.WriteLine($"{joueurs[0].nom} jouera {joueurs[0].symboleChoisi}");
        Console.WriteLine($"{joueurs[1].nom} jouera {joueurs[1].symboleChoisi}");
        Console.WriteLine();

        List<List<char>> grilleMorpion = grilleService.ConstruireGrille();
        grilleService.AfficherGrille(grilleMorpion);

        var jouer = new Jouer(joueurs, grilleService);

        Guid gameId = await repository.CreateGameAsync(
            joueurs[0].nom,
            joueurs[1].nom,
            joueurs[0].symboleChoisi,
            joueurs[1].symboleChoisi,
            joueurs[1].estIA,
            grilleMorpion,
            jouer.tour,
            joueurs[0].nom);

        await BouclePartieAsync(gameId, jouer, grilleMorpion, partieService, repository);
    }

    private static async Task ReprendrePartieAsync(
        string joueurHumain,
        IGrille grilleService,
        IPartie partieService,
        GameRepository repository)
    {
        Game? game = await repository.GetLatestUnfinishedGameAsync(joueurHumain);

        if (game is null)
        {
            Console.WriteLine("Aucune partie en cours à reprendre.");
            return;
        }

        var joueurs = new List<Joueur>
        {
            new Joueur(game.Player1Name, estIA: false) { symboleChoisi = game.Player1Symbol },
            new Joueur(game.Player2Name, estIA: game.IsPlayer2Bot) { symboleChoisi = game.Player2Symbol }
        };

        List<List<char>> grilleMorpion = BoardSerializer.Deserialize(game.BoardJson);

        var jouer = new Jouer(joueurs, grilleService)
        {
            tour = game.CurrentTurn
        };

        Console.WriteLine("Partie reprise.");
        Console.WriteLine($"Joueur courant : {game.CurrentPlayerName}");
        Console.WriteLine($"Tour courant : {game.CurrentTurn + 1}");
        grilleService.AfficherGrille(grilleMorpion);

        await BouclePartieAsync(game.Id, jouer, grilleMorpion, partieService, repository);
    }

    private static async Task AfficherStatistiquesAsync(string joueurHumain, GameRepository repository)
    {
        var stats = await repository.GetStatsAsync(joueurHumain);

        Console.WriteLine();
        Console.WriteLine("=== STATISTIQUES VS BOT ===");
        Console.WriteLine($"Parties jouées : {stats.PlayedGamesAgainstBot}");
        Console.WriteLine($"Victoires humain : {stats.HumanWinsAgainstBot}");
        Console.WriteLine($"Victoires bot : {stats.BotWinsAgainstBot}");
        Console.WriteLine($"Ratio humain/bot : {stats.HumanBotRatio:F2}");
    }

    private static async Task BouclePartieAsync(
        Guid gameId,
        Jouer jouer,
        List<List<char>> grilleMorpion,
        IPartie partieService,
        GameRepository repository)
    {
        EtatPartie etatPartie = EtatPartie.EnCours;

        while (etatPartie == EtatPartie.EnCours)
        {
            string[] coordonneesSaisies = jouer.jouerTour(grilleMorpion);

            if (!jouer.coupValide(coordonneesSaisies, grilleMorpion))
            {
                continue;
            }

            var joueurCourant = jouer.ObtenirJoueurActuel();
            var (ligne, colonne) = Jouer.ParserCoordonnees(coordonneesSaisies);

            if (partieService.PartieTerminee(grilleMorpion, jouer.symboleActuel))
            {
                etatPartie = EtatPartie.Gagne;

                Console.WriteLine($"\nLe joueur {joueurCourant.nom} ({joueurCourant.symboleChoisi}) a gagné !\n");

                await repository.SaveMoveAndStateAsync(
                    gameId,
                    jouer.tour,
                    joueurCourant.nom,
                    jouer.symboleActuel,
                    ligne,
                    colonne,
                    grilleMorpion,
                    "Gagne",
                    joueurCourant.nom,
                    joueurCourant.nom);
            }
            else if (partieService.GrillePleine(grilleMorpion))
            {
                etatPartie = EtatPartie.Nul;

                Console.WriteLine("\nLa partie est nulle !\n");

                await repository.SaveMoveAndStateAsync(
                    gameId,
                    jouer.tour,
                    joueurCourant.nom,
                    jouer.symboleActuel,
                    ligne,
                    colonne,
                    grilleMorpion,
                    "Nul",
                    joueurCourant.nom,
                    null);
            }
            else
            {
                int prochainTour = jouer.tour + 1;
                string prochainJoueur = prochainTour % 2 == 0
                    ? jouer.joueurs[0].nom
                    : jouer.joueurs[1].nom;

                await repository.SaveMoveAndStateAsync(
                    gameId,
                    prochainTour,
                    joueurCourant.nom,
                    jouer.symboleActuel,
                    ligne,
                    colonne,
                    grilleMorpion,
                    "EnCours",
                    prochainJoueur,
                    null);

                jouer.tour++;
            }
        }
    }

    private static string LireNomAvecValeurParDefaut(string valeurParDefaut)
    {
        string? saisie = Console.ReadLine();
        return string.IsNullOrWhiteSpace(saisie) ? valeurParDefaut : saisie.Trim();
    }
}