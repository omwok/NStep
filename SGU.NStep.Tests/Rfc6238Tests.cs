using System;
using SGU.NStep.Core;

namespace SGU.NStep.Tests
{
    [NUnit.Framework.TestFixture(Category="RFC6238")]
    public class Rfc6238Tests
    {
        private static DateTime[] inputTimes = new DateTime[] { 
            new DateTime(1970,01,01,00,00,59, DateTimeKind.Utc),
            new DateTime(2005,03,18,01,58,29, DateTimeKind.Utc),
            new DateTime(2005,03,18,01,58,31, DateTimeKind.Utc),
            new DateTime(2009,02,13,23,31,30, DateTimeKind.Utc),
            new DateTime(2033,05,18,03,33,20, DateTimeKind.Utc),
            new DateTime(2603,10,11,11,33,20, DateTimeKind.Utc)
        };

        private const string seed = "12345678901234567890";
        private const string seed32 = "12345678901234567890123456789012";
        private const string seed64 = "1234567890123456789012345678901234567890123456789012345678901234";

        private static int[] output = new int[] { 94287082, 07081804, 14050471, 89005924, 69279037, 65353130 };
        private static int[] output32 = new int[] { 46119246, 68084774, 67062674, 91819424, 90698825, 77737706 };
        private static int[] output64 = new int[] { 90693936, 25091201, 99943326, 93441116, 38618901, 47863826 };

        private static void TestTOTP(SGU.NStep.Core.HashAlgorithm algorithm, string key, int[] expextedOutputs)
        {
            for (int i = 0; i < inputTimes.Length; i++)
            {
                var result = algorithm.TOTP(System.Text.Encoding.ASCII.GetBytes(key), inputTimes[i], 8);
                NUnit.Framework.Assert.AreEqual(expextedOutputs[i], result);
            }
        }

        [NUnit.Framework.Test(Description="TOTP, HMACSHA1")]
        public void TestTOTP1()
        {
            TestTOTP(HashAlgorithm.SHA1, seed, output);
        }

        [NUnit.Framework.Test(Description = "TOTP, HMACSHA256")]
        public void TestTOTP2()
        {
            TestTOTP(HashAlgorithm.SHA256, seed32, output32);
        }

        [NUnit.Framework.Test(Description = "TOTP, HMACSHA512")]
        public void TestTOTP3()
        {
            TestTOTP(HashAlgorithm.SHA512, seed64, output64);
        }

    }
}
