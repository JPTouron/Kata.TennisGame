namespace Kata.TennisGame.Application.Tests;



public static class lPayerExtensions
{

    public static void AddPoints(this IPlayer player, int successfulHitsNeeded)
    {

        var actualSuccessfulHits = 0;

        while (actualSuccessfulHits < successfulHitsNeeded)
        {

            if (player.AttemptHitBall())
                actualSuccessfulHits++;


        }


    }

    public static void AddPointsTo2PlayersSimoultaneusly(this IPlayer player1, IPlayer player2, int pointsToAdd)
    {

        for (var i = 0; i < pointsToAdd; i++)
        {
            player1.AddPoints(1);
            player2.AddPoints(1);
        }
    }



}

public static class PlayerProvider
{

    public static IPlayer CreateValidPlayer1() { return Player.CreateHighProfilePlayer("player 1"); }
    public static IPlayer CreateValidPlayer2() { return Player.CreateHighProfilePlayer("player 2"); }





}

public static class TeamProvider
{

    public static class Singles
    {




        public static ITeam CreateValidTeam_1_WithValidPlayer()
        {
            return Team.CreateSinglesTeam("single Team 1", PlayerProvider.CreateValidPlayer1());
        }
        public static ITeam CreateValidTeam_2_WithValidPlayer()
        {
            return Team.CreateSinglesTeam("single Team 2", PlayerProvider.CreateValidPlayer2());
        }


        public static ITeam CreateInvalidTeam_1_WithNoPlayer()
        {
            return Team.CreateSinglesTeam("single Team 1", Player.CreateEmpty());
        }
        public static ITeam CreateInvalidTeam_2_WithNoPlayer()
        {
            return Team.CreateSinglesTeam("single Team 2", Player.CreateEmpty());

        }
        public static ITeam CreateTeamWithValidPlayer(string teamName)
        {
            return Team.CreateSinglesTeam(teamName, PlayerProvider.CreateValidPlayer1());
        }

    }
    public static class Doubles
    {

        public static ITeam CreateValidTeam_1_WithValidPlayers()
        {
            return Team.CreateDoublesTeam("single Team 1", PlayerProvider.CreateValidPlayer1(), PlayerProvider.CreateValidPlayer2());
        }
        public static ITeam CreateValidTeam_2_WithValidPlayers()
        {
            return Team.CreateDoublesTeam("single Team 2", PlayerProvider.CreateValidPlayer1(), PlayerProvider.CreateValidPlayer2());
        }

        public static ITeam CreateInvalidTeam_2_WithOnePlayer()
        {
            return Team.CreateDoublesTeam("double Team 2", PlayerProvider.CreateValidPlayer1(), Player.CreateEmpty());
        }

    }

}
