namespace Kata.TennisGame.Application.Tests;

public class GameShould
{
    /*
     game should:
        * start with no players -- DONE
        * up to 4 players in total -- DONE
        * have up to 2 teams -- DONE
            * each team can have either 1 or 2 players -- DONE

        * keep track of history of scores -- IN PROGRESS
        * scoring: Game, set, match
            * Game:
                * love, 15, 30, 40, advantage
                * A game is won by the first player to have won at least four points in total and at least two points more than the opponent -- DONE
                * deuce happens when: If at least three points have been scored by each player, making the player's scores equal at 40 apiece -- DONE
                * advantage when:  If at least three points have been scored by each side and a player has one more point than his opponent, the score of the game is "advantage" for the player in the lead -- DONE
                * serving player score is called first, then the non-serving player
                * after every odd-numbered Game, the server changes
                    * within a team, the server is alternated between the team members

            * Set:
                * it is a series of Games -- DONE
                * a Set is won when one team wins at least 6 Games AND 2 Games more than the opponent, if 6-5 then another Game is played -- DONE
                * The final score in sets is always read with the winning player's score first, e.g. "6–2, 4–6, 6–0, 7–5"
                * if a Set reaches a 6-6 Games won by each team then a TieBreak Game is in play -- DONE
                * TieBreak: -- DONE
                    * to be won, it requires a player to win at least 7 points and at least 2 points more than opponent -- DONE
            * Match:
                * a sequence of sets
                * it is composed by a best of 5 or 3 sets
                    * the first team to win 2 sets outta 3 or 3 sets outta 5, wins the match
                * when a match is won, we refer to the winning team with the phrase: game, set, match <winning team/player>

        * determine the 1st server by a random toss coin
        * shots may:
            * be outta bounds (fault)
                * if fault 2x the receiver wins a point
            * be within the court and score a point
            * hit the net

     */

    public class GameStartUp
    {
        [Fact]
        public void StartWithNoPlayersAndNoTeams()
        {
            var game = Game.CreateGame();

            Assert.Equal(typeof(NoTeam), game.Team1.GetType());
            Assert.Equal(typeof(NoTeam), game.Team2.GetType());

            Assert.Equal(typeof(NoPlayer), game.Team1.Player1.GetType());
            Assert.Equal(typeof(NoPlayer), game.Team1.Player2.GetType());

            Assert.Equal(typeof(NoPlayer), game.Team2.Player1.GetType());
            Assert.Equal(typeof(NoPlayer), game.Team2.Player2.GetType());
        }

        [Fact]
        public void StartWithNoScores()
        {
            var game = Game.CreateGame();

            Assert.Equal(0, game.Score.Team1.CurrentGamePoints);
            Assert.Equal(0, game.Score.Team1.GamesWon);
            Assert.Equal(0, game.Score.Team1.SetsWon);
            Assert.False(game.Score.Team1.MatchWon);

            Assert.Equal(0, game.Score.Team2.CurrentGamePoints);
            Assert.Equal(0, game.Score.Team2.GamesWon);
            Assert.Equal(0, game.Score.Team2.SetsWon);
            Assert.False(game.Score.Team2.MatchWon);
        }

        [Fact]
        public void StartWithNotStartedStatus()
        {
            var game = Game.CreateGame();

            Assert.Equal(GameStatus.NotStarted, game.Status);
        }

        [Fact]
        public void BeAbleToStartHavingTwoPlayersAtMinimum()
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();
            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            game.Start();

            Assert.Equal(GameStatus.Started, game.Status);
        }

        [Fact]
        public void BeAbleToStartHavingFourPlayersAtMaximum()
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Doubles.CreateValidTeam_1_WithValidPlayers();
            var team2 = TeamProvider.Doubles.CreateValidTeam_2_WithValidPlayers();

            game.AddTeams(team1, team2);

            game.Start();

