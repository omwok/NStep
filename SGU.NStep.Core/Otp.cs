using System;
using System.Security.Cryptography;

namespace SGU.NStep.Core
{
    public enum HashAlgorithm
    {
        SHA1,
        SHA256,
        SHA512
    }

    public static class Otp
    {
        /// <summary>
        /// output digit lookup table
        /// </summary>
        private static int[] DIGITS_POWER = new int[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };

        public static int HOTP(this HashAlgorithm hash, byte[] key, ulong counter, ushort outputDigits = 6)
        {
            switch (hash)
            {
                case HashAlgorithm.SHA1:
                    return key.HOTP<HMACSHA1>(counter, outputDigits);
                case HashAlgorithm.SHA256:
                    return key.HOTP<HMACSHA256>(counter, outputDigits);
                default:
                    return key.HOTP<HMACSHA512>(counter, outputDigits);
            }
        }

        private static int HOTP<THash>(this byte[] key, ulong counter, ushort outputDigits = 6)
            where THash : HMAC, new()
        {
            if (key == null || key.Length == 0)
                return 0;
            using (var gen = new THash())
            {
                gen.Key = key;
                //generator.Initialize();
                var countBytes = CounterToBytes(counter);
                var hmac = gen.ComputeHash(countBytes);
                var offset = hmac[hmac.Length - 1] & 0xF;
                var code = (hmac[offset + 0] & 0x7F) << 24 | (hmac[offset + 1] & 0xFF) << 16 | (hmac[offset + 2] & 0xFF) << 8 | (hmac[offset + 3] & 0xFF);
                return code % DIGITS_POWER[outputDigits];
            }
        }

        private static byte[] CounterToBytes(ulong ul)
        {
            var bResult = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            short i = 0;
            while (ul != 0)
            {
                bResult[7 - i] = (byte)(ul & 0xFF);
                ul >>= 8;
                i++;
            }
            return bResult;
        }

        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int TOTP(this HashAlgorithm hash, byte[] key, DateTime utcTime, ushort outputDigits = 6, ushort x = 30)
        {
            if (utcTime < epoch)
                return 0;
            var delta = utcTime.Subtract(epoch);
            var time = Convert.ToUInt64(delta.TotalSeconds) / x;
            return hash.HOTP(key, time, outputDigits);
        }
    }
}
