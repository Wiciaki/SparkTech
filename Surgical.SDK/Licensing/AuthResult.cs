namespace Surgical.SDK.Licensing
{
    using System;
    using System.Globalization;

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
            var expiry = this.IsLifetime() ? "Lifetime" : this.Expiry.ToString(CultureInfo.InvariantCulture);

            return $"IsLicensed={this.IsLicensed},Expiry={expiry}";
        }

        public bool IsLifetime()
        {
            return this.Expiry == default;
        }

        public static AuthResult GetLifetime()
        {
            return new AuthResult(true);
        }
    }
}