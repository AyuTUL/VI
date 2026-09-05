using FifaSquadBuilder.Data;
using FifaSquadBuilder.Models;
using FifaSquadBuilder.Services.Calculations;
using FifaSquadBuilder.Services.Squad;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FifaSquadBuilder.Tests;

public class SquadStatisticsServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ComputeAsync_AveragesRatingAgeAndPotential_OverStartingXIOnly()
    {
        var db = CreateContext();
        var st = new Position { Code = "ST", Name = "Striker" };
        var gk = new Position { Code = "GK", Name = "Goalkeeper" };
        var nation = new Nation { Name = "Testland" };
        var formation = new Formation { Name = "F" };
        db.AddRange(st, gk, nation, formation);
        await db.SaveChangesAsync();

        var slot = new FormationPosition { FormationId = formation.Id, PositionId = st.Id, X = 50, Y = 85, OrderIndex = 0 };
        db.FormationPositions.Add(slot);

        var starter = new Player { Name = "Starter", Age = 30, NationId = nation.Id, PositionId = st.Id, Overall = 90, Potential = 90, ValueEUR = 1000, WageEUR = 100, SourceId = 1 };
        // Bench player has very different age/overall - must NOT affect StartingXIRating/AverageAge.
        var benchPlayer = new Player { Name = "Bench", Age = 18, NationId = nation.Id, PositionId = gk.Id, Overall = 60, Potential = 60, ValueEUR = 2000, WageEUR = 200, SourceId = 2 };
        db.Players.AddRange(starter, benchPlayer);
        await db.SaveChangesAsync();

        var squadService = new SquadService(db);
        var squadId = await squadService.CreateSquadAsync("u1", "Squad", formation.Id, 500_000);
        await squadService.AssignPlayerAsync(squadId, "u1", starter.Id, slot.Id);
        await squadService.AssignPlayerAsync(squadId, "u1", benchPlayer.Id, formationPositionId: null);

        var statsService = new SquadStatisticsService(db, new ChemistryCalculator());
        var stats = await statsService.ComputeAsync(squadId);

        Assert.NotNull(stats);
        Assert.Equal(90, stats!.StartingXIRating); // starter only
        Assert.Equal(30.0, stats.AverageAge);       // starter only, bench's age 18 excluded
        Assert.Equal(75, stats.SquadRating);        // (90+60)/2, includes bench
        Assert.Equal(3000, stats.SquadValueEUR);    // 1000 + 2000, whole squad
        Assert.Equal(300, stats.TotalWageEUR);      // 100 + 200, whole squad
    }

    [Fact]
    public async Task ComputeAsync_FlagsOverBudget_WhenWagesExceedBudget()
    {
        var db = CreateContext();
        var st = new Position { Code = "ST", Name = "Striker" };
        var nation = new Nation { Name = "Testland" };
        var formation = new Formation { Name = "F" };
        db.AddRange(st, nation, formation);
        await db.SaveChangesAsync();

        var player = new Player { Name = "Expensive", Age = 25, NationId = nation.Id, PositionId = st.Id, Overall = 90, Potential = 90, ValueEUR = 1, WageEUR = 999_999 };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var squadService = new SquadService(db);
        var squadId = await squadService.CreateSquadAsync("u1", "Squad", formation.Id, wageBudgetEUR: 100_000);
        await squadService.AssignPlayerAsync(squadId, "u1", player.Id, formationPositionId: null);

        var statsService = new SquadStatisticsService(db, new ChemistryCalculator());
        var stats = await statsService.ComputeAsync(squadId);

        Assert.NotNull(stats);
        Assert.True(stats!.OverBudget);
    }
}
