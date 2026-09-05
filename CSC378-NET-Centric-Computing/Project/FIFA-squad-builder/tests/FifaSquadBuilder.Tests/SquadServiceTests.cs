using FifaSquadBuilder.Data;
using FifaSquadBuilder.Models;
using FifaSquadBuilder.Services.Squad;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FifaSquadBuilder.Tests;

public class SquadServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<(ApplicationDbContext db, int formationId, int slot1Id, int slot2Id, int playerAId, int playerBId, int playerCId)> SeedAsync()
    {
        var db = CreateContext();

        var gk = new Position { Code = "GK", Name = "Goalkeeper" };
        var st = new Position { Code = "ST", Name = "Striker" };
        db.Positions.AddRange(gk, st);

        var nation = new Nation { Name = "Testland" };
        db.Nations.Add(nation);

        var formation = new Formation { Name = "Test-2" };
        db.Formations.Add(formation);
        await db.SaveChangesAsync();

        var slot1 = new FormationPosition { FormationId = formation.Id, PositionId = gk.Id, X = 50, Y = 5, OrderIndex = 0 };
        var slot2 = new FormationPosition { FormationId = formation.Id, PositionId = st.Id, X = 50, Y = 85, OrderIndex = 1 };
        db.FormationPositions.AddRange(slot1, slot2);

        var playerA = new Player { Name = "Player A", Age = 25, NationId = nation.Id, PositionId = st.Id, Overall = 80, Potential = 82, ValueEUR = 1_000_000, WageEUR = 10_000, SourceId = 1 };
        var playerB = new Player { Name = "Player B", Age = 26, NationId = nation.Id, PositionId = gk.Id, Overall = 75, Potential = 78, ValueEUR = 500_000, WageEUR = 8_000, SourceId = 2 };
        var playerC = new Player { Name = "Player C", Age = 27, NationId = nation.Id, PositionId = st.Id, Overall = 70, Potential = 72, ValueEUR = 300_000, WageEUR = 5_000, SourceId = 3 };
        db.Players.AddRange(playerA, playerB, playerC);
        await db.SaveChangesAsync();

        return (db, formation.Id, slot1.Id, slot2.Id, playerA.Id, playerB.Id, playerC.Id);
    }

    [Fact]
    public async Task GetSquadForEditAsync_ReturnsNull_WhenSquadBelongsToAnotherUser()
    {
        var (db, formationId, _, _, _, _, _) = await SeedAsync();
        var service = new SquadService(db);

        var squadId = await service.CreateSquadAsync("owner-user", "My Squad", formationId, 500_000);

        var result = await service.GetSquadForEditAsync(squadId, "different-user");

        Assert.Null(result);
    }

    [Fact]
    public async Task AssignPlayerAsync_MovingAnAlreadyAssignedPlayer_DoesNotCreateADuplicateRow()
    {
        var (db, formationId, slot1Id, slot2Id, playerAId, _, _) = await SeedAsync();
        var service = new SquadService(db);
        var squadId = await service.CreateSquadAsync("u1", "Squad", formationId, 500_000);

        // Assign to bench first, then move the same player to a pitch slot.
        var first = await service.AssignPlayerAsync(squadId, "u1", playerAId, formationPositionId: null);
        Assert.True(first.Success);

        var second = await service.AssignPlayerAsync(squadId, "u1", playerAId, formationPositionId: slot2Id);
        Assert.True(second.Success);

        var editor = await service.GetSquadForEditAsync(squadId, "u1");
        Assert.NotNull(editor);
        // Exactly one occurrence of this player anywhere in the squad, not two.
        var onPitch = editor!.Slots.Count(s => s.PlayerId == playerAId);
        var onBench = editor.Bench.Count(b => b.PlayerId == playerAId);
        Assert.Equal(1, onPitch + onBench);
    }

    [Fact]
    public async Task AssignPlayerAsync_ToOccupiedSlot_BumpsPreviousOccupantToBench()
    {
        var (db, formationId, slot1Id, _, playerAId, playerBId, _) = await SeedAsync();
        var service = new SquadService(db);
        var squadId = await service.CreateSquadAsync("u1", "Squad", formationId, 500_000);

        // playerB (GK) takes the GK slot first.
        var assignB = await service.AssignPlayerAsync(squadId, "u1", playerBId, slot1Id);
        Assert.True(assignB.Success);

        // playerA is a striker but let's force-assign to the same GK slot to test bumping.
        var assignA = await service.AssignPlayerAsync(squadId, "u1", playerAId, slot1Id);
        Assert.True(assignA.Success);

        var editor = await service.GetSquadForEditAsync(squadId, "u1");
        Assert.NotNull(editor);
        Assert.Equal(playerAId, editor!.Slots.First(s => s.FormationPositionId == slot1Id).PlayerId);
        Assert.Contains(editor.Bench, b => b.PlayerId == playerBId);
    }

    [Fact]
    public async Task AssignPlayerAsync_RejectsWrongUser_WithoutModifyingSquad()
    {
        var (db, formationId, slot1Id, _, playerAId, _, _) = await SeedAsync();
        var service = new SquadService(db);
        var squadId = await service.CreateSquadAsync("owner", "Squad", formationId, 500_000);

        var result = await service.AssignPlayerAsync(squadId, "attacker", playerAId, slot1Id);

        Assert.False(result.Success);
        var ownerView = await service.GetSquadForEditAsync(squadId, "owner");
        Assert.Null(ownerView!.Slots.First(s => s.FormationPositionId == slot1Id).PlayerId);
    }

    [Fact]
    public async Task AssignPlayerAsync_ToBench_RejectsWhenBenchIsFull()
    {
        var db = CreateContext();
        var position = new Position { Code = "ST", Name = "Striker" };
        var nation = new Nation { Name = "Testland" };
        var formation = new Formation { Name = "Empty" };
        db.AddRange(position, nation, formation);
        await db.SaveChangesAsync();

        var players = new List<Player>();
        for (int i = 0; i < 8; i++)
        {
            var p = new Player { Name = $"P{i}", Age = 20, NationId = nation.Id, PositionId = position.Id, Overall = 70, Potential = 70, ValueEUR = 1000, WageEUR = 100, SourceId = 100 + i };
            players.Add(p);
        }
        db.Players.AddRange(players);
        await db.SaveChangesAsync();

        var service = new SquadService(db);
        var squadId = await service.CreateSquadAsync("u1", "Squad", formation.Id, 500_000);

        for (int i = 0; i < 7; i++)
        {
            var result = await service.AssignPlayerAsync(squadId, "u1", players[i].Id, formationPositionId: null);
            Assert.True(result.Success, $"Bench slot {i} should have succeeded: {result.ErrorMessage}");
        }

        var eighth = await service.AssignPlayerAsync(squadId, "u1", players[7].Id, formationPositionId: null);
        Assert.False(eighth.Success);
        Assert.Contains("Bench is full", eighth.ErrorMessage);
    }

    [Fact]
    public async Task ChangeFormationAsync_MovesEveryoneToBench_WithoutRemovingAnyone()
    {
        var (db, formationId, slot1Id, slot2Id, playerAId, playerBId, _) = await SeedAsync();
        var service = new SquadService(db);
        var squadId = await service.CreateSquadAsync("u1", "Squad", formationId, 500_000);
        await service.AssignPlayerAsync(squadId, "u1", playerAId, slot1Id);
        await service.AssignPlayerAsync(squadId, "u1", playerBId, slot2Id);

        var otherFormation = new Formation { Name = "Other" };
        db.Formations.Add(otherFormation);
        await db.SaveChangesAsync();

        var result = await service.ChangeFormationAsync(squadId, "u1", otherFormation.Id);
        Assert.True(result.Success);

        var editor = await service.GetSquadForEditAsync(squadId, "u1");
        Assert.NotNull(editor);
        Assert.Equal(2, editor!.Bench.Count); // both players preserved, just moved to bench
    }
}
