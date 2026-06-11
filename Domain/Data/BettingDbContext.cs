using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Data;

public class BettingDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Bet> Bets { get; set; }

    public BettingDbContext(DbContextOptions<BettingDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            const string connectionString =
                "server=localhost;port=3306;database=BettingDb;user=root;password=123456";
            optionsBuilder.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.HasData(
                new User { Id = 1, Email = "alice@betarena.com" },
                new User { Id = 2, Email = "bob@betarena.com" },
                new User { Id = 3, Email = "carol@betarena.com" },
                new User { Id = 4, Email = "dave@betarena.com" },
                new User { Id = 5, Email = "eve@betarena.com" }
            );
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Games");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Description)
                  .IsRequired()
                  .HasMaxLength(512);
            entity.Property(g => g.Rtp)
                  .HasColumnType("decimal(8,2)")
                  .HasDefaultValue(0m);

            entity.HasData(
                new Game { Id = 1, Description = "Roulette",  Rtp = 0m },
                new Game { Id = 2, Description = "Blackjack", Rtp = 0m },
                new Game { Id = 3, Description = "Slots",     Rtp = 0m },
                new Game { Id = 4, Description = "Poker",     Rtp = 0m },
                new Game { Id = 5, Description = "Baccarat",  Rtp = 0m }
            );
        });

        modelBuilder.Entity<Bet>(entity =>
        {
            entity.ToTable("Bets");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Amount)
                  .HasColumnType("decimal(18,2)");
            entity.Property(b => b.WinAmount)
                  .HasColumnType("decimal(18,2)")
                  .HasDefaultValue(0m);
            entity.Property(b => b.Result)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasDefaultValue("Pending");

            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bets)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.Game)
                  .WithMany(g => g.Bets)
                  .HasForeignKey(b => b.GameId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
