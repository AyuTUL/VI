using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifaSquadBuilder.ViewModels.Squad;

public class SquadCreateViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Formation is required")]
    public int FormationId { get; set; }

    [Range(0, 99_999_999)]
    [Display(Name = "Wage budget (EUR)")]
    public decimal WageBudgetEUR { get; set; } = 500_000;

    public List<SelectListItem> FormationOptions { get; set; } = new();
}
