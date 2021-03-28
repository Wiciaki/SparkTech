namespace SparkTech.Ensoul.Entities.Props
{
    using SparkTech.SDK.Entities;

    public class ItemData : PropWrapper<EnsoulSharp.ItemData>, IItemData
    {
        public ItemData(EnsoulSharp.ItemData itemdata) : base(itemdata)
        { }

        public int RequiredLevel => this.Prop.RequiredLevel;
        public ItemId Id => (ItemId)this.Prop.Id;
        public int MaxStack => this.Prop.MaxStack;
        public int Price => this.Prop.Price;
        public bool UsableInStore => this.Prop.UsableInStore;
        public bool CanBeSold => this.Prop.CanBeSold;
        public float SellBackPercentage => this.Prop.SellBackPercentage;
        public string DisplayName => this.Prop.DisplayName;
        public string TranslatedDisplayName => this.Prop.TranslatedDisplayName;
        public string SpellName => this.Prop.SpellName;
    }
}