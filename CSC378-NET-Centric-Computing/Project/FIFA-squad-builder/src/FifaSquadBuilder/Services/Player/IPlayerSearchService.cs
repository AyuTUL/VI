namespace FifaSquadBuilder.Services.Player;

public interface IPlayerSearchService
{
    Task<PagedResult<PlayerSearchResultItem>> SearchAsync(PlayerSearchQuery query);
}
