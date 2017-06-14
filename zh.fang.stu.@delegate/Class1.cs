using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zh.fang.stu._delegate
{
   
    class Program
    {
        static void Main(params string[] args)
        {
            

            Console.ReadKey();
        }
    }

    class A
    {
        public void DoSomething()
        {
            Console.WriteLine("do something ...");
        }
    }

    interface IA<TContext>
    {
        void Do(TContext context);
    }
    
    interface IB
    {
        void Do(A context);
    }

    class AA<TContext> : IA<TContext>
    {
        public void Do(TContext context)
        {
            throw new NotImplementedException();
        }
    }

    class BB:AA<A>, IB
    {

    }
}
