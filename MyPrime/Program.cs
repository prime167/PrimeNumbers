using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using LibPrime;
using LinqToDB;

// http://www.hanselman.com/blog/BackToParallelBasicsDoYouReallyWantToDoThatOrWhyDoesntTheNewParallelForSupportBigInteger.aspx
namespace MyPrime
{
    class Program
    {
        static void Main(string[] args)
        {
            BigInteger maxFound;
            Int64 foundCount = 0;
            using (var pdb = new PrimeDB())
            {
                if (!pdb.Primes.Any())
                {
                    maxFound = 2;
                }
                else
                {
                    var maxId = pdb.Primes.Max(p => p.Id);
                    var v = pdb.Primes.Single(p => p.Id == maxId);
                    maxFound = BigInteger.Parse(v.PrimeNumber) + 1;
                    foundCount = v.Id;
                }
            }

            var max = BigInteger.Parse(new string('9', 9999));
            var sw = new Stopwatch();
            var a = new ConcurrentBag<BigInteger>();

            const int gap = 1000000;

            sw.Start();
            for (var i = maxFound; i < max; i += gap)
            {
                var sortedList = new List<BigInteger>();
                var last = maxFound;
                ParallelFor(i, i + gap, j =>
                {
                    if (j.IsProbablePrime())
                    {
                        a.Add(j);
                    }
                });

                sortedList.AddRange(a);
                sortedList.Sort();
                Prime p;
                using (var pdb = new PrimeDB())
                {
                    pdb.BeginTransaction();
                    foreach (var bigInteger in sortedList)
                    {
                        p = new Prime
                        {
                            Id = ++foundCount,
                            PrimeNumber = bigInteger.ToString()
                        };

                        if (p.Id % 10000 == 0)
                        {
                            pdb.Insert(p);
                        }

                        if (bigInteger - last > gap)
                        {
                            sw.Stop();
                            Console.WriteLine("{0} -第{1}个- -{2}位- [{3}]", sw.Elapsed, p.Id, p.PrimeNumber.Length,
                                p.PrimeNumber);
                            sw.Reset();
                            sw.Start();
                            last = bigInteger;
                        }
                    }

                    pdb.CommitTransaction();
                }

                sortedList.Clear();
                while (!a.IsEmpty)
                {
                    BigInteger someItem;
                    a.TryTake(out someItem);
                }

                Thread.Sleep(500);
            }

            Console.WriteLine(sw.Elapsed);
            Console.WriteLine(a.Count);
            Console.ReadLine();
        }

        private static IEnumerable<BigInteger> Range(BigInteger fromInclusive, BigInteger toExclusive)
        {
            for (BigInteger i = fromInclusive; i < toExclusive; i++) yield return i;
        }

        public static void ParallelFor(BigInteger fromInclusive, BigInteger toExclusive, Action<BigInteger> body)
        {
            Parallel.ForEach(Range(fromInclusive, toExclusive), new ParallelOptions { MaxDegreeOfParallelism = -1 }, body);
        }
    }
}
