using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Timers;

namespace ClockTest
{
    class Program
    {
        const char Period = '.';
        const int Time = 8500;
        const int Timezone = 2;
        static Timer timer = new Timer(Time);
        static ConcurrentBag<DateTime> DateTimes = new ConcurrentBag<DateTime>();
        
        static void Main(string[] args)
        {
            Console.WriteLine("--- Testing Parallelism ---");
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            timer.Enabled = true;
            while (timer.Enabled)
                Task.Run(() => DateTimes.Add(DateTime.UtcNow.AddHours(Timezone)));

            int Count = DateTimes.Count;
            Console.WriteLine($"Asyncronous: {DateTimes.Count}");

            timer.Close();
            timer.Dispose();
            timer = new Timer(Time);
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            DateTimes.Clear();
            timer.Enabled = true;
            while (timer.Enabled)
                DateTimes.Add(DateTime.UtcNow.AddHours(Timezone));

            Console.WriteLine($"Syncronous:  {DateTimes.Count}\r\n{DateTimes.Count / Count}x Faster");
            DateTimes.Clear();
            timer.Close();
            timer.Dispose();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        static void Timer_Elapsed(object sender, ElapsedEventArgs e) { timer.Enabled = false; }
    }
}