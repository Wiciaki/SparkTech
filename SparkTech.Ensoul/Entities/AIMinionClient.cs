namespace SparkTech.Ensoul.Entities
{
    using SDK.Entities;

    public class AIMinionClient<TEntity> : AIBaseClient<TEntity>, IMinion where TEntity : EnsoulSharp.AIMinionClient
    {
        public AIMinionClient(TEntity entity) : base(entity)
        { }


    }
}