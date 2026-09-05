namespace FifaSquadBuilder.Models;

public class FormationPosition
{
    public int Id { get; set; }

    public int FormationId { get; set; }
    public Formation Formation { get; set; } = null!;

    public int PositionId { get; set; }
    public Position Position { get; set; } = null!;

    // Pitch coordinates on a 0-100 scale. X: 0=left touchline, 100=right touchline.
    // Y: 0=own goal line (GK end), 100=opponent's goal line (striker end).
    // The UI maps these directly to CSS percentage positioning on the pitch element.
    public int X { get; set; }
    public int Y { get; set; }

    // Display/assignment order within the formation (0 = GK, ascending outward).
    // Also used as a stable slot identifier when rendering the pitch.
    public int OrderIndex { get; set; }

    public ICollection<SquadPlayer> SquadPlayers { get; set; } = new List<SquadPlayer>();
}
