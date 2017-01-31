namespace App.Middleware.HMACSignatureAuth
{
    public static class HMACSignatureAuthDefaults
    {
        public const string AuthenticationScheme = "Header";
        public const string DATE_HEADER = "X-DATE";
        public const string AUTHORIZATION_HEADER = "Authorization";
        public const int DRIFT_TIME_ALLOWANCE = 90;
        public const string HKDF_ALGO = "SHA256";
        public const string AUTH_INFO = "HMAC|AuthenticationKey";
    }
}