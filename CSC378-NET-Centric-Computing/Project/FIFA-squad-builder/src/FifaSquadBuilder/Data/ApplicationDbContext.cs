using FifaSquadBuilder.Data.SeedData;
using FifaSquadBuilder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Nation> Nations => Set<Nation>();
    public DbSet<League> Leagues => Set<League>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Formation> Formations => Set<Formation>();
    public DbSet<FormationPosition> FormationPositions => Set<FormationPosition>();
    public DbSet<Squad> Squads => Set<Squad>();
    public DbSet<SquadPlayer> SquadPlayers => Set<SquadPlayer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- Nation ---
        builder.Entity<Nation>(e =>
        {
            e.HasIndex(n => n.Name).IsUnique();
        });

        // --- League ---
        builder.Entity<League>(e =>
        {
            e.HasIndex(l => l.Name).IsUnique();
            e.HasIndex(l => l.SourceLeagueId).IsUnique();
        });

        // --- Club ---
        builder.Entity<Club>(e =>
        {
            e.HasIndex(c => c.Name);
            e.HasOne(c => c.League)
                .WithMany(l => l.Clubs)
                .HasForeignKey(c => c.LeagueId)
                .OnDelete(DeleteBehavior.Restrict); // don't cascade-wipe clubs if a league row is removed
        });

        // --- Position ---
        builder.Entity<Position>(e =>
        {
            e.HasIndex(p => p.Code).IsUnique();
        });

        // --- Player ---
        builder.Entity<Player>(e =>
        {
            e.HasIndex(p => p.SourceId).IsUnique();
            e.HasIndex(p => p.Overall);
            e.HasIndex(p => p.PositionId);
            e.HasIndex(p => p.ClubId);
            e.HasIndex(p => p.NationId);
            e.HasIndex(p => p.Name);

            e.HasOne(p => p.Nation)
                .WithMany(n => n.Players)
                .HasForeignKey(p => p.NationId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.Club)
                .WithMany(c => c.Players)
                .HasForeignKey(p => p.ClubId)
                .OnDelete(DeleteBehavior.SetNull); // losing a club shouldn't delete the player

            e.HasOne(p => p.Position)
                .WithMany(pos => pos.Players)
                .HasForeignKey(p => p.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(p => p.ValueEUR).HasColumnType("decimal(18,2)");
            e.Property(p => p.WageEUR).HasColumnType("decimal(18,2)");
        });

        // --- Formation ---
        builder.Entity<Formation>(e =>
        {
            e.HasIndex(f => f.Name).IsUnique();
        });

        // --- FormationPosition ---
        builder.Entity<FormationPosition>(e =>
        {
            e.HasIndex(fp => new { fp.FormationId, fp.OrderIndex }).IsUnique();

            e.HasOne(fp => fp.Formation)
                .WithMany(f => f.Positions)
                .HasForeignKey(fp => fp.FormationId)
                .OnDelete(DeleteBehavior.Cascade); // deleting a formation removes its own slot defs

            e.HasOne(fp => fp.Position)
                .WithMany(pos => pos.FormationPositions)
                .HasForeignKey(fp => fp.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Squad ---
        builder.Entity<Squad>(e =>
        {
            e.HasIndex(s => s.UserId);
            e.Property(s => s.WageBudgetEUR).HasColumnType("decimal(18,2)");

            e.HasOne(s => s.User)
                .WithMany(u => u.Squads)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade); // user deleted -> their squads go too

            e.HasOne(s => s.Formation)
                .WithMany(f => f.Squads)
                .HasForeignKey(s => s.FormationId)
                .OnDelete(DeleteBehavior.Restrict); // don't let a formation delete cascade into user squads
        });

        // --- SquadPlayer ---
        builder.Entity<SquadPlayer>(e =>
        {
            // A player can't occupy a squad twice (starting XI or bench).
            e.HasIndex(sp => new { sp.SquadId, sp.PlayerId }).IsUnique();

            // A pitch slot can hold at most one player. NOTE: MySQL has no partial/filtered
            // index support (unlike SQL Server), so this is a plain unique index — but MySQL
            // treats each NULL as distinct in a unique index, so multiple bench rows
            // (FormationPositionId = null) never collide; only two rows sharing the same
            // real slot id for the same squad would. That's exactly the constraint we want.
            e.HasIndex(sp => new { sp.SquadId, sp.FormationPositionId })
                .IsUnique();

            e.HasOne(sp => sp.Squad)
                .WithMany(s => s.SquadPlayers)
                .HasForeignKey(sp => sp.SquadId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(sp => sp.Player)
                .WithMany(p => p.SquadPlayers)
                .HasForeignKey(sp => sp.PlayerId)
                .OnDelete(DeleteBehavior.Restrict); // don't let deleting a player silently gut saved squads

            e.HasOne(sp => sp.FormationPosition)
                .WithMany(fp => fp.SquadPlayers)
                .HasForeignKey(sp => sp.FormationPositionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Seed data ---
        builder.Entity<Position>().HasData(PositionSeedData.GetSeedData());
        builder.Entity<Formation>().HasData(FormationSeedData.GetFormations());
        builder.Entity<FormationPosition>().HasData(FormationSeedData.GetFormationPositions());
        builder.Entity<IdentityRole>().HasData(RoleSeedData.GetRoles());
    }
}
