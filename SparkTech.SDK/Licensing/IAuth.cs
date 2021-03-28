namespace SparkTech.SDK.Licensing
{
    using System.Threading.Tasks;

    public interface IAuth
    {
        Task<AuthResult> GetAuth(string product);
    }
}