namespace SparkTech.SDK.Entities
{
    using SharpDX;

    public interface IMissile : IGameObject
    {
        IUnit Caster { get; }

        Vector3 StartPosition { get; }

        Vector3 EndPosition { get; }

        IGameObject Target { get; }

        SpellSlot Slot { get; }

        bool IsComplete { get; }

        float Width { get; }

        float Speed { get; }

        bool IsDestroyed { get; }

        bool IsVisible { get; }
    }
}