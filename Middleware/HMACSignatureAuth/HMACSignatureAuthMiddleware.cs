using System;
using System.Net.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
namespace App.Middleware.HMACSignatureAuth
{
    public class HMACSignatureAuthMiddleware : AuthenticationMiddleware<HMACSignatureAuthOptions>
    {
        public HMACSignatureAuthMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IOptions<HMACSignatureAuthOptions> options)
            : base(next, options, loggerFactory, encoder)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
        }
        
        protected override AuthenticationHandler<HMACSignatureAuthOptions> CreateHandler()
        {
            return new HMACSignatureAuthHandler();
        }
    }
}