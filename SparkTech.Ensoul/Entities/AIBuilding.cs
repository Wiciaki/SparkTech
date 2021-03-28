namespace SparkTech.Ensoul.Entities
{
    using SparkTech.SDK.Entities;

    public class AIBuilding<TEntity> : AttackableUnit<TEntity>, IBuilding where TEntity : EnsoulSharp.BuildingClient
    {
        public AIBuilding(TEntity entity) : base(entity)
        { }
    }
}