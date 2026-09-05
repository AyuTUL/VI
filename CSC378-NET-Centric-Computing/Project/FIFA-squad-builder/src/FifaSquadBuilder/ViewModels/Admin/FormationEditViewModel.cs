using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifaSquadBuilder.ViewModels.Admin;

public class FormationPositionRow
{
    public int Id { get; set; }
    public string PositionCode { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int OrderIndex { get; set; }
}

public class FormationEditViewModel
{
    public FormationFormViewModel Formation { get; set; } = new();
    public List<FormationPositionRow> Positions { get; set; } = new();
    public FormationPositionFormViewModel NewPosition { get; set; } = new();
    public List<SelectListItem> PositionOptions { get; set; } = new();
}
