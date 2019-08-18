namespace SparkTech.SDK.Entities
{
    using System;

    using SparkTech.SDK.Platform.API;

    public static class Spellbook
    {
        public static event Action<SpellbookCastSpellEventArgs> OnCastSpell;
        
        public static event Action<SpellbookStopCastEventArgs> OnStopCast;
        
        public static event Action<SpellbookUpdateChargedSpell> OnUpdateChargedSpell;

        internal static void Initialize(ISpellbook spellbook)
        {
            spellbook.CastSpell = OnCastSpell.SafeInvoke;
            spellbook.StopCast = OnStopCast.SafeInvoke;
            spellbook.UpdateChargedSpell = OnUpdateChargedSpell.SafeInvoke;
        }
    }
}