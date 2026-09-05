using Microsoft.AspNetCore.Identity;

namespace FifaSquadBuilder.Data.SeedData;

public static class RoleSeedData
{
    // Fixed GUIDs so HasData produces a stable migration rather than a new one
    // every time IDs would otherwise regenerate.
    public const string AdminRoleId = "8f14e45f-ceea-467e-9575-e5c1d1e12a10";
    public const string UserRoleId = "3c59dc04-8bf5-4c3d-9e79-5ab4e9c4c31b";

    public static IdentityRole[] GetRoles() => new[]
    {
        new IdentityRole
        {
            Id = AdminRoleId,
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = AdminRoleId
        },
        new IdentityRole
        {
            Id = UserRoleId,
            Name = "User",
            NormalizedName = "USER",
            ConcurrencyStamp = UserRoleId
        },
    };
}
