namespace FifaSquadBuilder.Services.Calculations;

public interface ISquadStatisticsService
{
    /// <summary>Null if the squad doesn't exist. Ownership is the caller's responsibility.</summary>
    Task<SquadStatistics?> ComputeAsync(int squadId);
}
