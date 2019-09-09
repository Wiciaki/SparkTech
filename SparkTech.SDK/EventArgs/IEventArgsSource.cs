namespace SparkTech.SDK.EventArgs
{
    public interface ISourcedEventArgs<out T>
    {
        T Source { get; }
    }
}