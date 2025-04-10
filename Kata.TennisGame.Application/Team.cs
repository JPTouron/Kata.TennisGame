namespace Kata.TennisGame.Application;

public interface ITeam
{
    string Name { get; }

    IPlayer Player1 { get; }

    IPlayer Player2 { get; }

}
public class NoTeam : ITeam
{
    public NoTeam()
    {
    }

    public string Name => "unknown";

    public IPlayer Player1 => new NoPlayer();

    public IPlayer Player2 => new NoPlayer();
}

public class Team : ITeam
{

    private Team(string name, IPlayer player1, IPlayer player2)
    {
        Name = name;
        Player1 = player1;
        Player2 = player2;
    }

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
}
