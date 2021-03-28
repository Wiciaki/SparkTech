namespace SparkTech.SDK.Entities
{
    public interface IItemData
    {
        int RequiredLevel { get; }
        ItemId Id { get; }
        int MaxStack { get; }
        int Price { get; }
        bool UsableInStore { get; }
        bool CanBeSold { get; }
        float SellBackPercentage { get; }
        string DisplayName { get; }
        string TranslatedDisplayName { get; }
        string SpellName { get; }
    }
}