namespace App
{
    using System;
    using System.Security.Cryptography;

    class HKDF
    {
        public byte[] hash { get; set; }
        private readonly HashAlgorithmName algorithm;
        private readonly HMAC hmac;
        private readonly int digestLength;

        public HKDF(HashAlgorithmName algo, byte[] ikm, byte[] info, int outputLength = 0, byte[] salt = null)
        {
            this.algorithm = algo;
            this.hmac = this.determineHMAC();
            this.digestLength = this.hmac.ComputeHash(ikm).Length;

            if (outputLength == 0) {
                outputLength = this.digestLength;
            }

            if (outputLength < 0 || outputLength > 255 * digestLength) {
                throw new Exception("Bad output length requested of HKDF");
            }

            this.hash = this.deriveKey(salt, ikm, info, outputLength);
        }

        private HMAC determineHMAC()
        {
            if (this.algorithm == HashAlgorithmName.MD5) {
                return new HMACMD5();
            } else if (this.algorithm == HashAlgorithmName.SHA1) {
                return new HMACSHA1();
            } else if (this.algorithm == HashAlgorithmName.SHA256) {
                return new HMACSHA256();
            } else if (this.algorithm == HashAlgorithmName.SHA384) {
                return new HMACSHA384();
            } else if (this.algorithm == HashAlgorithmName.SHA512) {
                return new HMACSHA512();
            }

            return new HMACSHA256();
        }

        private byte[] extract(byte[] salt, byte[] ikm)
        {
            return this.HMAC(salt, ikm);
        }

        private byte[] HMAC(byte[] key, byte[] message)
        {
            var hmac = this.hmac;
            hmac.Key = key;
            return hmac.ComputeHash(message);
        }

        private byte[] expand(byte[] prk, byte[] info, int outputLength)
        {
            var resultBlock = new byte[0];
            var result = new byte[outputLength];
            var bytesRemaining = outputLength;

            for (int i = 1; bytesRemaining > 0; i++)
            {
                var currentInfo = new byte[resultBlock.Length + info.Length + 1];
                Array.Copy(resultBlock, 0, currentInfo, 0, resultBlock.Length);
                Array.Copy(info, 0, currentInfo, resultBlock.Length, info.Length);
                currentInfo[currentInfo.Length - 1] = (byte)i;
                resultBlock = this.HMAC(prk, currentInfo);
                Array.Copy(resultBlock, 0, result, outputLength - bytesRemaining, Math.Min(resultBlock.Length, bytesRemaining));
                bytesRemaining -= resultBlock.Length;
            }

            return result;
        }

        private byte[] deriveKey(byte[] salt, byte[] ikm, byte[] info, int outputLength)
        {
            if (info == null) {
                info = new byte[0];
            }

            if (salt == null) {
                salt = new byte[this.digestLength];
            }

            var prk = extract(salt, ikm);

            if (prk.Length < digestLength) {
                throw new Exception("Psuedo-random key is larger then digest length. Cannot perform operation");
            }

            var result = expand(prk, info, outputLength);
            return result;
        }
    }
}