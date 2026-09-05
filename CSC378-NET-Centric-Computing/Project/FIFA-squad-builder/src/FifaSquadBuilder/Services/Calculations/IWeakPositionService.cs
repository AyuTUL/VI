namespace FifaSquadBuilder.Services.Calculations;

public interface IWeakPositionService
{
    /// <summary>
    /// For each filled starting-XI slot, compares the assigned player's Overall against
    /// the best-Overall player in the database at that exact position who isn't already
    /// used anywhere in this squad. Returns only gaps meeting the significance threshold.
    /// Ownership is the caller's responsibility - this takes a raw squadId.
    /// </summary>
    Task<List<WeakPositionEntry>> FindWeakPositionsAsync(int squadId);
}
