namespace SparkTech.SDK.EventData
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;

    public class NewPathEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public Vector3[] Path { get; }

        public bool IsDash { get; }

        public float Speed { get; }

        public NewPathEventArgs(int sourceId, Vector3[] path, bool isDash, float speed)
        {
            this.SourceId = sourceId;
            this.Path = path;
            this.IsDash = isDash;
            this.Speed = speed;
        }
    }
}