namespace SparkTech.SDK.API
{
    using System;

    using SparkTech.SDK.EventData;

    public interface IUserInputAPI
    {
        Action<WndProcEventArgs> WndProcess { get; set; }
    }
}