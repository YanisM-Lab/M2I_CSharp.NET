namespace TD1_Morpion.Tests
{
    public class GrilleTest
    {
        private Grille CreerGrille()
        {
            return new Grille();
        }

        // =========================
        // Tests ConstruireGrille()
        // =========================

        [Fact]
        public void ConstruireGrille_Retourne_Grille_3x3_Vide()
        {
            // Arrange
            var grille = CreerGrille();

            // Act
            List<List<char>> resultat = grille.ConstruireGrille();

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(3, resultat.Count);

            foreach (var ligne in resultat)
            {
                Assert.Equal(3, ligne.Count);
                Assert.All(ligne, cellule => Assert.Equal(' ', cellule));
            }
        }

        // =========================
        // Tests PartieTerminee()
        // =========================

        [Fact]
        public void PartieTerminee_Retourne_True_Si_Ligne_Gagnante()
        {
            var grille = CreerGrille();
            var grid = grille.ConstruireGrille();

            grid[1][0] = 'X';
            grid[1][1] = 'X';
            grid[1][2] = 'X';

            bool resultat = grille.PartieTerminee(grid, 'X');

            Assert.True(resultat);
        }

        [Fact]
        public void PartieTerminee_Retourne_True_Si_Colonne_Gagnante()
        {
            var grille = CreerGrille();
            var grid = grille.ConstruireGrille();

            grid[0][2] = 'O';
            grid[1][2] = 'O';
            grid[2][2] = 'O';

            bool resultat = grille.PartieTerminee(grid, 'O');

            Assert.True(resultat);
        }

        [Fact]
        public void PartieTerminee_Retourne_True_Si_Diagonale_Principale_Gagnante()
        {
            var grille = CreerGrille();
            var grid = grille.ConstruireGrille();

            grid[0][0] = 'X';
            grid[1][1] = 'X';
            grid[2][2] = 'X';

            bool resultat = grille.PartieTerminee(grid, 'X');

            Assert.True(resultat);
        }

        [Fact]
        public void PartieTerminee_Retourne_True_Si_Diagonale_Secondaire_Gagnante()
        {
            var grille = CreerGrille();
            var grid = grille.ConstruireGrille();

            grid[0][2] = 'O';
            grid[1][1] = 'O';
            grid[2][0] = 'O';

            bool resultat = grille.PartieTerminee(grid, 'O');

            Assert.True(resultat);
        }

        [Fact]
        public void PartieTerminee_Retourne_False_Si_Pas_De_Victoire()
        {
            var grille = CreerGrille();
            var grid = grille.ConstruireGrille();

            grid[0][0] = 'X';
            grid[0][1] = 'O';
            grid[0][2] = 'X';
            grid[1][0] = 'O';
            grid[1][1] = 'X';
            grid[1][2] = 'O';

            bool resultat = grille.PartieTerminee(grid, 'X');

            Assert.False(resultat);
        }
    }
}
