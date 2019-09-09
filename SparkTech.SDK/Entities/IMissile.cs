namespace SparkTech.SDK.Entities
{
    using SharpDX;

    public interface IMissile
    {
        ISpell Spell { get; }

        IUnit Caster { get; }

        Vector3 StartPosition { get; }

        Vector3 EndPosition { get; }

        IGameObject Target { get; }

        SpellSlot Slot { get; }
    }
}