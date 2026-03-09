using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TD1_Morpion.Data;

public class MorpionDbContextFactory : IDesignTimeDbContextFactory<MorpionDbContext>
{
    public MorpionDbContext CreateDbContext(string[] args)
    {
        string db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "morpiondb";
        string user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "morpion";
        string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "morpionpwd";
        string port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";

        string connectionString =
            $"Host=localhost;Port={port};Database={db};Username={user};Password={password}";

        var optionsBuilder = new DbContextOptionsBuilder<MorpionDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new MorpionDbContext(optionsBuilder.Options);
    }
}