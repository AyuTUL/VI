namespace FifaSquadBuilder.Services.Calculations;

public class SquadStatistics
{
    public int StartingXICount { get; set; }
    public int StartingXIRating { get; set; } // avg Overall of the 11 starters (0 if none set)
    public int SquadRating { get; set; }       // avg Overall of starting XI + bench together
    public double AverageAge { get; set; }      // starting XI only
    public double AveragePotential { get; set; } // starting XI only, straight from the dataset's Potential field
    public decimal SquadValueEUR { get; set; }   // starting XI + bench
    public decimal TotalWageEUR { get; set; }    // starting XI + bench
    public decimal WageBudgetEUR { get; set; }
    public bool OverBudget => TotalWageEUR > WageBudgetEUR;
    public int Chemistry { get; set; } // 0-100, starting XI only
    public Dictionary<int, int> ChemistryBySquadPlayerId { get; set; } = new(); // 0-3 points per starter
}
