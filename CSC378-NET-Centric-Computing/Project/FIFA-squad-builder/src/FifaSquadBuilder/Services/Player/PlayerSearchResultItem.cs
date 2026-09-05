namespace FifaSquadBuilder.Services.Player;

public class PlayerSearchResultItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PositionCode { get; set; } = string.Empty;
    public int Overall { get; set; }
    public int Potential { get; set; }
    public string? ClubName { get; set; }
    public string? LeagueName { get; set; }
    public string NationName { get; set; } = string.Empty;
    public decimal ValueEUR { get; set; }
    public decimal WageEUR { get; set; }
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
