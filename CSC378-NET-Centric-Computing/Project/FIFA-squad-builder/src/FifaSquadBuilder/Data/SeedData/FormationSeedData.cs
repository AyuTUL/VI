using FifaSquadBuilder.Models;
using static FifaSquadBuilder.Data.SeedData.PositionSeedData;

namespace FifaSquadBuilder.Data.SeedData;

// Coordinate scheme: X 0-100 (0=left touchline, 100=right touchline),
// Y 0-100 (0=own goal/GK end, 100=opponent goal/striker end). UI maps these
// directly to CSS percentage positioning on the pitch element.
public static class FormationSeedData
{
    public static Formation[] GetFormations() => new[]
    {
        new Formation { Id = 1, Name = "4-3-3" },
        new Formation { Id = 2, Name = "4-4-2" },
        new Formation { Id = 3, Name = "4-2-3-1" },
        new Formation { Id = 4, Name = "4-3-2-1" },
        new Formation { Id = 5, Name = "4-1-2-1-2" },
        new Formation { Id = 6, Name = "3-5-2" },
        new Formation { Id = 7, Name = "5-3-2" },
    };

    public static FormationPosition[] GetFormationPositions() => new[]
    {
        // --- 4-3-3 (FormationId 1) ---
        new FormationPosition { Id = 1,  FormationId = 1, PositionId = GK,  X = 50, Y = 5,  OrderIndex = 0 },
        new FormationPosition { Id = 2,  FormationId = 1, PositionId = LB,  X = 15, Y = 20, OrderIndex = 1 },
        new FormationPosition { Id = 3,  FormationId = 1, PositionId = CB,  X = 35, Y = 18, OrderIndex = 2 },
        new FormationPosition { Id = 4,  FormationId = 1, PositionId = CB,  X = 65, Y = 18, OrderIndex = 3 },
        new FormationPosition { Id = 5,  FormationId = 1, PositionId = RB,  X = 85, Y = 20, OrderIndex = 4 },
        new FormationPosition { Id = 6,  FormationId = 1, PositionId = CM,  X = 30, Y = 50, OrderIndex = 5 },
        new FormationPosition { Id = 7,  FormationId = 1, PositionId = CM,  X = 50, Y = 45, OrderIndex = 6 },
        new FormationPosition { Id = 8,  FormationId = 1, PositionId = CM,  X = 70, Y = 50, OrderIndex = 7 },
        new FormationPosition { Id = 9,  FormationId = 1, PositionId = LW,  X = 20, Y = 80, OrderIndex = 8 },
        new FormationPosition { Id = 10, FormationId = 1, PositionId = ST,  X = 50, Y = 85, OrderIndex = 9 },
        new FormationPosition { Id = 11, FormationId = 1, PositionId = RW,  X = 80, Y = 80, OrderIndex = 10 },

        // --- 4-4-2 (FormationId 2) ---
        new FormationPosition { Id = 12, FormationId = 2, PositionId = GK,  X = 50, Y = 5,  OrderIndex = 0 },
        new FormationPosition { Id = 13, FormationId = 2, PositionId = LB,  X = 15, Y = 20, OrderIndex = 1 },
        new FormationPosition { Id = 14, FormationId = 2, PositionId = CB,  X = 35, Y = 18, OrderIndex = 2 },
        new FormationPosition { Id = 15, FormationId = 2, PositionId = CB,  X = 65, Y = 18, OrderIndex = 3 },
        new FormationPosition { Id = 16, FormationId = 2, PositionId = RB,  X = 85, Y = 20, OrderIndex = 4 },
        new FormationPosition { Id = 17, FormationId = 2, PositionId = LM,  X = 15, Y = 55, OrderIndex = 5 },
        new FormationPosition { Id = 18, FormationId = 2, PositionId = CM,  X = 40, Y = 50, OrderIndex = 6 },
        new FormationPosition { Id = 19, FormationId = 2, PositionId = CM,  X = 60, Y = 50, OrderIndex = 7 },
        new FormationPosition { Id = 20, FormationId = 2, PositionId = RM,  X = 85, Y = 55, OrderIndex = 8 },
        new FormationPosition { Id = 21, FormationId = 2, PositionId = ST,  X = 35, Y = 85, OrderIndex = 9 },
        new FormationPosition { Id = 22, FormationId = 2, PositionId = ST,  X = 65, Y = 85, OrderIndex = 10 },

        // --- 4-2-3-1 (FormationId 3) ---
        new FormationPosition { Id = 23, FormationId = 3, PositionId = GK,  X = 50, Y = 5,  OrderIndex = 0 },
        new FormationPosition { Id = 24, FormationId = 3, PositionId = LB,  X = 15, Y = 20, OrderIndex = 1 },
        new FormationPosition { Id = 25, FormationId = 3, PositionId = CB,  X = 35, Y = 18, OrderIndex = 2 },
        new FormationPosition { Id = 26, FormationId = 3, PositionId = CB,  X = 65, Y = 18, OrderIndex = 3 },
        new FormationPosition { Id = 27, FormationId = 3, PositionId = RB,  X = 85, Y = 20, OrderIndex = 4 },
        new FormationPosition { Id = 28, FormationId = 3, PositionId = CDM, X = 35, Y = 38, OrderIndex = 5 },
        new FormationPosition { Id = 29, FormationId = 3, PositionId = CDM, X = 65, Y = 38, OrderIndex = 6 },
        new FormationPosition { Id = 30, FormationId = 3, PositionId = LW,  X = 20, Y = 65, OrderIndex = 7 },
        new FormationPosition { Id = 31, FormationId = 3, PositionId = CAM, X = 50, Y = 68, OrderIndex = 8 },
        new FormationPosition { Id = 32, FormationId = 3, PositionId = RW,  X = 80, Y = 65, OrderIndex = 9 },
        new FormationPosition { Id = 33, FormationId = 3, PositionId = ST,  X = 50, Y = 88, OrderIndex = 10 },

        // --- 4-3-2-1 "Christmas tree" (FormationId 4) ---
        new FormationPosition { Id = 34, FormationId = 4, PositionId = GK,  X = 50, Y = 5,  OrderIndex = 0 },
        new FormationPosition { Id = 35, FormationId = 4, PositionId = LB,  X = 15, Y = 20, OrderIndex = 1 },
        new FormationPosition { Id = 36, FormationId = 4, PositionId = CB,  X = 35, Y = 18, OrderIndex = 2 },
        new FormationPosition { Id = 37, FormationId = 4, PositionId = CB,  X = 65, Y = 18, OrderIndex = 3 },
        new FormationPosition { Id = 38, FormationId = 4, PositionId = RB,  X = 85, Y = 20, OrderIndex = 4 },
        new FormationPosition { Id = 39, FormationId = 4, PositionId = CM,  X = 30, Y = 45, OrderIndex = 5 },
        new FormationPosition { Id = 40, FormationId = 4, PositionId = CM,  X = 50, Y = 42, OrderIndex = 6 },
        new FormationPosition { Id = 41, FormationId = 4, PositionId = CM,  X = 70, Y = 45, OrderIndex = 7 },
        new FormationPosition { Id = 42, FormationId = 4, PositionId = CF,  X = 35, Y = 70, OrderIndex = 8 },
        new FormationPosition { Id = 43, FormationId = 4, PositionId = CF,  X = 65, Y = 70, OrderIndex = 9 },
        new FormationPosition { Id = 44, FormationId = 4, PositionId = ST,  X = 50, Y = 88, OrderIndex = 10 },

        // --- 4-1-2-1-2 narrow diamond (FormationId 5) ---
        new FormationPosition { Id = 45, FormationId = 5, PositionId = GK,  X = 50, Y = 5,  OrderIndex = 0 },
        new FormationPosition { Id = 46, FormationId = 5, PositionId = LB,  X = 15, Y = 20, OrderIndex = 1 },
        new FormationPosition { Id = 47, FormationId = 5, PositionId = CB,  X = 35, Y = 18, OrderIndex = 2 },
        new FormationPosition { Id = 48, FormationId = 5, PositionId = CB,  X = 65, Y = 18, OrderIndex = 3 },
        new FormationPosition { Id = 49, FormationId = 5, PositionId = RB,  X = 85, Y = 20, OrderIndex = 4 },
        new FormationPosition { Id = 50, FormationId = 5, PositionId = CDM, X = 50, Y = 38, OrderIndex = 5 },
        new FormationPosition { Id = 51, FormationId = 5, PositionId = CM,  X = 30, Y = 55, OrderIndex = 6 },
        new FormationPosition { Id = 52, FormationId = 5, PositionId = CM,  X = 70, Y = 55, OrderIndex = 7 },
        new FormationPosition { Id = 53, FormationId = 5, PositionId = CAM, X = 50, Y = 70, OrderIndex = 8 },
        new FormationPosition { Id = 54, FormationId = 5, PositionId = ST,  X = 35, Y = 88, OrderIndex = 9 },
        new FormationPosition { Id = 55, FormationId = 5, PositionId = ST,  X = 65, Y = 88, OrderIndex = 10 },

        // --- 3-5-2 (FormationId 6) ---
        new FormationPosition { Id = 56, FormationId = 6, PositionId = GK,  X = 50, Y = 5,  OrderIndex = 0 },
        new FormationPosition { Id = 57, FormationId = 6, PositionId = CB,  X = 25, Y = 18, OrderIndex = 1 },
        new FormationPosition { Id = 58, FormationId = 6, PositionId = CB,  X = 50, Y = 15, OrderIndex = 2 },
        new FormationPosition { Id = 59, FormationId = 6, PositionId = CB,  X = 75, Y = 18, OrderIndex = 3 },
        new FormationPosition { Id = 60, FormationId = 6, PositionId = LWB, X = 10, Y = 45, OrderIndex = 4 },
        new FormationPosition { Id = 61, FormationId = 6, PositionId = CM,  X = 35, Y = 50, OrderIndex = 5 },
        new FormationPosition { Id = 62, FormationId = 6, PositionId = CM,  X = 50, Y = 45, OrderIndex = 6 },
        new FormationPosition { Id = 63, FormationId = 6, PositionId = CM,  X = 65, Y = 50, OrderIndex = 7 },
        new FormationPosition { Id = 64, FormationId = 6, PositionId = RWB, X = 90, Y = 45, OrderIndex = 8 },
        new FormationPosition { Id = 65, FormationId = 6, PositionId = ST,  X = 35, Y = 85, OrderIndex = 9 },
        new FormationPosition { Id = 66, FormationId = 6, PositionId = ST,  X = 65, Y = 85, OrderIndex = 10 },

        // --- 5-3-2 (FormationId 7) ---
        new FormationPosition { Id = 67, FormationId = 7, PositionId = GK,  X = 50, Y = 5,  OrderIndex = 0 },
        new FormationPosition { Id = 68, FormationId = 7, PositionId = LWB, X = 10, Y = 25, OrderIndex = 1 },
        new FormationPosition { Id = 69, FormationId = 7, PositionId = CB,  X = 30, Y = 18, OrderIndex = 2 },
        new FormationPosition { Id = 70, FormationId = 7, PositionId = CB,  X = 50, Y = 15, OrderIndex = 3 },
        new FormationPosition { Id = 71, FormationId = 7, PositionId = CB,  X = 70, Y = 18, OrderIndex = 4 },
        new FormationPosition { Id = 72, FormationId = 7, PositionId = RWB, X = 90, Y = 25, OrderIndex = 5 },
        new FormationPosition { Id = 73, FormationId = 7, PositionId = CM,  X = 30, Y = 50, OrderIndex = 6 },
        new FormationPosition { Id = 74, FormationId = 7, PositionId = CM,  X = 50, Y = 45, OrderIndex = 7 },
        new FormationPosition { Id = 75, FormationId = 7, PositionId = CM,  X = 70, Y = 50, OrderIndex = 8 },
        new FormationPosition { Id = 76, FormationId = 7, PositionId = ST,  X = 35, Y = 85, OrderIndex = 9 },
        new FormationPosition { Id = 77, FormationId = 7, PositionId = ST,  X = 65, Y = 85, OrderIndex = 10 },
    };
}
