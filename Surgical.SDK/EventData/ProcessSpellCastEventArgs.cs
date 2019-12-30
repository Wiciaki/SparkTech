namespace Surgical.SDK.EventData
{
    using System;

    using SharpDX;

    using Surgical.SDK.Entities;

    public class ProcessCastEventArgs : EventArgs, IEventArgsSource<IUnit>, IEventArgsTarget<IGameObject>
    {
        public IUnit Source { get; }

        public ISpellData Spell { get; }

        public IGameObject? Target { get; }

        public int Level { get; }

        public Vector3 Start { get; }

        public Vector3 End { get; }

        public int CastedSpellCount { get; }

        public SpellSlot Slot { get; }

        public int MissileNetworkId { get; }

        public ProcessCastEventArgs(IUnit source, ISpellData spell, IGameObject target, int level, Vector3 start, Vector3 end, int counter, SpellSlot slot, int missileNetworkId)
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