namespace Kata.TennisGame.Application;

public class Score
{
    public Score()
    {
        Team1 = new TeamScore();
        Team2 = new TeamScore();
    }

    public TeamScore Team1 { get; }

    public TeamScore Team2 { get; }
}

public class TeamScore
{
    public TeamScore()
    {
        Points = 0;
        Game = 0;
        Set = 0;
        MatchWon = false;
    }

    public int Points { get; }

    public int Game { get; }

    public int Set { get; }

    public bool MatchWon { get; }
}


