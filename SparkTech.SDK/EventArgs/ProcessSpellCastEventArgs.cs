namespace SparkTech.SDK.EventArgs
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;

    public class ProcessCastEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly ISpell Spell;

        public readonly IGameObject Target;

        public readonly int Level;

        public readonly Vector3 Start, End;

        public readonly int CastedSpellCount;

        public readonly SpellSlot Slot;

        public readonly int MissileNetworkId;

        public ProcessCastEventArgs(IUnit source, ISpell spell, IGameObject target, int level, Vector3 start, Vector3 end, int counter, SpellSlot slot, int missileNetworkId)
        {
            this.Source = source;

            this.Spell = spell;

            this.Level = level;

            this.Start = start;

            this.End = end;

            this.Target = target;

            this.CastedSpellCount = counter;

            this.Slot = slot;

            this.MissileNetworkId = missileNetworkId;
        }
    }
}