using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

namespace LibPrime
{
    // http://rosettacode.org/wiki/Miller-Rabin_primality_test#C.23
    // http://www.matrix67.com/blog/archives/234
    // http://snipd.net/primality-testing-with-fermats-little-theorem-and-miller-rabin-in-c
    // http://blog.softwx.net/2013/05/miller-rabin-primality-test-in-c.html

    // Miller-Rabin primality test as an extension method on the BigInteger type.
    // Based on the Ruby implementation on this page.
    public static class BigIntegerExtensions
    {
        public static bool IsProbablePrime(this BigInteger source, int certainty = 20, bool alwaysUseRandom = false)
        {
            // https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test#Deterministic_variants_of_the_test
            // if n < 1,373,653, it is enough to test a = 2 and 3;
            // if n < 9,080,191, it is enough to test a = 31 and 73;
            // if n < 4,759,123,141, it is enough to test a = 2, 7, and 61;
            // if n < 1,122,004,669,633, it is enough to test a = 2, 13, 23, and 1662803;
            // if n < 2,152,302,898,747, it is enough to test a = 2, 3, 5, 7, and 11;
            // if n < 3,474,749,660,383, it is enough to test a = 2, 3, 5, 7, 11, and 13;
            // if n < 341,550,071,728,321, it is enough to test a = 2, 3, 5, 7, 11, 13, and 17.

            // https://miller-rabin.appspot.com/
            // if n < 2^64, it is enough to test a = 2, 325, 9375, 28178, 450775, 9780504, and 1795265022

            var toTest = new List<Int64>();
            var useBigRandom = false;
            int loopCount;
            if (source < 1373653)
            {
                toTest = new List<Int64> { 2, 3 };
            }
            else if (source < 9080191)
            {
                toTest = new List<Int64> { 31, 73 };
            }
            else if (source < 4759123141)
            {
                toTest = new List<Int64> { 2, 7, 61 };
            }
            else if (source < 1122004669633)
            {
                toTest = new List<Int64> { 2, 3, 23, 1662803 };
            }
            else if (source < 2152302898747)
            {
                toTest = new List<Int64> { 2, 3, 5, 7, 11 };
            }
            else if (source < 3474749660383)
            {
                toTest = new List<Int64> { 2, 3, 5, 7, 11, 13 };
            }
            else if (source < 341550071728321)
            {
                toTest = new List<Int64> { 2, 3, 5, 7, 11, 13, 17 };
            }
            else if (source < 585226005592931977)
            {
                toTest = new List<Int64> { 2, 123635709730000, 9233062284813009, 43835965440333360, 761179012939631437, 1263739024124850375 };
            }
            else if (source < 18446744073709551615) // 2^64 - 1
            {
                toTest = new List<Int64> { 2, 325, 9375, 28178, 450775, 9780504, 1795265022};
            }
            else
            {
                useBigRandom = true;
            }

            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            // There is no built-in method for generating random BigInteger values.
            // Instead, random BigIntegers are constructed from randomly generated
            // byte arrays of the same length as the source.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;
            if (!useBigRandom)
            {
                loopCount = toTest.Count;
            }
            else
            {
                loopCount = certainty;
            }

            if (alwaysUseRandom)
            {
                loopCount = certainty;
                useBigRandom = true;
            }

            for (int i = 0; i < loopCount; i++)
            {
                if (useBigRandom)
                {
                    do
                    {
                        // This may raise an exception in Mono 2.10.8 and earlier.
                        // http://bugzilla.xamarin.com/show_bug.cgi?id=2761
                        rng.GetBytes(bytes);
                        a = new BigInteger(bytes);
                    }
                    while (a < 2 || a >= source - 2);
                }
                else
                {
                    a = toTest[i];
                }

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (var r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }
    }
}