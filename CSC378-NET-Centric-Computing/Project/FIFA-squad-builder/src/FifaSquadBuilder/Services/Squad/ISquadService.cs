namespace FifaSquadBuilder.Services.Squad;

public interface ISquadService
{
    Task<List<SquadSummary>> GetUserSquadsAsync(string userId);

    /// <summary>Null if the squad doesn't exist OR isn't owned by userId - the two
    /// cases are deliberately indistinguishable to the caller, so a controller can't
    /// leak whether a given squad id belongs to someone else.</summary>
    Task<SquadEditorData?> GetSquadForEditAsync(int squadId, string userId);

    Task<int> CreateSquadAsync(string userId, string name, int formationId, decimal wageBudgetEUR);
    Task<SquadOperationResult> RenameSquadAsync(int squadId, string userId, string newName);
    Task<SquadOperationResult> SetWageBudgetAsync(int squadId, string userId, decimal budget);

    /// <summary>Changing formation clears every pitch-slot assignment (everyone moves
    /// to bench) without removing any player from the squad - non-destructive by design.</summary>
    Task<SquadOperationResult> ChangeFormationAsync(int squadId, string userId, int newFormationId);

    Task<SquadOperationResult> DeleteSquadAsync(int squadId, string userId);

    /// <summary>Assigns a player to a pitch slot (formationPositionId set) or the bench
    /// (formationPositionId null). Handles both "add a new player to the squad" and
    /// "move/replace an already-assigned player" in one call.</summary>
    Task<SquadOperationResult> AssignPlayerAsync(int squadId, string userId, int playerId, int? formationPositionId);

    Task<SquadOperationResult> RemovePlayerAsync(int squadId, string userId, int squadPlayerId);
}
