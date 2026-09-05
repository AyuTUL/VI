namespace FifaSquadBuilder.Services.Player;

public enum PlayerSortField
{
    Overall,
    Potential,
    Age,
    ValueEUR,
    WageEUR,
    Name
}

public class PlayerSearchQuery
{
    public string? NameContains { get; set; }
    public int? PositionId { get; set; }
    public int? ClubId { get; set; }
    public int? LeagueId { get; set; }
    public int? NationId { get; set; }

    public int? MinOverall { get; set; }
    public int? MaxOverall { get; set; }
    public int? MinPotential { get; set; }
    public int? MaxPotential { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public decimal? MinWage { get; set; }
    public decimal? MaxWage { get; set; }

    public PlayerSortField SortBy { get; set; } = PlayerSortField.Overall;
    public bool SortDescending { get; set; } = true;

    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 25;
    // Hard cap: the picker/admin list never gets to request an unbounded page,
    // which is what keeps this safe against "load everything into the browser".
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch { < 1 => 1, > 100 => 100, _ => value };
    }
}
