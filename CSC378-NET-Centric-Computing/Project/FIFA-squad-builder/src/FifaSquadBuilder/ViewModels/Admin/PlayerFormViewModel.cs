using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifaSquadBuilder.ViewModels.Admin;

public class PlayerFormViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Card image URL")]
    public string? CardImageUrl { get; set; }

    [Required, Range(15, 55)]
    public int Age { get; set; }

    [Required(ErrorMessage = "Nation is required")]
    public int NationId { get; set; }

    // Nullable: "no club / free agent" is a valid, real state (confirmed in the FIFA23
    // dataset via the literal "Free agent" club value), not an oversight.
    public int? ClubId { get; set; }

    [Required(ErrorMessage = "Position is required")]
    public int PositionId { get; set; }

    [Required, Range(1, 99)]
    public int Overall { get; set; }

    [Required, Range(1, 99)]
    public int Potential { get; set; }

    [Range(1, 99)]
    public int? Pace { get; set; }

    [Range(1, 99)]
    public int? Shooting { get; set; }

    [Range(1, 99)]
    public int? Passing { get; set; }

    [Range(1, 99)]
    public int? Dribbling { get; set; }

    [Range(1, 99)]
    public int? Defending { get; set; }

    [Range(1, 99)]
    public int? Physicality { get; set; }

    [Required, Range(0, 999_999_999)]
    [Display(Name = "Value (EUR)")]
    public decimal ValueEUR { get; set; }

    [Required, Range(0, 9_999_999)]
    [Display(Name = "Wage (EUR)")]
    public decimal WageEUR { get; set; }

    // Populated by the controller before rendering, not posted back by the form.
    public List<SelectListItem> NationOptions { get; set; } = new();
    public List<SelectListItem> ClubOptions { get; set; } = new();
    public List<SelectListItem> PositionOptions { get; set; } = new();
}
