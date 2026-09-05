namespace FifaSquadBuilder.Services.Squad;

public class SquadSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FormationName { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class SquadEditorSlot
{
    public int FormationPositionId { get; set; }
    public int PositionId { get; set; }
    public string PositionCode { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int OrderIndex { get; set; }

    // Null if the slot is empty.
    public int? SquadPlayerId { get; set; }
    public int? PlayerId { get; set; }
    public int? PlayerSourceId { get; set; }
    public string? CardImageUrl { get; set; }
    public string? PlayerName { get; set; }
    public int? PlayerOverall { get; set; }
    public int? Pace { get; set; }
    public int? Shooting { get; set; }
    public int? Passing { get; set; }
    public int? Dribbling { get; set; }
    public int? Defending { get; set; }
    public int? Physicality { get; set; }
}

public class SquadEditorBenchSlot
{
    public int SquadPlayerId { get; set; }
    public int PlayerId { get; set; }
    public int PlayerSourceId { get; set; }
    public string? CardImageUrl { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string PositionCode { get; set; } = string.Empty;
    public int Overall { get; set; }
    public int? Pace { get; set; }
    public int? Shooting { get; set; }
    public int? Passing { get; set; }
    public int? Dribbling { get; set; }
    public int? Defending { get; set; }
    public int? Physicality { get; set; }
    public int SlotOrder { get; set; }
}

public class SquadEditorData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FormationId { get; set; }
    public string FormationName { get; set; } = string.Empty;
    public decimal WageBudgetEUR { get; set; }

    public List<SquadEditorSlot> Slots { get; set; } = new();
    public List<SquadEditorBenchSlot> Bench { get; set; } = new();
}
