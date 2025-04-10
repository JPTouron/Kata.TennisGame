namespace Kata.TennisGame.Application;

internal interface IPlayerProfile
{
    bool IsHitSuccessful();
}

internal class PlayerProfile : IPlayerProfile
{
    private int accuracy;

    private int consistency;

    private PlayerProfile(int accuracy, int consistency)
    {
        this.accuracy = accuracy;
        this.consistency = consistency;
    }

    public static IPlayerProfile HighLevel()
    {
        return new HighProfilePlayer();
    }

    public static IPlayerProfile MediumLevel()
    {
        return new MediumProfilePlayer();
    }

    public static IPlayerProfile LowLevel()
    {
        return new LowProfilePlayer();
    }

    public bool IsHitSuccessful()
    {
        // Calculate hit probability based on player's accuracy and consistency
        var hitProbability = accuracy * 0.7 + consistency * 0.3;

        var isHitSuccessful = RandomFactor() <= hitProbability;

        return isHitSuccessful;
    }

    private double RandomFactor()
    {
        Random RandomGenerator = new();
        var randomFactor = RandomGenerator.NextDouble() * 100;
        return randomFactor;
    }

    private class HighProfilePlayer : PlayerProfile
    {
        public HighProfilePlayer() : base(90, 85)
        {
        }
    }

    private class MediumProfilePlayer : PlayerProfile
    {
        public MediumProfilePlayer() : base(70, 75)
        {
        }
    }

    private class LowProfilePlayer : PlayerProfile
    {
        public LowProfilePlayer() : base(50, 55)
        {
        }
    }
}