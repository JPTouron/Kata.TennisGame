namespace Kata.TennisGame.Application;



public interface IPlayer
{
    string Name { get; }

}

public class NoPlayer : IPlayer
{
    public NoPlayer()
    {
    }

    public string Name => "unknown";
}

public class Player : IPlayer
{
    private Player(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static IPlayer Create(string name)
    {
        return new Player(name);
    }

    public static IPlayer CreateEmpty()
    {
        return new NoPlayer();
    }
}