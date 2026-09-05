using FifaSquadBuilder.Data;
using FifaSquadBuilder.Models;
using FifaSquadBuilder.Services.Calculations;
using FifaSquadBuilder.Services.Squad;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FifaSquadBuilder.Tests;

public class WeakPositionServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task FindWeakPositionsAsync_ReportsGap_WhenSignificantlyBetterAlternativeExists()
    {
        var db = CreateContext();
        var st = new Position { Code = "ST", Name = "Striker" };
        var nation = new Nation { Name = "Testland" };
        var formation = new Formation { Name = "F" };
        db.AddRange(st, nation, formation);
        await db.SaveChangesAsync();

        var slot = new FormationPosition { FormationId = formation.Id, PositionId = st.Id, X = 50, Y = 85, OrderIndex = 0 };
        db.FormationPositions.Add(slot);

        var currentPlayer = new Player { Name = "Weak Striker", Age = 30, NationId = nation.Id, PositionId = st.Id, Overall = 65, Potential = 65, ValueEUR = 1000, WageEUR = 100, SourceId = 1 };
        var betterPlayer = new Player { Name = "Star Striker", Age = 25, NationId = nation.Id, PositionId = st.Id, Overall = 88, Potential = 90, ValueEUR = 90_000_000, WageEUR = 300_000, SourceId = 2 };
        db.Players.AddRange(currentPlayer, betterPlayer);
        await db.SaveChangesAsync();

        var squadService = new SquadService(db);
        var squadId = await squadService.CreateSquadAsync("u1", "Squad", formation.Id, 500_000);
        await squadService.AssignPlayerAsync(squadId, "u1", currentPlayer.Id, slot.Id);

        var weakService = new WeakPositionService(db);
        var results = await weakService.FindWeakPositionsAsync(squadId);

        var entry = Assert.Single(results);
        Assert.Equal("Weak Striker", entry.CurrentPlayerName);
        Assert.Equal("Star Striker", entry.BestAvailableName);
        Assert.Equal(88 - 65, entry.Difference);
    }

    [Fact]
    public async Task FindWeakPositionsAsync_ExcludesPlayersAlreadyInTheSquad()
    {
        var db = CreateContext();
        var st = new Position { Code = "ST", Name = "Striker" };
        var nation = new Nation { Name = "Testland" };
        var formation = new Formation { Name = "F" };
        db.AddRange(st, nation, formation);
        await db.SaveChangesAsync();

        var slot = new FormationPosition { FormationId = formation.Id, PositionId = st.Id, X = 50, Y = 85, OrderIndex = 0 };
        db.FormationPositions.Add(slot);

        var currentPlayer = new Player { Name = "Current", Age = 28, NationId = nation.Id, PositionId = st.Id, Overall = 70, Potential = 70, ValueEUR = 1000, WageEUR = 100, SourceId = 1 };
        // The objectively best striker is already on the bench of this same squad -
        // they should not be recommended as a "replacement" for themselves-adjacent slot.
        var alreadyOnBench = new Player { Name = "AlreadyMine", Age = 24, NationId = nation.Id, PositionId = st.Id, Overall = 95, Potential = 95, ValueEUR = 1, WageEUR = 1, SourceId = 2 };
        var genuinelyAvailable = new Player { Name = "TrueAlternative", Age = 26, NationId = nation.Id, PositionId = st.Id, Overall = 80, Potential = 80, ValueEUR = 1, WageEUR = 1, SourceId = 3 };
        db.Players.AddRange(currentPlayer, alreadyOnBench, genuinelyAvailable);
        await db.SaveChangesAsync();

        var squadService = new SquadService(db);
        var squadId = await squadService.CreateSquadAsync("u1", "Squad", formation.Id, 500_000);
        await squadService.AssignPlayerAsync(squadId, "u1", currentPlayer.Id, slot.Id);
        await squadService.AssignPlayerAsync(squadId, "u1", alreadyOnBench.Id, formationPositionId: null);

        var weakService = new WeakPositionService(db);
        var results = await weakService.FindWeakPositionsAsync(squadId);

        var entry = Assert.Single(results);
        Assert.Equal("TrueAlternative", entry.BestAvailableName); // not "AlreadyMine", even though it's rated higher
    }

    [Fact]
    public async Task FindWeakPositionsAsync_DoesNotReport_WhenGapBelowThreshold()
    {
        var db = CreateContext();
        var st = new Position { Code = "ST", Name = "Striker" };
        var nation = new Nation { Name = "Testland" };
        var formation = new Formation { Name = "F" };
        db.AddRange(st, nation, formation);
        await db.SaveChangesAsync();

        var slot = new FormationPosition { FormationId = formation.Id, PositionId = st.Id, X = 50, Y = 85, OrderIndex = 0 };
        db.FormationPositions.Add(slot);

        var currentPlayer = new Player { Name = "Current", Age = 28, NationId = nation.Id, PositionId = st.Id, Overall = 80, Potential = 80, ValueEUR = 1000, WageEUR = 100, SourceId = 1 };
        var slightlyBetter = new Player { Name = "Marginal", Age = 26, NationId = nation.Id, PositionId = st.Id, Overall = 83, Potential = 83, ValueEUR = 1, WageEUR = 1, SourceId = 2 }; // +3, below the 5-point threshold
        db.Players.AddRange(currentPlayer, slightlyBetter);
        await db.SaveChangesAsync();

        var squadService = new SquadService(db);
        var squadId = await squadService.CreateSquadAsync("u1", "Squad", formation.Id, 500_000);
        await squadService.AssignPlayerAsync(squadId, "u1", currentPlayer.Id, slot.Id);

        var weakService = new WeakPositionService(db);
        var results = await weakService.FindWeakPositionsAsync(squadId);

        Assert.Empty(results);
    }

    [Fact]
    public async Task FindWeakPositionsAsync_NoAlternativeExists_DoesNotThrow()
    {
        var db = CreateContext();
        var st = new Position { Code = "ST", Name = "Striker" };
        var nation = new Nation { Name = "Testland" };
        var formation = new Formation { Name = "F" };
        db.AddRange(st, nation, formation);
        await db.SaveChangesAsync();

        var slot = new FormationPosition { FormationId = formation.Id, PositionId = st.Id, X = 50, Y = 85, OrderIndex = 0 };
        db.FormationPositions.Add(slot);

        var onlyPlayer = new Player { Name = "OnlyOne", Age = 28, NationId = nation.Id, PositionId = st.Id, Overall = 60, Potential = 60, ValueEUR = 1, WageEUR = 1, SourceId = 1 };
        db.Players.Add(onlyPlayer);
        await db.SaveChangesAsync();

        var squadService = new SquadService(db);
        var squadId = await squadService.CreateSquadAsync("u1", "Squad", formation.Id, 500_000);
        await squadService.AssignPlayerAsync(squadId, "u1", onlyPlayer.Id, slot.Id);

        var weakService = new WeakPositionService(db);
        var results = await weakService.FindWeakPositionsAsync(squadId);

        Assert.Empty(results);
    }
}
