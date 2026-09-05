namespace FifaSquadBuilder.Models;

public class SquadPlayer
{
    public int Id { get; set; }

    public int SquadId { get; set; }
    public Squad Squad { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    // Null = bench slot. Non-null = assigned to this specific pitch slot in the
    // squad's formation. Enforced unique per (SquadId, FormationPositionId) where
    // not null, so two players can't occupy the same pitch position.
    public int? FormationPositionId { get; set; }
    public FormationPosition? FormationPosition { get; set; }

    public bool IsStartingXI { get; set; }

    // Ordering for bench display (0..6). Meaningless for starting XI slots, which
    // are ordered by FormationPosition.OrderIndex instead.
    public int SlotOrder { get; set; }
}
