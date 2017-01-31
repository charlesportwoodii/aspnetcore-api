using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;

namespace App.Middleware.HMACSignatureAuth
{
    public class HMACSignatureAuthOptions : AuthenticationOptions
    {
        /// <summary>
        /// Creates an instance of bearer authentication options with default values.
        /// </summary>
        public HMACSignatureAuthOptions() : base()
        {
            AuthenticationScheme = HMACSignatureAuthDefaults.AuthenticationScheme;
            AutomaticAuthenticate = true;
            AutomaticChallenge = true;
        }

        public string DATE_HEADER { get; set; } = HMACSignatureAuthDefaults.DATE_HEADER;
        public string AUTHORIZATION_HEADER { get; set; } = HMACSignatureAuthDefaults.AUTHORIZATION_HEADER;
        public int DRIFT_TIME_ALLOWANCE { get; set; } = HMACSignatureAuthDefaults.DRIFT_TIME_ALLOWANCE;
        public string HKDF_ALGO { get; set; } = HMACSignatureAuthDefaults.HKDF_ALGO;
        public string AUTH_INFO { get; set; } = HMACSignatureAuthDefaults.AUTH_INFO;

        public IDistributedCache cache { get; set; }
    }
}