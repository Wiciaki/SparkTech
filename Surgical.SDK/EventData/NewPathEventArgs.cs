namespace Surgical.SDK.EventData
{
    using System;

    using SharpDX;

    using Surgical.SDK.Entities;

    public class NewPathEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public IUnit Source { get; }

        public Vector3[] Path { get; }

        public bool IsDash { get; }

        public float Speed { get; }

        public NewPathEventArgs(IUnit source, Vector3[] path, bool isDash, float speed)
        {
            this.Source = source;

            this.Path = path;

            this.IsDash = isDash;

            this.Speed = speed;
        }
    }
}