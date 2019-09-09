namespace SparkTech.SDK.EventArgs
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;

    public class NewPathEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly Vector3[] Path;

        public readonly bool IsDash;

        public readonly float Speed;

        public NewPathEventArgs(IUnit source, Vector3[] path, bool isDash, float speed)
        {
            this.Source = source;

            this.Path = path;

            this.IsDash = isDash;

            this.Speed = speed;
        }
    }
}