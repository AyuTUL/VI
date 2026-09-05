using System.ComponentModel.DataAnnotations;

namespace FifaSquadBuilder.Models;

public class Position
{
    public int Id { get; set; }

    // Short code, e.g. "GK", "CB", "ST" — referenced throughout the UI and formation logic.
    [Required, MaxLength(5)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<FormationPosition> FormationPositions { get; set; } = new List<FormationPosition>();
}
