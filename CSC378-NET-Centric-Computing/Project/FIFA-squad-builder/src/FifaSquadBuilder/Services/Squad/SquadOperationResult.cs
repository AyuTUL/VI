namespace FifaSquadBuilder.Services.Squad;

public class SquadOperationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public static SquadOperationResult Ok() => new() { Success = true };
    public static SquadOperationResult Fail(string message) => new() { Success = false, ErrorMessage = message };
}
