using System.ComponentModel.DataAnnotations;

namespace FifaSquadBuilder.ViewModels.Admin;

public class FormationPositionFormViewModel
{
    public int Id { get; set; }
    public int FormationId { get; set; }

    [Required(ErrorMessage = "Position is required")]
    public int PositionId { get; set; }

    [Range(0, 100, ErrorMessage = "X must be 0-100 (0=left touchline, 100=right touchline)")]
    public int X { get; set; }

    [Range(0, 100, ErrorMessage = "Y must be 0-100 (0=own goal end, 100=striker end)")]
    public int Y { get; set; }

    [Range(0, 10, ErrorMessage = "Order index must be 0-10 (11 slots max, 0-indexed)")]
    public int OrderIndex { get; set; }
}
