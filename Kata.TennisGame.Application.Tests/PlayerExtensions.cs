﻿namespace Kata.TennisGame.Application.Tests;

public static class PlayerExtensions
{

    public static void ScorePointsForTwoPlayersSequentially(this IPlayer player1, IPlayer player2, int player1ScoredPoints, int player2ScoredPoints)
    {



        var actualTeam1ScoredPoints = 0;
        var actualTeam2ScoredPoints = 0;
        while (actualTeam1ScoredPoints <= player1ScoredPoints || actualTeam2ScoredPoints <= player2ScoredPoints)
        {
            actualTeam1ScoredPoints++;
            actualTeam2ScoredPoints++;

            if (actualTeam1ScoredPoints <= player1ScoredPoints)
                player1.AddPoints(1);

            if (actualTeam2ScoredPoints <= player2ScoredPoints)
                player2.AddPoints(1);
        }

    }


    public static void WinManyGamesSimultaneously(this IPlayer player1, IPlayer player2, int gameCount)
    {

        for (var i = 0; i < gameCount; i++)
        {
            player1.WinGame();
            player2.WinGame();
        }



    }

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

    public static void AddPointsTo2PlayersSimultaneusly(this IPlayer player1, IPlayer player2, int pointsToAdd)
    {
        for (var i = 0; i < pointsToAdd; i++)
        {
            player1.AddPoints(1);
            player2.AddPoints(1);
        }
    }
}
