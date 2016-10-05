using System;
using System.Threading;
using static System.Console;

namespace NAsyncThreads
{
    class Program
    {
        static AsyncResetEvent asyncResetEvent = new AsyncResetEvent(4);
        private const int sleepTime = 2000;
        private const int timeout = 1000;

        static void Main(string[] args)
        {
            var random = new Random();

            for (int i = 0; i < 200; i++)
                asyncResetEvent.WaitAction(() =>
                {
                    WriteLine($"Complete");
                    Thread.Sleep(random.Next(0, sleepTime));
                }, timeout);

            WriteLine("End");
            Read();
        }
    }   
}