namespace FifaSquadBuilder.Services.Calculations;

public class WeakPositionEntry
{
    public string PositionCode { get; set; } = string.Empty;
    public string CurrentPlayerName { get; set; } = string.Empty;
    public int CurrentOverall { get; set; }
    public string BestAvailableName { get; set; } = string.Empty;
    public int BestAvailableOverall { get; set; }
    public int Difference { get; set; }
}
