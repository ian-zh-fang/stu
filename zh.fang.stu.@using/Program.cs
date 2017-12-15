using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zh.fang.stu._using
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var first = new FirstDisable())
            using (var second = new SecondDisable())
            using (var third = new ThirdDisable())
            {
                Console.WriteLine("invoked --> {0}", typeof(Program).GetMethod(nameof(Main), System.Reflection.BindingFlags.Static| System.Reflection.BindingFlags.NonPublic).Name);
            }

            Console.ReadKey();
        }
    }

    abstract class BaseDisable : IDisposable
    {
        void IDisposable.Dispose()
        {
            Console.WriteLine("disposed --> {0}", GetType().FullName);
        }
    }

    class FirstDisable : BaseDisable
    {

    }

    class SecondDisable : BaseDisable
    {

    }

    class ThirdDisable : BaseDisable
    {

    }
}
