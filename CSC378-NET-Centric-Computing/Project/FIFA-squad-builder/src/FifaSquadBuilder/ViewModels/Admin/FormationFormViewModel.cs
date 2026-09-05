using System.ComponentModel.DataAnnotations;

namespace FifaSquadBuilder.ViewModels.Admin;

public class FormationFormViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Name { get; set; } = string.Empty;
}
