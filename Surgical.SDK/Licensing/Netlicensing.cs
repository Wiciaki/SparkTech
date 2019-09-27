namespace Surgical.SDK.Licensing
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

    using Surgical.SDK.Logging;

    public class Netlicensing : IAuth, IShop
    {
        private readonly string licenseeNumber;

        private readonly NetworkCredential crendentials;

        private Netlicensing(string licenseeNumber, NetworkCredential credentials)
        {
            this.licenseeNumber = WebUtility.UrlEncode(licenseeNumber);

            this.crendentials = credentials;
        }

        public Netlicensing(string licenseeNumber, SecureString apiKey) : this(licenseeNumber, new NetworkCredential("apiKey", apiKey))
        {

        }

        public Netlicensing(string licenseeNumber, string apiKey) : this(licenseeNumber, new NetworkCredential("apiKey", apiKey))
        {

        }

        public async Task<string> GetShopUrl()
        {
            var licensee = "licenseeNumber=" + this.licenseeNumber;

            var json = await this.SendPost("token", "tokenType=SHOP", licensee);

            return json == null ? null : GetResponseObjects(json)["shopURL"];
        }

        public async Task<AuthResult> GetAuth(string productNumber)
        {
            var ending = $"licensee/{this.licenseeNumber}/validate";
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
                            var expiry = DateTime.Parse(r["expires"]);

                            if (expiry > DateTime.Now)
                            {
                                return new AuthResult(true) { Expiry = expiry };
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

            request.UserAgent = "Surgical.SDK: .NET " + Environment.Version;
            request.Method = "POST";
            request.Credentials = this.crendentials;
            request.PreAuthenticate = true;
            request.Accept = "application/json";
            request.SendChunked = false;
            request.ContentType = "application/x-www-form-urlencoded";

            var bytes = Encoding.UTF8.GetBytes(string.Join("&", reqParams));

            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(bytes, 0, bytes.Length);
            }

            try
            {
                using var response = (HttpWebResponse)await request.GetResponseAsync();
                using var responseStream = response.GetResponseStream();

                if (responseStream == null)
                {
                    throw new ApplicationException("responseStream == null");
                }

                Log.Info("Auth OK");

                using var sr = new StreamReader(responseStream);
                using var textReader = new JsonTextReader(sr);

                return JObject.Load(textReader);
            }
            catch (WebException ex)
            {
                Log.Info("Auth failed!");

                var response = (HttpWebResponse)ex.Response;

                if (response != null)
                {
                    var statusCode = (int)response.StatusCode;

                    Log.Info("Response code " + statusCode);
                }

                return null;
            }
        }
    }
}