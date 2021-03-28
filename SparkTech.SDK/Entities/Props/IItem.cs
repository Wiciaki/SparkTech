namespace SparkTech.SDK.Entities
{
    public interface IItem
    {
        SpellSlot SpellSlot { get; }
        int Slot { get; }
        ItemId Id { get; }
        int CountInSlot { get; }
        float PurchaseTime { get; }
        int SpellCharges { get; }
        float InvestedGoldAmount { get; }
        IItemData ItemData { get; }
    }
}