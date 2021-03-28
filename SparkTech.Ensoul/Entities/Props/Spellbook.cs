namespace SparkTech.Ensoul.Entities.Props
{
    using System;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventData;

    public class Spellbook : PropWrapper<EnsoulSharp.Spellbook>, ISpellbook
    {
        public Spellbook(EnsoulSharp.Spellbook spellbook) : base(spellbook)
        { }

        public ISpell[] Spells => Array.ConvertAll(this.Prop.Spells, EntityConverter.Convert);
        public IUnit Owner => ObjectManager.GetById<IUnit>(this.Prop.Owner.NetworkId);

        public ProcessSpellCastEventArgs ActiveSpell
        {
            get
            {
                var args = this.Prop.ActiveSpell;

                return new ProcessSpellCastEventArgs(this.Prop.Owner.NetworkId, EntityConverter.Convert(args.SData), args.Level, args.Start, args.To, args.End, EntityConverter.T(args.Target?.NetworkId), args.MissileNetworkId, args.CastTime, args.TotalTime, (SpellSlot)args.Slot);
            }
        }

        public bool IsStopped => this.Prop.IsStopped;
        public bool IsCastingSpell => this.Prop.IsCastingSpell;
        public bool IsAutoAttack => this.Prop.IsAutoAttack;
        public bool IsCharging => this.Prop.IsCharging;
        public bool IsChanneling => this.Prop.IsChanneling;
        public float CastTime => this.Prop.CastTime;
        public bool IsWindingUp => this.Prop.IsWindingUp;
        public bool SpellWasCast => this.Prop.SpellWasCast;
        public float CastEndTime => this.Prop.CastEndTime;

        public SpellState CanUseSpell(SpellSlot slot)
        {
            return (SpellState)this.Prop.CanUseSpell((EnsoulSharp.SpellSlot)slot);
        }

        public ISpell GetSpell(SpellSlot slot)
        {
            return EntityConverter.Convert(this.Prop.GetSpell((EnsoulSharp.SpellSlot)slot));
        }
    }
}