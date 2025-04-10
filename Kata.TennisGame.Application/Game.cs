namespace Kata.TennisGame.Application;

public enum GameStatus
{
    NotStarted,
    Started,
    Finished
}

public interface IGame
{
    Score Score { get; }

    GameStatus Status { get; }

    ITeam Team1 { get; }

    ITeam Team2 { get; }

    void AddTeams(ITeam team1, ITeam team2);

    void Start();
}

public class Game : IGame
{
    private Game()
    {
        Status = GameStatus.NotStarted;
        Team1 = Team.CreateEmptyTeam();
        Team2 = Team.CreateEmptyTeam();

        Score = new Score();
    }

    //note: verify what info i should be disclosing of the teams, i don't wanna disclose it all
    public ITeam Team1 { get; private set; }

    public ITeam Team2 { get; private set; }

    public Score Score { get; }

    public GameStatus Status { get; private set; }

    public static IGame CreateGame()
    {
        return new Game();
    }

    public void AddTeams(ITeam team1, ITeam team2)
    {
        if (Team1 is Team && Team2 is Team)
            throw new InvalidOperationException("Cannot add teams to this game as it already has teams assigned to it");

        if (string.IsNullOrWhiteSpace(team1.Name) || string.IsNullOrWhiteSpace(team2.Name))
            throw new InvalidOperationException("There's a team with an empty name, all teams must have a name");

        if (string.Compare(team1.Name, team2.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
            throw new InvalidOperationException($"The teams cannot have the same name! teams name: {team1.Name}");

        ThrowInvalidOperationExceptionWhenOddNumberOfPlayers(team1, team2);

        Team1 = team1;
        Team2 = team2;


        var xxx = new List<IPlayer> { team1.Player1, team1.Player2 }.Where(x => x is Player);
        SubscribeEventsForTeam(xxx, OnBallHitSuccessfulTeam1);

        xxx = new List<IPlayer> { team2.Player1, team2.Player2 }.Where(x => x is Player);
        SubscribeEventsForTeam(xxx, OnBallHitSuccessfulTeam2);

    }
    IReadOnlyCollection<IPlayer> subscribedPlayersEvents;
    private void SubscribeEventsForTeam(IEnumerable<IPlayer> players, EventHandler onBallHitSuccessfulTeam)
    {
        foreach (var player in players)
            player.BallHitSuccessful += onBallHitSuccessfulTeam;
    }

    private void OnBallHitSuccessfulTeam1(object? sender, EventArgs e)
    {
        Score.ScorePoint(TeamId.Team1);
    }
    private void OnBallHitSuccessfulTeam2(object? sender, EventArgs e)
    {
        Score.ScorePoint(TeamId.Team2);

    }

    private void Cleanup()
    {
        foreach (var player in subscribedPlayersEvents)
        {
            player.BallHitSuccessful -= OnBallHitSuccessfulTeam1;
            player.BallHitSuccessful -= OnBallHitSuccessfulTeam2;
        }
    }
    public void Finish() { Cleanup(); }

    public void Start()
    {
        Status = GameStatus.Started;
    }

    private static void ThrowInvalidOperationExceptionWhenOddNumberOfPlayers(ITeam team1, ITeam team2)

    {
        var allPlayers = new List<IPlayer> { team1.Player1, team1.Player2, team2.Player1, team2.Player2 };

        var playersCount = allPlayers.Where(x => x is Player == true).Count();

        if (new[] { 0, 1, 3 }.Contains(playersCount))
            throw new InvalidOperationException($"The total players are invalid as they are odd or none, they should be a pair number of players");
    }
}