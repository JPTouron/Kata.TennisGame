

namespace Kata.TennisGame.Application;

public enum TeamId
{
    Team1, Team2
}

public interface IScore
{
    public ITeamScore Team1 { get; }

    public ITeamScore Team2 { get; }

    void ScorePoint(TeamId teamId);
}

public interface ITeamScore
{
    int GamePoints { get; }

    bool MatchWon { get; }


    int Set { get; }
}

//revisar el disenio de esto q es horrible
public class Score : IScore
{
    private readonly TeamScore team1;
    private readonly TeamScore team2;

    public Score()
    {
        team1 = new TeamScore();
        team2 = new TeamScore();
    }

    public ITeamScore Team1 => team1;

    public ITeamScore Team2 => team2;

    public string CurrentStatus => CalculateCurrentStatus();

    private string CalculateCurrentStatus()
    {
        if (team1.GamePoints >= 3 && team2.GamePoints >= 3 && team1.GamePoints == team2.GamePoints)
            return "Deuce";

        if (team1.GamePoints > team2.GamePoints && team1.GamePoints >= 3 && team2.GamePoints >= 3)
            return $"Advantage - {team2.GamePoints}";

        if (team1.GamePoints > team2.GamePoints)
            return $"{team1.GamePoints} - {team2.GamePoints}";

        if (team2.GamePoints > team1.GamePoints && team2.GamePoints >= 3 && team1.GamePoints >= 3)
            return $"Advantage - {team1.GamePoints}";


        return $"{team2.GamePoints} - {team1.GamePoints}";
    }

    public void ScorePoint(TeamId teamId)
    {
        switch (teamId)
        {
            case TeamId.Team1:
                team1.ScorePoint();
                break;

            case TeamId.Team2:
                team2.ScorePoint();
                break;

            default:
                break;
        }
    }

    private class TeamScore : ITeamScore
    {
        public TeamScore()
        {
            GamePoints = 0;
            Set = 0;
            MatchWon = false;
        }



        public int GamePoints { get; private set; }

        public int Set { get; private set; }

        public bool MatchWon { get; private set; }

        internal void ScorePoint() => GamePoints++;
    }
}