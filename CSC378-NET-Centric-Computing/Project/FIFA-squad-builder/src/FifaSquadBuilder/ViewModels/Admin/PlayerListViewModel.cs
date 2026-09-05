using FifaSquadBuilder.Services.Player;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifaSquadBuilder.ViewModels.Admin;

public class PlayerListViewModel
{
    public PlayerSearchQuery Query { get; set; } = new();
    public PagedResult<PlayerSearchResultItem> Results { get; set; } = new();

    public List<SelectListItem> PositionOptions { get; set; } = new();
    public List<SelectListItem> ClubOptions { get; set; } = new();
    public List<SelectListItem> LeagueOptions { get; set; } = new();
    public List<SelectListItem> NationOptions { get; set; } = new();
}
