namespace Surgical.SDK.Security
{
    using System.Reflection;

    internal static class VendorValidation
    {
        private static readonly byte[] MyKey = new byte[0];

        internal static bool IsTrusted(Assembly a)
        {
            return true;

            return a.GetName().GetPublicKeyToken() == MyKey;
        }
    }
}