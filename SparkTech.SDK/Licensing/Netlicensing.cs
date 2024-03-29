﻿namespace SparkTech.SDK.Licensing
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
        { }

        public Netlicensing(string licenseeNumber, string apiKey) : this(licenseeNumber, new NetworkCredential("apiKey", apiKey))
        { }

        public async Task<string> GetShopUrl()
        {
            const string Shop = "tokenType=SHOP";
            var licensee = $"licenseeNumber={this.licenseeNumber}";
            var json = await this.SendPost("token", Shop, licensee);

            return json == null ? null : GetResponseObjects(json)["shopURL"];
        }

        public async Task<AuthResult> GetAuth(string productNumber)
        {
            var ending = $"licensee/{this.licenseeNumber}/validate";
            var param = "productNumber=" + productNumber;

            return GetAuth(await this.SendPost(ending, param)) ?? new AuthResult(false);
        }

        private static Dictionary<string, string> GetResponseObjects(JObject json)
        {
            // firstly, this
            var items = json["items"]["item"];

            // then, there is a 1-item array
            var props = items.Single()["property"];

            // and finally, everything in "property" is a JObject
            // var jObjects = props.Cast<JObject>();

            // which are basically name/value pairs
            return props.ToDictionary(j => j["name"].Value<string>(), j => j["value"].Value<string>());
        }

        private static AuthResult GetAuth(JObject json)
        {
            if (json == null)
            {
                return null;
            }

            var resp = GetResponseObjects(json);

            if (!bool.Parse(resp["valid"]))
            {
                return null;
            }

            // inspect to add more models
            // Console.WriteLine(json);

            switch (resp["licensingModel"])
            {
                case "Subscription":
                    var expiry = DateTime.Parse(resp["expires"]);

                    if (expiry > DateTime.Now)
                    {
                        return new AuthResult(true, expiry);
                    }

                    break;
            }

            return null;
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

            using (var requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(bytes, 0, bytes.Length);
            }

            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        Log.Info("Netlicensing post OK");

                        using (var sr = new StreamReader(responseStream))
                        {
                            using (var textReader = new JsonTextReader(sr))
                            {
                                return JObject.Load(textReader);
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Log.Info("Netlicensing request failed!");

                var response = (HttpWebResponse)ex.Response;

                if (response != null)
                {
                    Log.Info("Response code: " + (int)response.StatusCode);
                }

                return null;
            }
        }
    }
}