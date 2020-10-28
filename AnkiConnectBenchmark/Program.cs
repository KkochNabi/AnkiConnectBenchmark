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
            // var fields = new List<string>() {"Front", "Back"};
            // var content = new List<string>(){"front content", "back content"};
            // var tags = new List<string>(){"test", "test2"};
            //
            // var response = SendToAnki.AddNote("test", "Basic", fields, content, tags);
            // Console.WriteLine(response);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Benchmark.SendBenchmark(10);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();

        }
    }
}
