namespace SparkTech.SDK.Entities.EventArgs
{
    public interface ISourcedEventArgs<out T>
    {
        T Source { get; }
    }
}