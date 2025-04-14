namespace Kata.TennisGame.Application;

public interface IPlayer
{
    event EventHandler? BallHitSuccessful;

    string Name { get; }

    bool AttemptHitBall();
}

public class NoPlayer : IPlayer
{
    public NoPlayer()
    {
    }

    public event EventHandler? BallHitSuccessful;

    public string Name => "unknown";

    public bool AttemptHitBall() => false;
}

public class Player : IPlayer
{
    private readonly IPlayerProfile playerProfile;

    private Player(string name, IPlayerProfile playerProfile)
    {
        Name = name;
        this.playerProfile = playerProfile;
    }

    public event EventHandler? BallHitSuccessful;

    public string Name { get; }

    public static IPlayer CreateHighProfilePlayer(string name)
    {
        return new Player(name, PlayerProfile.HighLevel());
    }

    public static IPlayer CreateMediumProfilePlayer(string name)
    {
        return new Player(name, PlayerProfile.MediumLevel());
    }

    public static IPlayer CreateLowProfilePlayer(string name)
    {
        return new Player(name, PlayerProfile.LowLevel());
    }

    public static IPlayer CreateEmpty()
    {
        return new NoPlayer();
    }

    public bool AttemptHitBall()
    {
        var isHitSuccessful = playerProfile.IsHitSuccessful();
        if (isHitSuccessful)
        {
            OnBallHitSuccessful(EventArgs.Empty);
        }
        return isHitSuccessful;
    }

    protected virtual void OnBallHitSuccessful(EventArgs e)
    {
        BallHitSuccessful?.Invoke(this, e);
    }
}