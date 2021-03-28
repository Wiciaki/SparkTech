namespace SparkTech.Ensoul.Entities.Props
{
    public abstract class PropWrapper<T>
    {
        protected readonly T Prop;

        protected PropWrapper(T prop)
        {
            this.Prop = prop;
        }
    }
}