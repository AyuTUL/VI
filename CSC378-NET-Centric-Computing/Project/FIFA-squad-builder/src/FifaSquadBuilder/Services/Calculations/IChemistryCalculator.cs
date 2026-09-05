namespace FifaSquadBuilder.Services.Calculations;

public interface IChemistryCalculator
{
    /// <summary>
    /// Computes this app's own chemistry model (documented in README, not EA's algorithm).
    /// Only pass starting XI players - bench doesn't contribute to chemistry.
    /// </summary>
    ChemistryResult Calculate(IReadOnlyList<ChemistryPlayerInput> startingXI);
}
