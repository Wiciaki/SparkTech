namespace SparkTech.SDK.Security
{
    using System;

    public static class Sandbox
    {
        static Sandbox()
        {
            Domain = AppDomain.CreateDomain("boo");
        }

        private static readonly AppDomain Domain;

        internal static void LoadThirdParty()
        {
            
        }
    }
}