namespace zh.fang.stu.callbackevent
{
    using System;
    using System.Linq;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IA)) && t.IsClass && !t.IsAbstract && t.IsPublic))
                .Where(ts => 0 < ts.Count())
                .ToArray();
            

            Console.WriteLine("hello world .");
            new B();

            Console.ReadKey();
        }
    }

    public interface IA { }

    public abstract class AA:IA { }

    public class BB : AA { }

    public class SEvent
    {
        public static event Func<string> OnFire;

        public static string Fire()
        {
            if (null == OnFire) return null;

            return OnFire();
        }
    }

    public class SEvent_1 : SEvent
    {
        static SEvent_1()
        {
            
        }
    }

    abstract class A
    {
        protected A()
        {
            Console.WriteLine($"{nameof(A)} .actor");
        }
    }

    class B:A
    {
        public B()
        {
            Console.WriteLine($"{nameof(B)} .actor");
        }
    }
}
