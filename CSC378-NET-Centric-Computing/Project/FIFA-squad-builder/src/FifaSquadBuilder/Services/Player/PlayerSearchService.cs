using FifaSquadBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Services.Player;

public class PlayerSearchService : IPlayerSearchService
{
    private readonly ApplicationDbContext _db;

    public PlayerSearchService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<PlayerSearchResultItem>> SearchAsync(PlayerSearchQuery query)
    {
        var players = _db.Players
            .AsNoTracking()
            .Include(p => p.Nation)
            .Include(p => p.Club)
                .ThenInclude(c => c!.League)
            .Include(p => p.Position)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.NameContains))
        {
            var term = query.NameContains.Trim();
            players = players.Where(p => EF.Functions.Like(p.Name, $"%{term}%"));
        }
        if (query.PositionId.HasValue) players = players.Where(p => p.PositionId == query.PositionId.Value);
        if (query.ClubId.HasValue) players = players.Where(p => p.ClubId == query.ClubId.Value);
        if (query.LeagueId.HasValue) players = players.Where(p => p.Club != null && p.Club.LeagueId == query.LeagueId.Value);
        if (query.NationId.HasValue) players = players.Where(p => p.NationId == query.NationId.Value);

        if (query.MinOverall.HasValue) players = players.Where(p => p.Overall >= query.MinOverall.Value);
        if (query.MaxOverall.HasValue) players = players.Where(p => p.Overall <= query.MaxOverall.Value);
        if (query.MinPotential.HasValue) players = players.Where(p => p.Potential >= query.MinPotential.Value);
        if (query.MaxPotential.HasValue) players = players.Where(p => p.Potential <= query.MaxPotential.Value);
        if (query.MinAge.HasValue) players = players.Where(p => p.Age >= query.MinAge.Value);
        if (query.MaxAge.HasValue) players = players.Where(p => p.Age <= query.MaxAge.Value);
        if (query.MinValue.HasValue) players = players.Where(p => p.ValueEUR >= query.MinValue.Value);
        if (query.MaxValue.HasValue) players = players.Where(p => p.ValueEUR <= query.MaxValue.Value);
        if (query.MinWage.HasValue) players = players.Where(p => p.WageEUR >= query.MinWage.Value);
        if (query.MaxWage.HasValue) players = players.Where(p => p.WageEUR <= query.MaxWage.Value);

        // Count against the filtered-but-unsorted-and-unpaged query, before Skip/Take.
        var totalCount = await players.CountAsync();

        players = (query.SortBy, query.SortDescending) switch
        {
            (PlayerSortField.Overall, true) => players.OrderByDescending(p => p.Overall),
            (PlayerSortField.Overall, false) => players.OrderBy(p => p.Overall),
            (PlayerSortField.Potential, true) => players.OrderByDescending(p => p.Potential),
            (PlayerSortField.Potential, false) => players.OrderBy(p => p.Potential),
            (PlayerSortField.Age, true) => players.OrderByDescending(p => p.Age),
            (PlayerSortField.Age, false) => players.OrderBy(p => p.Age),
            (PlayerSortField.ValueEUR, true) => players.OrderByDescending(p => p.ValueEUR),
            (PlayerSortField.ValueEUR, false) => players.OrderBy(p => p.ValueEUR),
            (PlayerSortField.WageEUR, true) => players.OrderByDescending(p => p.WageEUR),
            (PlayerSortField.WageEUR, false) => players.OrderBy(p => p.WageEUR),
            (PlayerSortField.Name, true) => players.OrderByDescending(p => p.Name),
            (PlayerSortField.Name, false) => players.OrderBy(p => p.Name),
            _ => players.OrderByDescending(p => p.Overall),
        };

        var items = await players
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PlayerSearchResultItem
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                PositionCode = p.Position.Code,
                Overall = p.Overall,
                Potential = p.Potential,
                ClubName = p.Club != null ? p.Club.Name : null,
                LeagueName = p.Club != null ? p.Club.League.Name : null,
                NationName = p.Nation.Name,
                ValueEUR = p.ValueEUR,
                WageEUR = p.WageEUR,
            })
            .ToListAsync();

        return new PagedResult<PlayerSearchResultItem>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }
}
