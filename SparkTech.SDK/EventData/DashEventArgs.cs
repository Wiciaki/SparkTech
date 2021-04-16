namespace SparkTech.SDK.EventData
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;

    public class DashEventArgs : EventArgs, IEventArgsSource<IHero>
    {
        public int SourceId { get; }

        public IHero Source => ObjectManager.GetById<IHero>(this.SourceId);

        public Vector3[] Path { get; }

        public float Speed { get; }

        public float StartTime { get; }

        public float EndTime { get; }

        public Vector3 StartPosition { get; }

        public Vector3 EndPosition { get; }

        public DashEventArgs(int sourceId, Vector3[] path, float speed, Vector3 startPosition, Vector3 endPosition, float startTime, float endTime)
        {
            this.SourceId = sourceId;
            this.Path = path;
            this.Speed = speed;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }
    }
}