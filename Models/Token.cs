using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace App.Models
{
    public class Token
    {
        private readonly IDistributedCache _cache;

        public string ikm { get; set; }
        public string salt { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public long expires_at { get; set; }

        [JsonConstructor]
        private Token() {}
        
        public Token(IDistributedCache cache)
        {
            this._cache = cache;
            this.Generate();
        }

        public Token(IDistributedCache cache, string access_token)
        {
            this._cache = cache;

            try {
                string json = this._cache.GetString("access_token:" + access_token);
                Token token = JsonConvert.DeserializeObject<Token>(json);
                this.ikm = token.ikm;
                this.salt = token.salt;
                this.access_token = token.access_token;
                this.refresh_token = token.refresh_token;
                this.expires_at = token.expires_at;
                
            } catch (Exception e) {}
        }

        private void Generate()
        {
            var rng = new Random();
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");

            var ikm = new byte[32];
            var salt = new byte[32];
            var access_token = new byte[64];
            var refresh_token = new byte[64];
            rng.NextBytes(ikm);
            rng.NextBytes(salt);
            rng.NextBytes(access_token);
            rng.NextBytes(refresh_token);

            this.ikm = Convert.ToBase64String(ikm);
            this.salt = Convert.ToBase64String(salt);
            this.access_token = rgx.Replace(Convert.ToBase64String(access_token), "");
            this.refresh_token = rgx.Replace(Convert.ToBase64String(refresh_token), "");

            // Set a 15 minute timeout
            this.expires_at = DateTimeOffset.Now.ToUnixTimeSeconds() + 900;
            
            // Cache the data into redis
            this._cache.SetString("access_token:" + this.access_token, JsonConvert.SerializeObject(this));   
        }

        public bool IsExpired()
        {
            return this.expires_at < DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}