using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace App.Middleware.HMACSignatureAuth
{
    using App.Models;
    using App.DataContext;

    internal class HMACSignatureAuthHandler : AuthenticationHandler<HMACSignatureAuthOptions>
    {
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Retrieve the headers
            string authHeader = Request.Headers[Options.AUTHORIZATION_HEADER];
            string dateHeader = Request.Headers[Options.DATE_HEADER];
            try {
                if (authHeader == null) {
                    return AuthenticateResult.Fail("Authorization header is missing");
                }

                if (dateHeader == null) {
                    return AuthenticateResult.Fail("Missing date header");
                }

                // Make sure the HMAC header is valid
                Regex rgx = new Regex("^HMAC (.*),(.*),(.*)$");
                if (!rgx.Match(authHeader).Success) {
                    return AuthenticateResult.Fail("Invalid Authorization header");
                }

                // Retrieve the necessary components and conver them to the proper types
                string[] options = authHeader.Replace("HMAC ", "").Split(',');
                string accessToken = options[0];
                byte[] hmac = Convert.FromBase64String(options[1]);
                byte[] salt = Convert.FromBase64String(options[2]);

                // Retrieve the token object
                Token token = new Token(Options.cache, accessToken);

                if (token.access_token == null || token.IsExpired()) {
                    return AuthenticateResult.Fail("Invalid access token");
                }

                string body = new System.IO.StreamReader(Context.Request.Body).ReadToEnd();
                byte[] requestData = System.Text.Encoding.UTF8.GetBytes(body);
                Context.Request.Body = new System.IO.MemoryStream(requestData);

                if (!this.IsHMACSignatureValid(
                    token.access_token,
                    Convert.FromBase64String(token.ikm),
                    salt,
                    body,
                    hmac
                )) {
                    return AuthenticateResult.Fail("Invalid Signature");
                }

                var claims = new List<Claim>(2);
                claims.Add(new Claim("access_token", accessToken));
                claims.Add(new Claim("user_id", token.user_id.ToString()));

                ClaimsPrincipal principal = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        claims,
                        Options.AuthenticationScheme
                    )
                );

                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Options.AuthenticationScheme);
                return AuthenticateResult.Success(ticket);

            } catch (Exception) {
                return AuthenticateResult.Fail("An error occured when validating your request");
            }
        }

        private bool IsHMACSignatureValid(string access_token, byte[] ikm, byte[] salt, string request, byte[] hmac)
        {
            if (hmac == null) {
                return false;
            }

            try {
                var kdf = new App.HKDF(
                    HashAlgorithmName.SHA256,
                    ikm,
                    System.Text.Encoding.UTF8.GetBytes(Options.AUTH_INFO),
                    0,
                    salt
                );

                var drift = this.GetTimeDrift(Context.Request.Headers[Options.DATE_HEADER]);
                
                if (drift >= Options.DRIFT_TIME_ALLOWANCE) {
                    return false;
                }

                var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request));

                string signatureString = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower() + "\n" +
                                        Context.Request.Method + "+" + Context.Request.Path.ToString() + "\n" +
                                        Context.Request.Headers[Options.DATE_HEADER] + "\n" +
                                        Convert.ToBase64String(salt);

                var selfHMAC = new HMACSHA256();
                    selfHMAC.Key = kdf.hash;
                var selfHMACValue = selfHMAC.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signatureString));
                
                // We should do a constant time comparison of the two HMAC values
                return this.HMACCompareConstantTime(hmac, selfHMACValue);

            } catch (Exception) {
                return false;
            }

            return false;
        }

        private bool HMACCompareConstantTime(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++) {
                diff = a[i] ^ b[i];
            }

            return diff == 0;
        }

        private int GetTimeDrift(string header)
        {
            var dt = DateTime.Now;
            var t = Convert.ToDateTime(header.Replace("+0000", "GMT"));
            TimeSpan duration = dt - t;
            
            return Math.Abs(duration.Seconds);
        }

        private HashAlgorithmName DetermineHashAlgorithmName()
        {
            switch(Options.HKDF_ALGO) {
                case "SHA512":
                    return HashAlgorithmName.SHA512;
                    break;
                case "SHA384":
                    return HashAlgorithmName.SHA384;
                    break;
                case "SHA256":
                default:
                    return HashAlgorithmName.SHA256;
                
            }
        }
        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();
            Response.StatusCode = 401;

            return false;
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
        {
            throw new NotSupportedException();
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            throw new NotSupportedException();
        }
    }
}