using FifaSquadBuilder.Services.Calculations;
using FifaSquadBuilder.Services.Squad;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifaSquadBuilder.ViewModels.Squad;

public class SquadDetailsViewModel
{
    public SquadEditorData Editor { get; set; } = new();
    public SquadStatistics Statistics { get; set; } = new();
    public List<WeakPositionEntry> WeakPositions { get; set; } = new();
    public List<SelectListItem> FormationOptions { get; set; } = new();
    public List<SelectListItem> PositionOptions { get; set; } = new();
}