            Assert.Equal(GameStatus.Started, game.Status);
        }
    }

    public class GameTeamsSetup
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void NotBeAbleToAddATeamWithInvalidPlayerCount(int playerCount)
        {
            var game = Game.CreateGame();

            ITeam team1;
            ITeam team2;
            switch (playerCount)
            {
                case 0:
                    team1 = TeamProvider.Singles.CreateInvalidTeam_1_WithNoPlayer();
                    team2 = TeamProvider.Singles.CreateInvalidTeam_2_WithNoPlayer();
                    break;

                case 1:
                    team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();
                    team2 = TeamProvider.Singles.CreateInvalidTeam_2_WithNoPlayer();
                    break;

                case 3:
                    team1 = TeamProvider.Doubles.CreateValidTeam_1_WithValidPlayers();
                    team2 = TeamProvider.Doubles.CreateInvalidTeam_2_WithOnePlayer();
                    break;

                default:
                    throw new ArgumentOutOfRangeException("the test cases can contemplate only 1, 3, or 0 players");
            }

            Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
        }

        [Fact]
        public void NotBeAbleToAddTeamsIfTeamsAlreadyExist()
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();
            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
        }

        [Fact]
        public void NotBeAbleToAddTwoTeamsWithSameName()
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();
            var team2 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData((string)null)]
        public void NotBeAbleToAddTwoTeamsWithEmptyNames(string invalidName)
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateTeamWithValidPlayer(invalidName);
            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            Assert.Throws<InvalidOperationException>(() => game.AddTeams(team1, team2));
        }
    }

    public class GameScoring
    {
        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 1, 1)]
        [InlineData(3, 1, 1)]
        [InlineData(1, 1, 2)]
        [InlineData(2, 1, 2)]
        [InlineData(3, 1, 2)]
        [InlineData(1, 2, 1)]
        [InlineData(2, 2, 1)]
        [InlineData(3, 2, 1)]
        [InlineData(1, 2, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(3, 2, 2)]
        public void RecordPointWhenPlayerScoresAPoint(int successfulHitsNeeded, int teamScoring, int playerScoring)
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Doubles.CreateValidTeam_1_WithValidPlayers();

            var team2 = TeamProvider.Doubles.CreateValidTeam_2_WithValidPlayers();
            game.AddTeams(team1, team2);
            var actualSuccessfulHits = 0;

            while (actualSuccessfulHits < successfulHitsNeeded)
            {
                if (teamScoring == 1)
                {
                    if (playerScoring == 1)
                    {
                        if (team1.Player1.AttemptHitBall())
                            actualSuccessfulHits++;
                    }
                    else
                    {
                        if (team1.Player2.AttemptHitBall())
                            actualSuccessfulHits++;
                    }
                }
                else
                {
                    if (playerScoring == 1)
                    {
                        if (team2.Player1.AttemptHitBall())
                            actualSuccessfulHits++;
                    }
                    else
                    {
                        if (team2.Player2.AttemptHitBall())
                            actualSuccessfulHits++;
                    }
                }
            }

            Assert.True(actualSuccessfulHits > 0);

            if (teamScoring == 1)
                Assert.Equal(actualSuccessfulHits, game.Score.Team1.CurrentGamePoints);
            else
                Assert.Equal(actualSuccessfulHits, game.Score.Team2.CurrentGamePoints);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void ReturnScoreAsDeuceWhenBothTeamsHave3PointsOrMore(int deuceScore)
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            var player1 = team1.Player1;
            var player2 = team2.Player1;

            // increase 1 point at a time for both players to prevent any player from winning any games
            player1.AddPointsTo2PlayersSimultaneusly(player2, deuceScore);

            Assert.Equal("Deuce", game.Score.CurrentStatus);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void RecordAdvantagePointWhenPlayerWinsAfterADeuce(int winningTeam)
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            var player1 = team1.Player1;
            var player2 = team2.Player1;

            var score = game.Score;

            if (winningTeam == 1)
            {
                // increase 1 point at a time for both players to prevent any player from winning any games
                player1.AddPointsTo2PlayersSimultaneusly(player2, 3);
                var deuceStatusHappened = game.Score.CurrentStatus;

                player1.AddPoints(1); //advantage for player

                Assert.Equal("Deuce", deuceStatusHappened);
                Assert.Equal($"Advantage - {score.Team2.CurrentGamePoints}", game.Score.CurrentStatus);
            }
            else
            {
                // increase 1 point at a time for both players to prevent any player from winning any games
                player1.AddPointsTo2PlayersSimultaneusly(player2, 6);
                var deuceStatusHappened = game.Score.CurrentStatus;

                player2.AddPoints(1);//advantage for player

                Assert.Equal("Deuce", deuceStatusHappened);
                Assert.Equal($"Advantage - {score.Team1.CurrentGamePoints}", game.Score.CurrentStatus);
            }
        }

        [Fact]
        public void GoBackToDeuceIfAdvantageIsLost()
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            var player1 = team1.Player1;
            var player2 = team2.Player1;

            var deuceScore = 3;

            player1.AddPoints(deuceScore);
            player2.AddPoints(deuceScore + 1);

            var advantageStatus = game.Score.CurrentStatus;
            player1.AddPoints(1);

            Assert.Equal("Advantage - 3", advantageStatus);
            Assert.Equal("Deuce", game.Score.CurrentStatus);
        }

        [Fact]
        public void RecordGameWonWhenPlayerScoresFourGamePointsAndTwoMoreThanOpponent()
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            var player1 = team1.Player1;
            var player2 = team2.Player1;

            var deuceScore = 3;

            player1.AddPointsTo2PlayersSimultaneusly(player2, deuceScore);

            player1.AddPoints(2);//hits 2 points over the opponent, wins a game, score is reset to 0 - 0

            Assert.Equal("0 - 0", game.Score.CurrentStatus);

            Assert.Equal(0, game.Score.Team1.CurrentGamePoints);
            Assert.Equal(0, game.Score.Team2.CurrentGamePoints);

            Assert.Equal(1, game.Score.Team1.GamesWon);
            Assert.Equal(0, game.Score.Team2.GamesWon);

            Assert.Equal(0, game.Score.Team1.SetsWon);
            Assert.Equal(0, game.Score.Team2.SetsWon);

            Assert.False(game.Score.Team1.MatchWon);
            Assert.False(game.Score.Team2.MatchWon);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(1, 0, false)]
        [InlineData(2, 0, false)]
        [InlineData(3, 0, false)]
        [InlineData(4, 0, false)]
        [InlineData(5, 0, false)]
        [InlineData(6, 0, true)]
        [InlineData(1, 1, false)]
        [InlineData(2, 1, false)]
        [InlineData(3, 1, false)]
        [InlineData(4, 1, false)]
        [InlineData(5, 1, false)]
        [InlineData(6, 1, true)]
        [InlineData(6, 2, true)]
        [InlineData(6, 3, true)]
        [InlineData(6, 4, true)]
        [InlineData(6, 5, false)]
        [InlineData(6, 6, false)]
        [InlineData(7, 5, true)]
        public void RecordSetWonWhenPlayerWinsFourGamesAndTwoMoreThanOpponent(int team1GamesWon, int team2GamesWon, bool expectedSetsWonByTeam1)
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            var player1 = team1.Player1;
            var player2 = team2.Player1;

            var team1GameToWin = 0;
            var team2GameToWin = 0;
            while (team1GameToWin <= team1GamesWon || team2GameToWin <= team2GamesWon)
            {
                team1GameToWin++;
                team2GameToWin++;

                if (team1GameToWin <= team1GamesWon)
                    player1.WinGame();

                if (team2GameToWin <= team2GamesWon)
                    player2.WinGame();
            }

            Assert.Equal(0, game.Score.Team1.CurrentGamePoints);
            Assert.Equal(0, game.Score.Team2.CurrentGamePoints);

            Assert.Equal(team1GamesWon, game.Score.Team1.GamesWon);
            Assert.Equal(team2GamesWon, game.Score.Team2.GamesWon);

            Assert.Equal(expectedSetsWonByTeam1, game.Score.Team1.SetsWon > game.Score.Team2.SetsWon);

            if (expectedSetsWonByTeam1)
                Assert.Equal(1, game.Score.Team1.SetsWon);

            Assert.False(game.Score.Team1.MatchWon);
            Assert.False(game.Score.Team2.MatchWon);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, true)]
        public void ReachATieBreakWhenBothPlayersWinSixSets(int gamesToWin, bool shouldBeTieBreak)
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            var player1 = team1.Player1;
            var player2 = team2.Player1;

            player1.WinManyGamesSimultaneously(player2, gamesToWin);

            if (shouldBeTieBreak)
                Assert.Equal("0 - 0 Tie Break", game.Score.CurrentStatus);
            else
                Assert.Equal("0 - 0", game.Score.CurrentStatus);

            Assert.Equal(0, game.Score.Team1.CurrentGamePoints);
            Assert.Equal(0, game.Score.Team2.CurrentGamePoints);

            Assert.Equal(gamesToWin, game.Score.Team1.GamesWon);
            Assert.Equal(gamesToWin, game.Score.Team2.GamesWon);

            Assert.False(game.Score.Team1.MatchWon);
            Assert.False(game.Score.Team2.MatchWon);
        }

        //* to be won, it requires a player to win at least 7 points and at least 2 points more than opponent

        [Theory]
        [InlineData(7, 0, true)]
        [InlineData(6, 4, false)]
        [InlineData(7, 5, true)]
        [InlineData(7, 6, false)]
        [InlineData(8, 6, true)]
        public void WinATieBreakByHavingATeamWinningAtLeastSevenPointsAndTwoPointsOverTheOpponent(int team1ScoredPoints, int team2ScoredPoints, bool shouldTeam1WinTheTieBreak)
        {
            var game = Game.CreateGame();

            var team1 = TeamProvider.Singles.CreateValidTeam_1_WithValidPlayer();

            var team2 = TeamProvider.Singles.CreateValidTeam_2_WithValidPlayer();

            game.AddTeams(team1, team2);

            var player1 = team1.Player1;
            var player2 = team2.Player1;

            var tieBreakOnGame = 6;
            player1.WinManyGamesSimultaneously(player2, tieBreakOnGame);

            player1.ScorePointsForTwoPlayersSequentially(player2, team1ScoredPoints, team2ScoredPoints);

            if (shouldTeam1WinTheTieBreak)
            {
                Assert.Equal(1, game.Score.Team1.SetsWon);
                Assert.Equal(0, game.Score.Team1.CurrentGamePoints);
                Assert.Equal(0, game.Score.Team2.CurrentGamePoints);
            }
            else
            {
                Assert.Equal(0, game.Score.Team1.SetsWon);
                Assert.Equal(team1ScoredPoints, game.Score.Team1.CurrentGamePoints);
                Assert.Equal(team2ScoredPoints, game.Score.Team2.CurrentGamePoints);
            }

            var expectedGamesWonByTeam1 = shouldTeam1WinTheTieBreak ? tieBreakOnGame + 1 : tieBreakOnGame;
            Assert.Equal(expectedGamesWonByTeam1, game.Score.Team1.GamesWon); ;
            Assert.Equal(tieBreakOnGame, game.Score.Team2.GamesWon);

            Assert.False(game.Score.Team1.MatchWon);
            Assert.False(game.Score.Team2.MatchWon);
        }
    }
}

public class PendingImplementations
{
    [Fact]
    public void ReachATieBreakWhenBothPlayersWinSixSets()
    {
        Assert.False(true);
    }

    [Fact]
    public void RecordMatchWhenPlayerWinsMatch()
    {
        Assert.False(true);
    }

    [Fact]
    public void KeepTrackOfServerPlayer()
    {
        Assert.False(true);
    }

    [Fact]
    public void ReportScoreAccordingToServerPlayerAndWinningPlayer()

    {
        Assert.False(true);
    }

    //Teams tests
    //[Fact]
    //public void NotBeAbleToCreateDoublesWithOnlyOnePlayer()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
    //[Fact]
    //public void NotBeAbleToCreateSinglesWithNoPlayer()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
    //[Fact]
    //public void NotBeAbleToAddTwoPlayersWithSameName()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
    //[Fact]
    //public void NotBeAbleToAddTwoPlayersWithEmptyNames()
    //{
    //    var team = new Team();
    //    Assert.True(false);

    //}
}