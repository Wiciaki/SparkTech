namespace SparkTech.Ensoul.Entities
{
    using SparkTech.Ensoul.Entities.Props;
    using SparkTech.SDK.Entities;

    internal static class EntityConverter
    {
        internal static int T(int? id) => id ?? -1;

        internal static IBuff Convert(EnsoulSharp.BuffInstance buff)
        {
            return new Buff(buff);
        }

        internal static ISpell Convert(EnsoulSharp.SpellDataInst buff)
        {
            return new Spell(buff);
        }

        internal static ISpellbook Convert(EnsoulSharp.Spellbook spellbook)
        {
            return new Spellbook(spellbook);
        }

        internal static ISpellData Convert(EnsoulSharp.SpellData spelldata)
        {
            return new SpellData(spelldata);
        }

        internal static IItem Convert(EnsoulSharp.InventorySlot item)
        {
            return new Item(item);
        }

        internal static IItemData Convert(EnsoulSharp.ItemData itemdata)
        {
            return new ItemData(itemdata);
        }
    }
}
