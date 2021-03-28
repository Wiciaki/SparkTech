namespace SparkTech.Ensoul.Entities.Props
{
    using SparkTech.SDK.Entities;

    public class Spell : PropWrapper<EnsoulSharp.SpellDataInst>, ISpell
    {
        public Spell(EnsoulSharp.SpellDataInst spell) : base(spell)
        { }

        public int Level => this.Prop.Level;
        public bool Learned => this.Prop.Learned;
        public float CooldownExpires => this.Prop.CooldownExpires;
        public int NumericalDisplay => this.Prop.NumericalDisplay;
        public int Ammo => this.Prop.Ammo;
        public SpellToggleState ToggleState => (SpellToggleState)this.Prop.ToggleState;
        public float Cooldown => this.Prop.Cooldown;
        public string Name => this.Prop.Name;
        public float ManaCost => this.Prop.ManaCost;
        public SpellSlot Slot => (SpellSlot)this.Prop.Slot;
        public SpellState State => (SpellState)this.Prop.State;
        public float[] TooltipVars => this.Prop.TooltipVars;
        public int IconUsed => this.Prop.IconUsed;
        public ISpellData SpellData => EntityConverter.Convert(this.Prop.SData);
    }
}