using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FifaSquadBuilder.Models;

public class Squad
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int FormationId { get; set; }
    public Formation Formation { get; set; } = null!;

    // User-configurable budget ceiling for total wages. Exceeding it does not block
    // saving (Rule 12) — it just drives a warning in the UI/statistics view.
    [Column(TypeName = "decimal(18,2)")]
    public decimal WageBudgetEUR { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SquadPlayer> SquadPlayers { get; set; } = new List<SquadPlayer>();
}
