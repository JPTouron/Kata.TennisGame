namespace Kata.TennisGame.Application.Tests;

public static class PlayerExtensions
{
    public static void WinGame(this IPlayer player)
    {
        player.AddPoints(4);
    }

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
