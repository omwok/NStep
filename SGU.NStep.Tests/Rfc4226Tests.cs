using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGU.NStep.Core;


namespace SGU.NStep.Tests
{
    [NUnit.Framework.TestFixture(Category = "RFC4226")]
    public class Rfc4226Tests
    {
        private const string seed = "12345678901234567890";
        private int[] outputs = new int[] { 755224, 287082, 359152, 969429, 338314, 254676, 287922, 162583, 399871, 520489 };

        [NUnit.Framework.Test(Description = "HOTP, HMACSHA1")]
        public void TestHOTP1()
        {
            for (int ul = 0; ul < outputs.Length; ul++)            
                NUnit.Framework.Assert.AreEqual(outputs[ul], HashAlgorithm.SHA1.HOTP(System.Text.Encoding.ASCII.GetBytes(seed), Convert.ToUInt64(ul)));
        }
    }
}
