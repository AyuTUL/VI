using System.ComponentModel.DataAnnotations;

namespace FifaSquadBuilder.Models;

public class League
{
    public int Id { get; set; }

    // teams_fifa23.csv's own "LeagueId" column, e.g. 13 for the Premier League.
    // Nullable: leagues an admin adds manually (not from the dataset) won't have one.
    public int? SourceLeagueId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Club> Clubs { get; set; } = new List<Club>();
}
