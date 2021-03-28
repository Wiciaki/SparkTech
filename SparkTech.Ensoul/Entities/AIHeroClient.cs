namespace SparkTech.Ensoul.Entities
{
    using SDK.Entities;

    public class AIHeroClient<TEntity> : AIBaseClient<TEntity>, IHero where TEntity : EnsoulSharp.AIHeroClient
    {
        public AIHeroClient(TEntity entity) : base(entity)
        { }

        public int Level => this.Entity.Level;
        public float Experience => this.Entity.Experience;
        public float ExpToCurrentLevel => this.Entity.ExpToCurrentLevel;
        public float ExpToNextLevel => this.Entity.ExpToNextLevel;
        public int MinionsKilled => this.Entity.MinionsKilled;
        public int NeutralMinionsKilled => this.Entity.NeutralMinionsKilled;
        public int ChampionsKilled => this.Entity.ChampionsKilled;
        public int Deaths => this.Entity.Deaths;
        public int Assists => this.Entity.Assists;
    }
}