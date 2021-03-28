namespace SparkTech.SDK.EventData
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;

    public class ProcessSpellCastEventArgs : EventArgs, IEventArgsSource<IUnit>, IEventArgsTarget<IGameObject>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public int TargetId { get; }

        public IGameObject Target => ObjectManager.GetById(this.TargetId);

        public int MissileId { get; }

        public IMissile Missile => ObjectManager.GetById<IMissile>(this.MissileId);

        public ISpellData SpellData { get; }

        public int Level { get; }

        public Vector3 Start { get; }

        public Vector3 To { get; }

        public Vector3 End { get; }

        public float CastTime { get; }

        public float TotalTime { get; }

        public SpellSlot Slot { get; }

        public ProcessSpellCastEventArgs(int sourceId, ISpellData spellData, int level, Vector3 start, Vector3 to, Vector3 end, int targetId, int missileId, float castTime, float totalTime, SpellSlot slot)
        {
            this.SourceId = sourceId;
            this.SpellData = spellData;
            this.Level = level;
            this.Start = start;
            this.To = to;
            this.End = end;
            this.TargetId = targetId;
            this.MissileId = missileId;
            this.CastTime = castTime;
            this.TotalTime = totalTime;
            this.Slot = slot;
        }
    }
}