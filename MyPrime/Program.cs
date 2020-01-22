using LibPrime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

// http://www.hanselman.com/blog/BackToParallelBasicsDoYouReallyWantToDoThatOrWhyDoesntTheNewParallelForSupportBigInteger.aspx
namespace MyPrime
{
    class Program
    {
        static void Main(string[] args)
        {
            BigInteger maxFound;
            long foundCount = 0;
            var all = PrimeRepo.GetAll().ToList();

            if (!all.Any())
            {
                maxFound = 2;
            }
            else
            {
                var maxId = all.Max(p => p.Id);
                var v = all.Single(p => p.Id == maxId);
                maxFound = BigInteger.Parse(v.PrimeNumber) + 1;
                foundCount = v.Seq;
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

                foreach (var bigInteger in sortedList)
                {
                    var p = new Prime
                    {
                        Id = ++foundCount,
                        Seq = foundCount,
                        PrimeNumber = bigInteger.ToString()
                    };

                    if (p.Seq % 10000 == 0)
                    {
                        PrimeRepo.Insert(p);
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

                sortedList.Clear();
                while (!a.IsEmpty)
                {
                    a.TryTake(out _);
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
