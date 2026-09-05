using FifaSquadBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Services.Calculations;

public class SquadStatisticsService : ISquadStatisticsService
{
    private readonly ApplicationDbContext _db;
    private readonly IChemistryCalculator _chemistryCalculator;

    public SquadStatisticsService(ApplicationDbContext db, IChemistryCalculator chemistryCalculator)
    {
        _db = db;
        _chemistryCalculator = chemistryCalculator;
    }

    public async Task<SquadStatistics?> ComputeAsync(int squadId)
    {
        var squad = await _db.Squads
            .Include(s => s.SquadPlayers).ThenInclude(sp => sp.Player).ThenInclude(p => p.Club)
            .Include(s => s.SquadPlayers).ThenInclude(sp => sp.Player).ThenInclude(p => p.Position)
            .Include(s => s.SquadPlayers).ThenInclude(sp => sp.FormationPosition)
            .FirstOrDefaultAsync(s => s.Id == squadId);
        if (squad == null) return null;

        var all = squad.SquadPlayers;
        var starters = all.Where(sp => sp.IsStartingXI).ToList();

        var stats = new SquadStatistics
        {
            StartingXICount = starters.Count,
            WageBudgetEUR = squad.WageBudgetEUR,
            SquadValueEUR = all.Sum(sp => sp.Player.ValueEUR),
            TotalWageEUR = all.Sum(sp => sp.Player.WageEUR),
            StartingXIRating = starters.Count == 0 ? 0 : (int)Math.Round(starters.Average(sp => sp.Player.Overall)),
            SquadRating = all.Count == 0 ? 0 : (int)Math.Round(all.Average(sp => sp.Player.Overall)),
            AverageAge = starters.Count == 0 ? 0 : Math.Round(starters.Average(sp => sp.Player.Age), 1),
            AveragePotential = starters.Count == 0 ? 0 : Math.Round(starters.Average(sp => sp.Player.Potential), 1),
        };

        var chemistryInputs = starters
            .Where(sp => sp.FormationPosition != null)
            .Select(sp => new ChemistryPlayerInput
            {
                SquadPlayerId = sp.Id,
                PlayerId = sp.PlayerId,
                AssignedPositionId = sp.FormationPosition!.PositionId,
                PlayerPositionId = sp.Player.PositionId,
                ClubId = sp.Player.ClubId,
                LeagueId = sp.Player.Club?.LeagueId,
                NationId = sp.Player.NationId,
            })
            .ToList();

        var chemistryResult = _chemistryCalculator.Calculate(chemistryInputs);
        stats.Chemistry = chemistryResult.SquadChemistry;
        stats.ChemistryBySquadPlayerId = chemistryResult.PlayerResults.ToDictionary(r => r.SquadPlayerId, r => r.Points);

        return stats;
    }
}
