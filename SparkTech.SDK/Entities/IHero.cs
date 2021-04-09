namespace SparkTech.SDK.Entities
{
    public interface IHero : IUnit
    {
        int Level { get; }

        int EvolvePoints { get; }

        int SpellTrainingPoints { get; }

        float Experience { get; }

        float ExpToCurrentLevel { get; }

        float ExpToNextLevel { get; }

        int MinionsKilled { get; }

        int NeutralMinionsKilled { get; }

        int ChampionsKilled { get; }

        int Deaths { get; }

        int Assists { get; }
    }
}