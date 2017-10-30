using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace zh.fang.stu.cancellation
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileds = typeof(A).GetFields(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < fileds.Length; i++)
            {
                var field = fileds[i];
                var obj = field.GetValue(null);
            }

            var cancel = new System.Threading.CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                Task.Delay(10000).Wait();
                Console.WriteLine("thread is stoped .");
            }, cancel.Token);

            const int delays = 5000;
            Console.WriteLine("will be cancel after {0} milliseconds ...", delays);
            Task.Delay(delays).Wait();
            cancel.Cancel(true);
            Console.WriteLine("thread canceled .");

            Console.ReadKey();
        }
    }

    class A
    {
        

        public static readonly A a = new A();

        public static readonly A aa = new A();

        public static readonly A an;
    }
}
