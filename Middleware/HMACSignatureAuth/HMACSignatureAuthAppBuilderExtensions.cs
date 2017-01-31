using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace App.Middleware.HMACSignatureAuth
{
    public static class MACSignatureAuthAppBuilderExtensions
    {
        public static IApplicationBuilder UseHMACSignatureAuthentication(this IApplicationBuilder app)
        {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<HMACSignatureAuthMiddleware>();
        }

        public static IApplicationBuilder UseHMACSignatureAuthentication(this IApplicationBuilder app, HMACSignatureAuthOptions options)
        {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<HMACSignatureAuthMiddleware>(Options.Create(options));
        }
    }
}