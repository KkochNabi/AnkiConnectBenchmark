using AnkiConnectAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiConnectBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine(Benchmark.SendBenchmark(20)); //1000s is where it starts to get laggy; Anki probably doesn't like importing 2k cards (lag)
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();

        }
    }
}
