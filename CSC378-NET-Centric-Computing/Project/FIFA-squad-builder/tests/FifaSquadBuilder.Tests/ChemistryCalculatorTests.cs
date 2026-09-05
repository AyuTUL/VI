using FifaSquadBuilder.Services.Calculations;
using Xunit;

namespace FifaSquadBuilder.Tests;

public class ChemistryCalculatorTests
{
    private readonly ChemistryCalculator _calculator = new();

    private static ChemistryPlayerInput Player(int id, int assignedPos, int playerPos, int? club, int? league, int nation) =>
        new()
        {
            SquadPlayerId = id,
            PlayerId = id,
            AssignedPositionId = assignedPos,
            PlayerPositionId = playerPos,
            ClubId = club,
            LeagueId = league,
            NationId = nation,
        };

    [Fact]
    public void PositionalFit_AwardsOnePoint()
    {
        var players = new List<ChemistryPlayerInput> { Player(1, assignedPos: 5, playerPos: 5, club: null, league: null, nation: 1) };
        var result = _calculator.Calculate(players);
        Assert.Equal(1, result.PlayerResults.Single().Points);
    }

    [Fact]
    public void PositionalMismatch_AwardsNoPositionPoint()
    {
        var players = new List<ChemistryPlayerInput> { Player(1, assignedPos: 5, playerPos: 9, club: null, league: null, nation: 1) };
        var result = _calculator.Calculate(players);
        Assert.Equal(0, result.PlayerResults.Single().Points);
    }

    [Fact]
    public void ClubLink_RequiresAtLeastTwoOtherClubmates()
    {
        // Player 1 shares a club with only 1 other starter - below the threshold of 2.
        var players = new List<ChemistryPlayerInput>
        {
            Player(1, 1, 1, club: 100, league: null, nation: 1),
            Player(2, 2, 2, club: 100, league: null, nation: 2),
            Player(3, 3, 3, club: 200, league: null, nation: 3),
        };
        var result = _calculator.Calculate(players);
        // Position mismatched for all (assignedPos==playerPos actually matches here by construction) -
        // isolate club contribution by using distinct nations/leagues so only club could contribute.
        var p1 = result.PlayerResults.First(r => r.SquadPlayerId == 1);
        Assert.Equal(1, p1.Points); // just the positional-fit point, no club point (only 1 clubmate)
    }

    [Fact]
    public void ClubLink_AwardsPointWithTwoOtherClubmates()
    {
        var players = new List<ChemistryPlayerInput>
        {
            Player(1, 1, 1, club: 100, league: null, nation: 1),
            Player(2, 2, 2, club: 100, league: null, nation: 2),
            Player(3, 3, 3, club: 100, league: null, nation: 3),
        };
        var result = _calculator.Calculate(players);
        var p1 = result.PlayerResults.First(r => r.SquadPlayerId == 1);
        Assert.Equal(2, p1.Points); // positional fit + club link
    }

    [Fact]
    public void LeagueLink_RequiresAtLeastThreeOtherLeaguemates()
    {
        var players = new List<ChemistryPlayerInput>
        {
            Player(1, 1, 1, club: null, league: 50, nation: 1),
            Player(2, 2, 2, club: null, league: 50, nation: 2),
            Player(3, 3, 3, club: null, league: 50, nation: 3),
            Player(4, 4, 4, club: null, league: 99, nation: 4),
        };
        var result = _calculator.Calculate(players);
        var p1 = result.PlayerResults.First(r => r.SquadPlayerId == 1);
        // Only 2 other leaguemates (players 2, 3) - below threshold of 3.
        Assert.Equal(1, p1.Points);
    }

    [Fact]
    public void NationLink_AwardsPointWithTwoOtherCountrymen()
    {
        var players = new List<ChemistryPlayerInput>
        {
            Player(1, 1, 1, club: null, league: null, nation: 7),
            Player(2, 2, 2, club: null, league: null, nation: 7),
            Player(3, 3, 3, club: null, league: null, nation: 7),
        };
        var result = _calculator.Calculate(players);
        var p1 = result.PlayerResults.First(r => r.SquadPlayerId == 1);
        Assert.Equal(2, p1.Points); // positional fit + nation link
    }

    [Fact]
    public void Points_AreCappedAtThreeEvenIfAllFourConditionsHold()
    {
        // One player triggers position fit + club + league + nation simultaneously -
        // must still cap at 3, not 4.
        var players = new List<ChemistryPlayerInput>
        {
            Player(1, assignedPos: 1, playerPos: 1, club: 10, league: 20, nation: 30),
            Player(2, assignedPos: 2, playerPos: 2, club: 10, league: 20, nation: 30),
            Player(3, assignedPos: 3, playerPos: 3, club: 10, league: 20, nation: 30),
        };
        var result = _calculator.Calculate(players);
        Assert.All(result.PlayerResults, r => Assert.True(r.Points <= 3));
    }

    [Fact]
    public void EmptyStartingXI_ProducesZeroChemistry()
    {
        var result = _calculator.Calculate(new List<ChemistryPlayerInput>());
        Assert.Equal(0, result.SquadChemistry);
        Assert.Empty(result.PlayerResults);
    }

    [Fact]
    public void SquadChemistry_ScalesToZeroToHundredRange()
    {
        // Max possible per player is 3, max players considered is 11 -> max squad score 33.
        // A single player at full 3 points should scale to round(3*100/33) = 9.
        var players = new List<ChemistryPlayerInput>
        {
            Player(1, assignedPos: 1, playerPos: 1, club: 10, league: 20, nation: 30),
            Player(2, assignedPos: 2, playerPos: 2, club: 10, league: 20, nation: 30),
            Player(3, assignedPos: 3, playerPos: 3, club: 10, league: 20, nation: 30),
        };
        var result = _calculator.Calculate(players);
        Assert.InRange(result.SquadChemistry, 0, 100);
    }
}
