namespace FifaSquadBuilder.Services.Calculations;

// This app's own chemistry model. Not a reproduction of EA/FIFA's proprietary
// algorithm - documented fully in README.md under "Chemistry formula".
//
// Per starting-XI player, points are awarded (each worth +1, capped at 3 total):
//   - Positional fit: the player's natural position matches the formation slot
//     they're assigned to.
//   - Club link: at least 2 OTHER starting players share this player's club.
//   - League link: at least 3 OTHER starting players share this player's league.
//   - Nation link: at least 2 OTHER starting players share this player's nation.
// Squad chemistry = sum of all players' points, scaled onto a 0-100 display range
// (max possible = 11 players * 3 points = 33, so scaled by 100/33).
public class ChemistryCalculator : IChemistryCalculator
{
    private const int MaxPointsPerPlayer = 3;
    private const int ClubLinkThreshold = 2;
    private const int LeagueLinkThreshold = 3;
    private const int NationLinkThreshold = 2;
    private const int MaxStartingXI = 11;

    public ChemistryResult Calculate(IReadOnlyList<ChemistryPlayerInput> startingXI)
    {
        var result = new ChemistryResult();

        foreach (var player in startingXI)
        {
            var points = 0;

            if (player.AssignedPositionId == player.PlayerPositionId)
            {
                points++;
            }

            if (player.ClubId.HasValue)
            {
                var clubMates = startingXI.Count(p => p.SquadPlayerId != player.SquadPlayerId && p.ClubId == player.ClubId);
                if (clubMates >= ClubLinkThreshold) points++;
            }

            if (player.LeagueId.HasValue)
            {
                var leagueMates = startingXI.Count(p => p.SquadPlayerId != player.SquadPlayerId && p.LeagueId == player.LeagueId);
                if (leagueMates >= LeagueLinkThreshold) points++;
            }

            var nationMates = startingXI.Count(p => p.SquadPlayerId != player.SquadPlayerId && p.NationId == player.NationId);
            if (nationMates >= NationLinkThreshold) points++;

            points = Math.Min(points, MaxPointsPerPlayer);
            result.PlayerResults.Add(new ChemistryPlayerResult { SquadPlayerId = player.SquadPlayerId, Points = points });
        }

        var totalPoints = result.PlayerResults.Sum(r => r.Points);
        var maxPossible = MaxStartingXI * MaxPointsPerPlayer; // 33
        result.SquadChemistry = maxPossible == 0 ? 0 : (int)Math.Round(totalPoints * 100.0 / maxPossible);

        return result;
    }
}
