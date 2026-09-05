using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FifaSquadBuilder.Models;

public class Player
{
    public int Id { get; set; }

    // FIFA dataset's own "ID" column. Lets the importer upsert safely on re-run
    // instead of creating duplicate rows. Nullable is wrong here on purpose —
    // every imported player has one; admin-created players (not from CSV) would
    // need a different creation path, out of scope for this importer.
    public int SourceId { get; set; }

    [MaxLength(500)]
    public string? CardImageUrl { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(15, 55)]
    public int Age { get; set; }

    public int NationId { get; set; }
    public Nation Nation { get; set; } = null!;

    // Nullable: free agents / players without a club exist in real datasets.
    public int? ClubId { get; set; }
    public Club? Club { get; set; }

    public int PositionId { get; set; }
    public Position Position { get; set; } = null!;

    [Range(1, 99)]
    public int Overall { get; set; }

    [Range(1, 99)]
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

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValueEUR { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WageEUR { get; set; }

    public ICollection<SquadPlayer> SquadPlayers { get; set; } = new List<SquadPlayer>();
}
