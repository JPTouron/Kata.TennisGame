namespace Kata.TennisGame.Application.Tests;

public class GameShould
{
    /*
     game should:
        * start with no players -- DONE
        * up to 4 players in total -- DONE
        * have up to 2 teams -- DONE
            * each team can have either 1 or 2 players -- DONE

        * keep track of history of scores
        * scoring: Game, set, match
            * Game:
                * love, 15, 30, 40, advantage
                * deuce happens when: If at least three points have been scored by each player, making the player's scores equal at 40 apiece
                * advantage when:  If at least three points have been scored by each side and a player has one more point than his opponent, the score of the game is "advantage" for the player in the lead
                * serving player score is called first, then the non-serving player
                * after every odd-numbered Game, the server changes
                    * within a team, the server is alternated between the team members

            * Set:
                * it is a series of Games
                * a Set is won when one team wins at least 6 Games AND 2 Games more than the opponent, if 6-5 then another Game is played
                * The final score in sets is always read with the winning player's score first, e.g. "6–2, 4–6, 6–0, 7–5"
                * if a Set reaches a 6-6 Games won by each team then a TieBreak Game is in play
                * TieBreak:
                    * to be won, it requires a player to win at least 7 points and at least 2 points more than opponent
            * Match:
                * a sequence of sets
                * it is composed by a best of 5 or 3 sets
                    * the first team to win 2 sets outta 3 or 3 sets outta 5, wins the match
                * when a match is won, we refer to the winning team with the phrase: game, set, match <winning team/player>

        * determine the 1st server by a random toss coin
        * shots may:
            * be outta bounds (fault)
                * if fault 2x the receiver wins a point
            * be within the court and score a point
            * hit the net

     */

    [Fact]
    public void StartWithNoPlayersAndNoTeams()
    {
        var game = Game.CreateGame();

        Assert.Equal(typeof(NoTeam), game.Team1.GetType());
        Assert.Equal(typeof(NoTeam), game.Team2.GetType());

        Assert.Equal(typeof(NoPlayer), game.Team1.Player1.GetType());
        Assert.Equal(typeof(NoPlayer), game.Team1.Player2.GetType());

        Assert.Equal(typeof(NoPlayer), game.Team2.Player1.GetType());
        Assert.Equal(typeof(NoPlayer), game.Team2.Player2.GetType());
    }

    [Fact]
    public void StartWithNoScores()
    {
        var game = Game.CreateGame();

        Assert.Equal(0, game.Score.Team1.Points);
        Assert.Equal(0, game.Score.Team1.Game);
        Assert.Equal(0, game.Score.Team1.Set);
        Assert.False(game.Score.Team1.MatchWon);

        Assert.Equal(0, game.Score.Team2.Points);
        Assert.Equal(0, game.Score.Team2.Game);
        Assert.Equal(0, game.Score.Team2.Set);
        Assert.False(game.Score.Team2.MatchWon);
    }

    [Fact]
    public void StartWithNotStartedStatus()
    {
        var game = Game.CreateGame();

        Assert.Equal(GameStatus.NotStarted, game.Status);
    }

    [Fact]
    public void BeAbleToStartHavingTwoPlayersAtMinimum()
    {
        var game = Game.CreateGame();

        var team1 = Team.CreateSinglesTeam("single Team 1", Player.Create("player 1"));
        var team2 = Team.CreateSinglesTeam("single Team 2", Player.Create("player 2"));

        game.AddTeams(team1, team2);

        game.Start();

        Assert.Equal(GameStatus.Started, game.Status);
    }

    [Fact]
    public void BeAbleToStartHavingFourPlayersAtMaximum()
    {
        var game = Game.CreateGame();

        var team1 = Team.CreateDoublesTeam("double Team 1", Player.Create("player 1"), Player.Create("player 2"));
        var team2 = Team.CreateDoublesTeam("double Team 2", Player.Create("player 3"), Player.Create("player 4"));

        game.AddTeams(team1, team2);

        game.Start();

        Assert.Equal(GameStatus.Started, game.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void NotBeAbleToAddATeamWithInvalidPlayerCount(int playerCount)
    {
        var game = Game.CreateGame();

        ITeam team1;
        ITeam team2;
        switch (playerCount)
        {
            case 0:
                team1 = Team.CreateSinglesTeam("single Team 1", Player.CreateEmpty());
                team2 = Team.CreateSinglesTeam("single Team 2", Player.CreateEmpty());
                break;

            case 1:
                team1 = Team.CreateSinglesTeam("single Team 1", Player.Create("player 1"));
                team2 = Team.CreateSinglesTeam("single Team 2", Player.CreateEmpty());
                break;

            case 3:
                team1 = Team.CreateDoublesTeam("single Team 1", Player.Create("player 1"), Player.Create("player 2"));
                team2 = Team.CreateDoublesTeam("single Team 2", Player.Create("player 3"), Player.CreateEmpty());
                break;

            default:
                throw new ArgumentOutOfRangeException("the test cases can contemplate only 1, 3, or 0 players");
        }

        Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
    }

    [Fact]
    public void NotBeAbleToAddTeamsIfTeamsAlreadyExist()
    {
        var game = Game.CreateGame();

        var team1 = Team.CreateSinglesTeam("single Team 1", Player.Create("player 1"));
        var team2 = Team.CreateSinglesTeam("single Team 2", Player.Create("player 2"));

        game.AddTeams(team1, team2);

        Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
    }

    [Fact]
    public void NotBeAbleToAddTwoTeamsWithSameName()
    {
        var game = Game.CreateGame();

        var team1 = Team.CreateSinglesTeam("single Team 1", Player.Create("player 1"));
        var team2 = Team.CreateSinglesTeam("single Team 1", Player.Create("player 2"));

        Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData((string)null)]
    public void NotBeAbleToAddTwoTeamsWithEmptyNames(string invalidName)
    {
        var game = Game.CreateGame();

        var team1 = Team.CreateSinglesTeam(invalidName, Player.Create("player 1"));
        var team2 = Team.CreateSinglesTeam("single Team 1", Player.Create("player 2"));

        Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
    }


    [Fact]
    public void RecordPointWhenPlayerScoresPoint()
    {

        Assert.False(true);

    }

    [Fact]
    public void RecordDeucePointWhenLosingPlayerReachesWinningPlayer()
    {

        Assert.False(true);

    }
    [Fact]
    public void RecordAdvantagePointWhenPlayerWinsAfterADeuce()
    {

        Assert.False(true);

    }
    [Fact]
    public void GoBackToDeuceIfAdvantageIsLost()
    {

        Assert.False(true);

    }
    [Fact]
    public void RecordGameWhenPlayerWinsGame()
    {

        Assert.False(true);

    }
    [Fact]
    public void RecordWonGameOnlyWhenPlayerHasTwoMorePointsThanOpponent()
    {

        Assert.False(true);

    }

    [Fact]
    public void RecordMatchWhenPlayerWinsMatch()
    {

        Assert.False(true);

    }
    [Fact]
    public void ReportScorePointsInProperOrder()
    {

        Assert.False(true);

    }



    //Teams tests
    //[Fact]
    //public void NotBeAbleToCreateDoublesWithOnlyOnePlayer()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
    //[Fact]
    //public void NotBeAbleToCreateSinglesWithNoPlayer()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
    //[Fact]
    //public void NotBeAbleToAddTwoPlayersWithSameName()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
    //[Fact]
    //public void NotBeAbleToAddTwoPlayersWithEmptyNames()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
}