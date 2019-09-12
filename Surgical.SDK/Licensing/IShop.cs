namespace Surgical.SDK.Licensing
{
    using System.Threading.Tasks;

    public interface IShop
    {
        Task<string> GetShopUrl();
    }
}