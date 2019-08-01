namespace SparkTech.SDK.Auth
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Security;

    public class Netlicensing : IAuth
    {
        protected virtual string LicenseeNumber => WebUtility.UrlEncode(Machine.HardwareBasedUserId);

        private readonly NetworkCredential crendentials;

        static Netlicensing()
        {
            ServicePointManager.Expect100Continue = false;
        }

        public Netlicensing(SecureString apiKey)
        {
            this.crendentials = new NetworkCredential("apiKey", apiKey);
        }

        public Netlicensing(string apiKey)
        {
            this.crendentials = new NetworkCredential("apiKey", apiKey);
        }

        public async Task<string> GetShopUrl()
        {
            var licenseeNumber = "licenseeNumber=" + this.LicenseeNumber;

            var json = await this.SendPost("token", "tokenType=SHOP", licenseeNumber);

            return json == null ? null : GetResponseObjects(json)["shopURL"];
        }

        public async Task<AuthResult> Auth(string productNumber)
        {
            var ending = $"licensee/{this.LicenseeNumber}/validate";
            var param = "productNumber=" + productNumber;

            return GetAuth(await this.SendPost(ending, param));
        }

        private static Dictionary<string, string> GetResponseObjects(JObject json)
        {
            // this might fail under certain configurations so I delegated it into this method to take pain out of debugging

            // firstly, this
            var items = json["items"]["item"];

            // then, there is a 1-item array
            var props = items.Single()["property"];

            // and finally, everything in "property" is a JObject
            var jObjects = props.Cast<JObject>();

            // which are basically name/value pairs
            return jObjects.ToDictionary(j => j["name"].Value<string>(), j => j["value"].Value<string>());
        }

        private static AuthResult GetAuth(JObject json)
        {
            if (json != null)
            {
                // inspect to add more models
                // Console.WriteLine(json);

                var r = GetResponseObjects(json);

                if (bool.Parse(r["valid"]))
                {
                    switch (r["licensingModel"])
                    {
                        case "Subscription":
                            var exp = DateTime.Parse(r["expires"]);

                            if (exp > DateTime.Now)
                            {
                                return new AuthResult(true) { Expiry = exp };
                            }

                            break;
                    }
                }
            }

            return new AuthResult(false);
        }

        private async Task<JObject> SendPost(string ending, params string[] reqParams)
        {
            const string BaseLink = "https://go.netlicensing.io/core/v2/rest/";

            var request = (HttpWebRequest)WebRequest.Create(BaseLink + ending);

            request.UserAgent = "SparkTech.SDK: .NET " + Environment.Version;
            request.Method = "POST";
            request.Credentials = this.crendentials;
            request.PreAuthenticate = true;
            request.Accept = "application/json";
            request.SendChunked = false;
            request.ContentType = "application/x-www-form-urlencoded";

            var bytes = Encoding.UTF8.GetBytes(string.Join("&", reqParams));

            request.ContentLength = bytes.Length;

            await using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();

                if (responseStream == null)
                {
                    Log.Warn("responseStream == null");
                    return null;
                }

                Log.Info("Successfully exchanged data with the license server.");

                using var sr = new StreamReader(responseStream);
                using var textReader = new JsonTextReader(sr);

                return JObject.Load(textReader);
            }
            catch (WebException ex)
            {
                Log.Info("Auth failed");
                Log.Debug(ex);

                return null;
            }
        }
    }
}