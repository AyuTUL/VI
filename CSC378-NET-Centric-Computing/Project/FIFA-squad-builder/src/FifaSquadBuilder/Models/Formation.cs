using System.ComponentModel.DataAnnotations;

namespace FifaSquadBuilder.Models;

public class Formation
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Name { get; set; } = string.Empty; // e.g. "4-3-3"

    public ICollection<FormationPosition> Positions { get; set; } = new List<FormationPosition>();
    public ICollection<Squad> Squads { get; set; } = new List<Squad>();
}
