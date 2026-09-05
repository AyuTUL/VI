using FifaSquadBuilder.Data;
using FifaSquadBuilder.Models;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Services.Squad;

public class SquadService : ISquadService
{
    private readonly ApplicationDbContext _db;

    // Not DB-enforced (MySQL has no CHECK-on-count constraint mechanism here) - enforced
    // in this service instead, on every code path that can grow the bench.
    private const int BenchCapacity = 7;

    public SquadService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SquadSummary>> GetUserSquadsAsync(string userId)
    {
        return await _db.Squads
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => new SquadSummary
            {
                Id = s.Id,
                Name = s.Name,
                FormationName = s.Formation.Name,
                UpdatedAt = s.UpdatedAt,
            })
            .ToListAsync();
    }

    public async Task<SquadEditorData?> GetSquadForEditAsync(int squadId, string userId)
    {
        // Ownership check is baked into the query itself (not a separate check after
        // loading) - a squad belonging to someone else simply doesn't match this query,
        // so this method can't be used to distinguish "not found" from "not yours".
        var squad = await _db.Squads
            .Include(s => s.Formation).ThenInclude(f => f.Positions).ThenInclude(fp => fp.Position)
            .Include(s => s.SquadPlayers).ThenInclude(sp => sp.Player).ThenInclude(p => p.Position)
            .FirstOrDefaultAsync(s => s.Id == squadId && s.UserId == userId);

        if (squad == null) return null;

        var data = new SquadEditorData
        {
            Id = squad.Id,
            Name = squad.Name,
            FormationId = squad.FormationId,
            FormationName = squad.Formation.Name,
            WageBudgetEUR = squad.WageBudgetEUR,
        };

        foreach (var fp in squad.Formation.Positions.OrderBy(p => p.OrderIndex))
        {
            var occupant = squad.SquadPlayers.FirstOrDefault(sp => sp.FormationPositionId == fp.Id);
            data.Slots.Add(new SquadEditorSlot
            {
                FormationPositionId = fp.Id,
                PositionId = fp.PositionId,
                PositionCode = fp.Position.Code,
                X = fp.X,
                Y = fp.Y,
                OrderIndex = fp.OrderIndex,
                SquadPlayerId = occupant?.Id,
                PlayerId = occupant?.PlayerId,
                PlayerSourceId = occupant?.Player.SourceId,
                CardImageUrl = occupant?.Player.CardImageUrl,
                PlayerName = occupant?.Player.Name,
                PlayerOverall = occupant?.Player.Overall,
                Pace = occupant?.Player.Pace,
                Shooting = occupant?.Player.Shooting,
                Passing = occupant?.Player.Passing,
                Dribbling = occupant?.Player.Dribbling,
                Defending = occupant?.Player.Defending,
                Physicality = occupant?.Player.Physicality,
            });
        }

        foreach (var sp in squad.SquadPlayers.Where(sp => sp.FormationPositionId == null).OrderBy(sp => sp.SlotOrder))
        {
            data.Bench.Add(new SquadEditorBenchSlot
            {
                SquadPlayerId = sp.Id,
                PlayerId = sp.PlayerId,
                PlayerSourceId = sp.Player.SourceId,
                CardImageUrl = sp.Player.CardImageUrl,
                PlayerName = sp.Player.Name,
                PositionCode = sp.Player.Position.Code,
                Overall = sp.Player.Overall,
                Pace = sp.Player.Pace,
                Shooting = sp.Player.Shooting,
                Passing = sp.Player.Passing,
                Dribbling = sp.Player.Dribbling,
                Defending = sp.Player.Defending,
                Physicality = sp.Player.Physicality,
                SlotOrder = sp.SlotOrder,
            });
        }

        return data;
    }

    public async Task<int> CreateSquadAsync(string userId, string name, int formationId, decimal wageBudgetEUR)
    {
        var squad = new Models.Squad
        {
            UserId = userId,
            Name = name,
            FormationId = formationId,
            WageBudgetEUR = wageBudgetEUR,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _db.Squads.Add(squad);
        await _db.SaveChangesAsync();
        return squad.Id;
    }

    public async Task<SquadOperationResult> RenameSquadAsync(int squadId, string userId, string newName)
    {
        var squad = await FindOwnedSquadAsync(squadId, userId);
        if (squad == null) return SquadOperationResult.Fail("Squad not found.");

        if (string.IsNullOrWhiteSpace(newName) || newName.Length > 100)
            return SquadOperationResult.Fail("Squad name must be 1-100 characters.");

        squad.Name = newName.Trim();
        squad.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return SquadOperationResult.Ok();
    }

    public async Task<SquadOperationResult> SetWageBudgetAsync(int squadId, string userId, decimal budget)
    {
        var squad = await FindOwnedSquadAsync(squadId, userId);
        if (squad == null) return SquadOperationResult.Fail("Squad not found.");
        if (budget < 0) return SquadOperationResult.Fail("Budget cannot be negative.");

        squad.WageBudgetEUR = budget;
        squad.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return SquadOperationResult.Ok();
    }

    public async Task<SquadOperationResult> ChangeFormationAsync(int squadId, string userId, int newFormationId)
    {
        var squad = await _db.Squads
            .Include(s => s.SquadPlayers)
            .FirstOrDefaultAsync(s => s.Id == squadId && s.UserId == userId);
        if (squad == null) return SquadOperationResult.Fail("Squad not found.");

        var formationExists = await _db.Formations.AnyAsync(f => f.Id == newFormationId);
        if (!formationExists) return SquadOperationResult.Fail("Formation not found.");

        squad.FormationId = newFormationId;
        // Non-destructive: nobody is removed from the squad, they all move to the bench.
        // Bench capacity is intentionally not enforced here - see BenchCapacity comment.
        foreach (var sp in squad.SquadPlayers)
        {
            sp.FormationPositionId = null;
            sp.IsStartingXI = false;
        }
        squad.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return SquadOperationResult.Ok();
    }

    public async Task<SquadOperationResult> DeleteSquadAsync(int squadId, string userId)
    {
        var squad = await FindOwnedSquadAsync(squadId, userId);
        if (squad == null) return SquadOperationResult.Fail("Squad not found.");

        _db.Squads.Remove(squad);
        try
        {
            await _db.SaveChangesAsync();
            return SquadOperationResult.Ok();
        }
        catch (DbUpdateException)
        {
            return SquadOperationResult.Fail("Could not delete this squad.");
        }
    }

    public async Task<SquadOperationResult> AssignPlayerAsync(int squadId, string userId, int playerId, int? formationPositionId)
    {
        var squad = await _db.Squads
            .Include(s => s.SquadPlayers)
            .FirstOrDefaultAsync(s => s.Id == squadId && s.UserId == userId);
        if (squad == null) return SquadOperationResult.Fail("Squad not found.");

        var playerExists = await _db.Players.AnyAsync(p => p.Id == playerId);
        if (!playerExists) return SquadOperationResult.Fail("Player not found.");

        if (formationPositionId.HasValue)
        {
            var slotValid = await _db.FormationPositions
                .AnyAsync(fp => fp.Id == formationPositionId.Value && fp.FormationId == squad.FormationId);
            if (!slotValid) return SquadOperationResult.Fail("That pitch slot doesn't belong to this squad's formation.");
        }

        var existingForPlayer = squad.SquadPlayers.FirstOrDefault(sp => sp.PlayerId == playerId);
        var benchCountExcludingMovingPlayer = squad.SquadPlayers
            .Count(sp => sp.FormationPositionId == null && sp.PlayerId != playerId);

        if (formationPositionId.HasValue)
        {
            var occupant = squad.SquadPlayers
                .FirstOrDefault(sp => sp.FormationPositionId == formationPositionId.Value && sp.PlayerId != playerId);

            if (occupant != null)
            {
                if (benchCountExcludingMovingPlayer + 1 > BenchCapacity)
                {
                    return SquadOperationResult.Fail(
                        "Bench is full (7 max) - remove a substitute before replacing this position.");
                }
                occupant.FormationPositionId = null;
                occupant.IsStartingXI = false;
                occupant.SlotOrder = NextBenchSlotOrder(squad.SquadPlayers, excludePlayerId: playerId);
            }
        }
        else
        {
            // Moving/adding to the bench. Only a genuinely new bench occupant counts
            // against capacity - a player already on the bench being reordered isn't new.
            var alreadyOnBench = existingForPlayer?.FormationPositionId == null && existingForPlayer != null;
            if (!alreadyOnBench && benchCountExcludingMovingPlayer >= BenchCapacity)
            {
                return SquadOperationResult.Fail("Bench is full (7 max).");
            }
        }

        if (existingForPlayer != null)
        {
            existingForPlayer.FormationPositionId = formationPositionId;
            existingForPlayer.IsStartingXI = formationPositionId.HasValue;
            if (!formationPositionId.HasValue)
            {
                existingForPlayer.SlotOrder = NextBenchSlotOrder(squad.SquadPlayers, excludePlayerId: playerId);
            }
        }
        else
        {
            _db.SquadPlayers.Add(new SquadPlayer
            {
                SquadId = squadId,
                PlayerId = playerId,
                FormationPositionId = formationPositionId,
                IsStartingXI = formationPositionId.HasValue,
                SlotOrder = formationPositionId.HasValue ? 0 : NextBenchSlotOrder(squad.SquadPlayers, excludePlayerId: playerId),
            });
        }

        squad.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _db.SaveChangesAsync();
            return SquadOperationResult.Ok();
        }
        catch (DbUpdateException)
        {
            // Final safety net behind the unique indexes (duplicate player in squad,
            // two players in one slot) - should be unreachable given the checks above,
            // but the DB constraint is the real source of truth, not this code.
            return SquadOperationResult.Fail("This assignment conflicts with squad rules (duplicate player or occupied slot).");
        }
    }

    public async Task<SquadOperationResult> RemovePlayerAsync(int squadId, string userId, int squadPlayerId)
    {
        var squad = await _db.Squads
            .Include(s => s.SquadPlayers)
            .FirstOrDefaultAsync(s => s.Id == squadId && s.UserId == userId);
        if (squad == null) return SquadOperationResult.Fail("Squad not found.");

        var squadPlayer = squad.SquadPlayers.FirstOrDefault(sp => sp.Id == squadPlayerId);
        if (squadPlayer == null) return SquadOperationResult.Fail("Player is not in this squad.");

        _db.SquadPlayers.Remove(squadPlayer);
        squad.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return SquadOperationResult.Ok();
    }

    private Task<Models.Squad?> FindOwnedSquadAsync(int squadId, string userId) =>
        _db.Squads.FirstOrDefaultAsync(s => s.Id == squadId && s.UserId == userId);

    private static int NextBenchSlotOrder(IEnumerable<SquadPlayer> squadPlayers, int excludePlayerId)
    {
        var benchOrders = squadPlayers
            .Where(sp => sp.FormationPositionId == null && sp.PlayerId != excludePlayerId)
            .Select(sp => sp.SlotOrder)
            .ToList();
        return benchOrders.Count == 0 ? 0 : benchOrders.Max() + 1;
    }
}
