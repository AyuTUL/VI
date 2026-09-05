using Microsoft.AspNetCore.Identity;

namespace FifaSquadBuilder.Models;

// Extends Identity's built-in user. Squad ownership nav property added in Phase 3
// once Squad entity exists, to avoid a forward reference here.
public class ApplicationUser : IdentityUser
{
    public ICollection<Squad> Squads { get; set; } = new List<Squad>();
}
