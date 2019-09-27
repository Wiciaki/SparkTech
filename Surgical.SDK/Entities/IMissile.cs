namespace Surgical.SDK.Entities
{
    using SharpDX;

    public interface IMissile : IGameObject
    {
        ISpellData Spell { get; }

        IUnit Caster { get; }

        Vector3 StartPosition { get; }

        Vector3 EndPosition { get; }

        IGameObject Target { get; }

        SpellSlot Slot { get; }
    }
}