namespace SparkTech.SDK.Licensing
{
    using System;
    using System.Globalization;

    public class AuthResult
    {
        public bool IsLicensed { get; }

        public DateTime Expiry { get; }

        public AuthResult(bool licensed, DateTime expiry = default(DateTime))
        {
            this.IsLicensed = licensed;
            this.Expiry = expiry;
        }

        public override string ToString()
        {
            var expiry = this.IsLifetime() ? "Never" : this.Expiry.ToString(CultureInfo.InvariantCulture);

            return $"IsLicensed={this.IsLicensed},Expiry={expiry}";
        }

        public bool IsLifetime()
        {
            return this.Expiry == default(DateTime);
        }
    }
}