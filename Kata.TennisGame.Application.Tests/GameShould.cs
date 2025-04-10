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
                * The final score in sets is always read with the winning player's score first, e.g. "6�2, 4�6, 6�0, 7�5"
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
}

internal class Game
{
    public Team Team1 { get; }
    public Team Team2 { get; }
    public Game()
    {
        Team1 = new NoTeam();
        Team2 = new NoTeam();
    }

}
internal class NoTeam : Team
{
    public NoTeam() : base("unknown")
    {
    }
}

internal class Team
{
    public Player Player1 { get; }
    public Player Player2 { get; }
    public string Name { get; }

    public Team(string name)
    {
        Name = name;
        Player1 = new NoPlayer();
        Player2 = new NoPlayer();
    }
}

public class NoPlayer : Player
{

    public NoPlayer() : base("unknown")
    {

    }
}

public class Player
{
    public Player(string name)
    {
        Name = name;
    }

    public string Name { get; }
}