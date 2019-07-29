namespace FatSquirrel
{
    using System;
    using System.Threading.Tasks;

    using SparkTech.SDK.Auth;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var lic = new Netlicensing("d1213e7b-0817-4544-aa37-01817170c494");

            //var store = await lic.GetShopLink();
            var auth = await lic.Auth("PUJD2J5XN");

            Console.WriteLine(auth);
        }
    }
}
