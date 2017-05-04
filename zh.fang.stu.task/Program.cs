using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zh.fang.stu.task
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = 0;
            Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                Console.WriteLine($"task thread id --> {System.Threading.Thread.CurrentThread.ManagedThreadId} -- > {t}");
            }).ConfigureAwait(false);
            Console.WriteLine($"main thread id --> {System.Threading.Thread.CurrentThread.ManagedThreadId} --> {t}");
            Console.ReadKey();

            
        }
    }
}
