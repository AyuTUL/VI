using FifaSquadBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Services.Calculations;

public class WeakPositionService : IWeakPositionService
{
    private readonly ApplicationDbContext _db;

    // Only surface a recommendation if the best alternative is meaningfully better -
    // otherwise every slot in every squad would show a 1-point "improvement".
    private const int SignificanceThreshold = 5;

    public WeakPositionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<WeakPositionEntry>> FindWeakPositionsAsync(int squadId)
    {
        var squad = await _db.Squads
            .Include(s => s.SquadPlayers).ThenInclude(sp => sp.Player).ThenInclude(p => p.Position)
            .Include(s => s.SquadPlayers).ThenInclude(sp => sp.FormationPosition)
            .FirstOrDefaultAsync(s => s.Id == squadId);
        if (squad == null) return new List<WeakPositionEntry>();

        var playerIdsInSquad = squad.SquadPlayers.Select(sp => sp.PlayerId).ToHashSet();
        var results = new List<WeakPositionEntry>();

        foreach (var sp in squad.SquadPlayers.Where(sp => sp.IsStartingXI && sp.FormationPosition != null))
        {
            var requiredPositionId = sp.FormationPosition!.PositionId;

            var bestAvailable = await _db.Players
                .Where(p => p.PositionId == requiredPositionId && !playerIdsInSquad.Contains(p.Id))
                .OrderByDescending(p => p.Overall)
                .Select(p => new { p.Name, p.Overall })
                .FirstOrDefaultAsync();

            if (bestAvailable == null) continue; // no alternative exists at this position at all

            var difference = bestAvailable.Overall - sp.Player.Overall;
            if (difference >= SignificanceThreshold)
            {
                results.Add(new WeakPositionEntry
                {
                    PositionCode = sp.Player.Position.Code,
                    CurrentPlayerName = sp.Player.Name,
                    CurrentOverall = sp.Player.Overall,
                    BestAvailableName = bestAvailable.Name,
                    BestAvailableOverall = bestAvailable.Overall,
                    Difference = difference,
                });
            }
        }

        return results.OrderByDescending(r => r.Difference).ToList();
    }
}
