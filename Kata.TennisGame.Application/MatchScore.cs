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
    int CurrentGamePoints { get; }

    bool MatchWon { get; }

    int SetsWon { get; }

    int GamesWon { get; }


}

internal class MatchScoreText
{
    private readonly ITeamScore team1;
    private readonly ITeamScore team2;

    public MatchScoreText(ITeamScore team1, ITeamScore team2)
    {
        this.team1 = team1;
        this.team2 = team2;
    }
    public string Calculate()
    {//review as :* serving player score is called first, then the non-serving player
        if (team1.CurrentGamePoints >= 3 && team2.CurrentGamePoints >= 3 && team1.CurrentGamePoints == team2.CurrentGamePoints)
            return "Deuce";

        if (team1.CurrentGamePoints > team2.CurrentGamePoints && team1.CurrentGamePoints >= 3 && team2.CurrentGamePoints >= 3)
            return $"Advantage - {team2.CurrentGamePoints}";

        if (team1.CurrentGamePoints > team2.CurrentGamePoints)
            return $"{team1.CurrentGamePoints} - {team2.CurrentGamePoints}";

        if (team2.CurrentGamePoints > team1.CurrentGamePoints && team2.CurrentGamePoints >= 3 && team1.CurrentGamePoints >= 3)
            return $"Advantage - {team1.CurrentGamePoints}";

        return $"{team2.CurrentGamePoints} - {team1.CurrentGamePoints}";
    }
}

//revisar el disenio de esto q es horrible
public class MatchScore : IScore
{
    private readonly TeamScore team1;
    private readonly TeamScore team2;
    MatchScoreText matchScoreText;
    public MatchScore()
    {
        team1 = new TeamScore();
        team2 = new TeamScore();
        matchScoreText = new MatchScoreText(team1, team2);
    }

    public ITeamScore Team1 => team1;

    public ITeamScore Team2 => team2;

    public string CurrentStatus => matchScoreText.Calculate();

    public void ScorePoint(TeamId teamId)
    {
        //refactor this, state machine or whatever
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

        UpdateTeamsStanding();
    }

    private void UpdateTeamsStanding()
    {
        if (HastTeamWonAGame(TeamId.Team1))
            ScoreGameForTeam(TeamId.Team1);

        else if (HastTeamWonAGame(TeamId.Team2))
            ScoreGameForTeam(TeamId.Team2);


        if (HasTeamWonASet(TeamId.Team1))
            team1.ScoreSet();

        else if (HasTeamWonASet(TeamId.Team2))
            team2.ScoreSet();
    }

    private bool HasTeamWonASet(TeamId teamId)
    {
        return teamId == TeamId.Team1 ? team1.GamesWon >= 6 && team1.GamesWon - team2.GamesWon >= 2
                                      : team2.GamesWon >= 6 && team2.GamesWon - team1.GamesWon >= 2;
    }

    private void ScoreGameForTeam(TeamId teamId)
    {
        if (teamId == TeamId.Team1)
            team1.ScoreGame();
        else
            team2.ScoreGame();


        team1.ResetScore();
        team2.ResetScore();
    }

    private bool HastTeamWonAGame(TeamId teamId)
    {
        return teamId == TeamId.Team1 ? team1.CurrentGamePoints >= 4 && team1.CurrentGamePoints - team2.CurrentGamePoints >= 2
                                      : team2.CurrentGamePoints >= 4 && team2.CurrentGamePoints - team1.CurrentGamePoints >= 2;
    }

    private class TeamScore : ITeamScore
    {
        public TeamScore()
        {
            CurrentGamePoints = 0;
            GamesWon = 0;
            SetsWon = 0;
            MatchWon = false;
        }

        public int CurrentGamePoints { get; private set; }

        public int SetsWon { get; private set; }

        public bool MatchWon { get; private set; }

        public int GamesWon { get; private set; }

        internal void ResetScore() => CurrentGamePoints = 0;

        internal void ScoreGame() => GamesWon++;

        internal void ScorePoint() => CurrentGamePoints++;

        internal void ScoreSet() => SetsWon++;
    }
}