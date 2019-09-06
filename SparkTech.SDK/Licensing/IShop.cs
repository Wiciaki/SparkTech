namespace SparkTech.SDK.Licensing
{
    using System.Threading.Tasks;

    public interface IShop
    {
        Task<string> GetShopUrl();
    }
}