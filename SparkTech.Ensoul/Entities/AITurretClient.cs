namespace SparkTech.Ensoul.Entities
{
    using SparkTech.SDK.Entities;

    public class AITurretClient<TEntity> : AIBaseClient<TEntity>, ITurret where TEntity : EnsoulSharp.AITurretClient
    {
        public AITurretClient(TEntity entity) : base(entity)
        { }
    }
}