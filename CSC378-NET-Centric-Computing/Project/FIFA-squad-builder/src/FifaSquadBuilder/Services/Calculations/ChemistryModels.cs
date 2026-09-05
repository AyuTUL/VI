namespace FifaSquadBuilder.Services.Calculations;

public class ChemistryPlayerInput
{
    public int SquadPlayerId { get; set; }
    public int PlayerId { get; set; }
    public int AssignedPositionId { get; set; } // the FormationPosition's PositionId
    public int PlayerPositionId { get; set; } // the player's own natural PositionId
    public int? ClubId { get; set; }
    public int? LeagueId { get; set; }
    public int NationId { get; set; }
}

public class ChemistryPlayerResult
{
    public int SquadPlayerId { get; set; }

    // 0-3 scale, documented in README - this app's own formula, not EA's.
    public int Points { get; set; }
}

public class ChemistryResult
{
    public List<ChemistryPlayerResult> PlayerResults { get; set; } = new();

    // Sum of PlayerResults.Points scaled to 0-100. Only starting XI players are passed
    // in at all, so this is inherently a starting-XI-only figure.
    public int SquadChemistry { get; set; }
}
