using Microsoft.EntityFrameworkCore;

using TD1_Morpion.Data.Entities;

namespace TD1_Morpion.Data;

public class MorpionDbContext : DbContext
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Move> Moves => Set<Move>();

    public MorpionDbContext(DbContextOptions<MorpionDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("games");

            entity.HasKey(g => g.Id);

            entity.Property(g => g.Player1Name).IsRequired().HasMaxLength(100);
            entity.Property(g => g.Player2Name).IsRequired().HasMaxLength(100);
            entity.Property(g => g.Status).IsRequired().HasMaxLength(20);
            entity.Property(g => g.CurrentPlayerName).IsRequired().HasMaxLength(100);
            entity.Property(g => g.WinnerName).HasMaxLength(100);
            entity.Property(g => g.BoardJson).IsRequired();
        });

        modelBuilder.Entity<Move>(entity =>
        {
            entity.ToTable("moves");

            entity.HasKey(m => m.Id);

            entity.Property(m => m.PlayerName).IsRequired().HasMaxLength(100);

            entity.HasOne(m => m.Game)
                .WithMany(g => g.Moves)
                .HasForeignKey(m => m.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
