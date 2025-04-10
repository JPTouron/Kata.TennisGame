


namespace Kata.TennisGame.Application.Tests;

public class GameShould
{
    /*
     game should:
        * start with no players
        * up to 4 players in total
        * have up to 2 teams
            * each team can have either 1 or 2 players
        * keep track of history of scores
        * determine the 1st server by a random toss coin
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
        
        * shots may:
            * be outta bounds (fault)
                * if fault 2x the receiver wins a point
            * be within the court and score a point
            * hit the net                
     
     
     */


    [Fact]
    public void StartWithNoPlayersAndNoTeams()
    {
        var game = new Game();

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
        var game = new Game();

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
        var game = new Game();

        Assert.Equal(GameStatus.NotStarted, game.Status);



    }

    [Fact]
    public void BeAbleToStartHavingTwoPlayersAtMinimum()
    {
        var game = new Game();

        var team1 = Team.CreateSinglesTeam("single Team 1", Player.Create("player 1"));
        var team2 = Team.CreateSinglesTeam("single Team 2", Player.Create("player 2"));

        game.AddTeams(team1, team2);


        game.Start();

        Assert.Equal(GameStatus.Started, game.Status);


    }
    [Fact]
    public void BeAbleToStartHavingFourPlayersAtMaximum()
    {
        var game = new Game();
        Assert.True(false);

    }
    [Fact]
    public void NotBeAbleToAddTwoTeamsWithSameName()
    {
        var game = new Game();
        Assert.True(false);

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
}

internal class Game
{
    //note: verify what info i should be disclosing of the teams, i don't wanna disclose it all
    public ITeam Team1 { get; private set; }
    public ITeam Team2 { get; private set; }
    public Score Score { get; }
    public GameStatus Status { get; private set; }

    public Game()
    {
        Status = GameStatus.NotStarted;
        Team1 = Team.CreateEmptyTeam();
        Team2 = Team.CreateEmptyTeam();
        Score = new Score();
    }

    public void AddTeams(ITeam team1, ITeam team2)
    {
        Team1 = team1;
        Team2 = team2;
    }

    internal void Start()
    {
        Status = GameStatus.Started;
    }
}

public enum GameStatus
{
    NotStarted,
    Started,
    Finished
}

internal class Score
{
    public TeamScore Team1 { get; }
    public TeamScore Team2 { get; }
    public Score()
    {
        Team1 = new TeamScore();
        Team2 = new TeamScore();
    }
}

internal class TeamScore
{
    public int Points { get; }
    public int Game { get; }
    public int Set { get; }
    public bool MatchWon { get; }
    public TeamScore()
    {
        Points = 0;
        Game = 0;
        Set = 0;
        MatchWon = false;

    }
}

internal class NoTeam : ITeam
{
    public NoTeam()
    {
    }

    public string Name => "unknown";

    public IPlayer Player1 => new NoPlayer();
    public IPlayer Player2 => new NoPlayer();
}

internal interface ITeam
{
    string Name { get; }
    IPlayer Player1 { get; }
    IPlayer Player2 { get; }

}

internal class Team : ITeam
{
    public IPlayer Player1 { get; }
    public IPlayer Player2 { get; }
    public string Name { get; }

    public static ITeam CreateSinglesTeam(string teamName, IPlayer player)
    {

        return new Team(teamName, player, new NoPlayer());
    }
    public static ITeam CreateDoublesTeam(string teamName, IPlayer player1, IPlayer player2)
    {

        return new Team(teamName, player1, player2);
    }
    public static ITeam CreateEmptyTeam()
    {

        return new NoTeam();
    }

    private Team(string name, IPlayer player1, IPlayer player2)
    {
        Name = name;
        Player1 = player1;
        Player2 = player2;
    }
}

public class NoPlayer : IPlayer
{

    public NoPlayer()
    {

    }

    public string Name => "unknown";


}

public interface IPlayer
{
    string Name { get; }

}

public class Player : IPlayer
{
    public static IPlayer Create(string name)
    {
        return new Player(name);
    }
    public static IPlayer CreateEmpty()
    {
        return new NoPlayer();
    }
    private Player(string name)
    {

        Name = name;
    }

    public string Name { get; }
}