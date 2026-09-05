using System.ComponentModel.DataAnnotations;

namespace FifaSquadBuilder.Models;

public class Club
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public int LeagueId { get; set; }
    public League League { get; set; } = null!;

    public ICollection<Player> Players { get; set; } = new List<Player>();
}
