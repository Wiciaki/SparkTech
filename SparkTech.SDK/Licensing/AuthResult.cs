namespace SparkTech.SDK.Licensing
{
    using System;

    public sealed class AuthResult
    {
        public readonly bool IsPremium;

        public AuthResult(bool isPremium)
        {
            this.IsPremium = isPremium;
        }

        public DateTime Expiry;

        public override string ToString()
        {
            return "IsPremium=" + this.IsPremium + ",Expiry=" + this.Expiry;
        }
    }
}