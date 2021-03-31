namespace SparkTech.SDK.EventData
{
    using SparkTech.SDK.Entities;

    using System;

    public class PropertyChangeEventArgs<T> : EventArgs, IEventArgsSource<IGameObject>
    {
        public int SourceId { get; }

        public IGameObject Source => ObjectManager.GetById(this.SourceId);

        public string Property { get; }

        public T NewValue { get; }

        public T OldValue { get; }

        public PropertyChangeEventArgs(int sourceId, string property, T oldValue, T newValue)
        {
            this.SourceId = sourceId;
            this.Property = property;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}