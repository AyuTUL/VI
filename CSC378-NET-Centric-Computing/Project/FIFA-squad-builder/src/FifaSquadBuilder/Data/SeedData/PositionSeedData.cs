using FifaSquadBuilder.Models;

namespace FifaSquadBuilder.Data.SeedData;

public static class PositionSeedData
{
    // Explicit IDs required by EF's HasData. Referenced by FormationSeedData below —
    // do not renumber without updating that file too.
    public const int GK = 1;
    public const int CB = 2;
    public const int LB = 3;
    public const int RB = 4;
    public const int LWB = 5;
    public const int RWB = 6;
    public const int CDM = 7;
    public const int CM = 8;
    public const int CAM = 9;
    public const int LM = 10;
    public const int RM = 11;
    public const int LW = 12;
    public const int RW = 13;
    public const int CF = 14;
    public const int ST = 15;

    public static Position[] GetSeedData() => new[]
    {
        new Position { Id = GK, Code = "GK", Name = "Goalkeeper" },
        new Position { Id = CB, Code = "CB", Name = "Centre Back" },
        new Position { Id = LB, Code = "LB", Name = "Left Back" },
        new Position { Id = RB, Code = "RB", Name = "Right Back" },
        new Position { Id = LWB, Code = "LWB", Name = "Left Wing Back" },
        new Position { Id = RWB, Code = "RWB", Name = "Right Wing Back" },
        new Position { Id = CDM, Code = "CDM", Name = "Central Defensive Midfielder" },
        new Position { Id = CM, Code = "CM", Name = "Central Midfielder" },
        new Position { Id = CAM, Code = "CAM", Name = "Central Attacking Midfielder" },
        new Position { Id = LM, Code = "LM", Name = "Left Midfielder" },
        new Position { Id = RM, Code = "RM", Name = "Right Midfielder" },
        new Position { Id = LW, Code = "LW", Name = "Left Winger" },
        new Position { Id = RW, Code = "RW", Name = "Right Winger" },
        new Position { Id = CF, Code = "CF", Name = "Centre Forward" },
        new Position { Id = ST, Code = "ST", Name = "Striker" },
    };
}
