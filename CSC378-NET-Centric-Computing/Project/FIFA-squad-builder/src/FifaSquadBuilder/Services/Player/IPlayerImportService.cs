namespace FifaSquadBuilder.Services.Player;

public interface IPlayerImportService
{
    /// <summary>
    /// Imports/upserts Nations, Leagues, Clubs and Players from the FIFA dataset CSVs.
    /// Safe to run repeatedly - existing rows are matched by their source dataset IDs
    /// (Player.SourceId, League.SourceLeagueId) or by unique name (Nation, Club) and
    /// updated in place rather than duplicated.
    /// </summary>
    /// <param name="playersCsvPath">Path to players_fifa23.csv (or equivalent export).</param>
    /// <param name="teamsCsvPath">Path to teams_fifa23.csv (or equivalent export), used
    /// only to resolve each club's league - the players CSV has no league column.</param>
    Task<PlayerImportResult> ImportAsync(string playersCsvPath, string teamsCsvPath);
}
