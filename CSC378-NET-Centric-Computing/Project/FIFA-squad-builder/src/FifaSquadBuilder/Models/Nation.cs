using System.ComponentModel.DataAnnotations;

namespace FifaSquadBuilder.Models;

public class Nation
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // ISO-ish short code for compact display (e.g. "BRA", "ENG"). Nullable: the FIFA23
    // dataset only provides full country names, not ISO codes, so the importer leaves
    // this null rather than fabricating an abbreviation (e.g. "United States" would
    // wrongly become "UNI" from a naive first-3-letters approach). An admin can fill
    // it in later, or a proper ISO 3166 lookup table can be added if this matters.
    [MaxLength(10)]
    public string? Code { get; set; }

    public ICollection<Player> Players { get; set; } = new List<Player>();
}
