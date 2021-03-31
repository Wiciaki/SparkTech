namespace SparkTech.SDK.DamageLibrary
{
    using Entities;
    using Modules;
    using Implementation;

    public static class DamageLibraryService
    {
        #region Static Fields

        public static readonly Picker<IDamageLibrary> Picker = new Picker<IDamageLibrary>(new DamageLibraryImpl());

        #endregion

        #region Public Methods and Operators

        public static float GetAutoAttackDamage(this IUnit source, IUnit target, bool includePassive = true)
        {
            return Picker.Current.GetAutoAttackDamage(source, target, includePassive);
        }

        public static float GetSpellDamage(this IHero source, IUnit target, SpellSlot slot, int stage = 0)
        {
            return Picker.Current.GetSpellDamage(source, target, slot, stage);
        }

        #endregion
    }
}
