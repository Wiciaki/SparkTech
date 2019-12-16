namespace Surgical.SDK.Licensing
{
    using System;

    public class AuthResult
    {
        public bool IsLicensed { get; }

        public DateTime Expiry { get; }

        public AuthResult(bool licensed, DateTime expiry = default)
        {
            this.IsLicensed = licensed;
            this.Expiry = expiry;
        }

        public override string ToString()
        {
            return "IsLicensed=" + this.IsLicensed + ",Expiry=" + this.Expiry;
        }

        public static AuthResult GetLifetime()
        {
            return new AuthResult(true);
        }
    }
}