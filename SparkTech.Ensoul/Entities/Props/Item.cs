namespace SparkTech.Ensoul.Entities.Props
{
    using SparkTech.SDK.Entities;

    public class Item : PropWrapper<EnsoulSharp.InventorySlot>, IItem
    {
        public Item(EnsoulSharp.InventorySlot item) : base(item)
        { }

        public SpellSlot SpellSlot => (SpellSlot)this.Prop.SpellSlot;
        public int Slot => this.Prop.Slot;
        public ItemId Id => (ItemId)this.Prop.Id;
        public int CountInSlot => this.Prop.CountInSlot;
        public float PurchaseTime => this.Prop.PurchaseTime;
        public int SpellCharges => this.Prop.SpellCharges;
        public float InvestedGoldAmount => this.Prop.InvestedGoldAmount;
        public IItemData ItemData => EntityConverter.Convert(this.Prop.IData);
    }
}