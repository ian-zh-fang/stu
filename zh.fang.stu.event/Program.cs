namespace zh.fang.stu.@event
{
    using System;

    static class Program
    {
        static void Main(string[] args)
        {
            sevent.OnExecute += Sevent_OnExecute;
            sevent.OnExecute += Sevent_OnExecute1;
            sevent.OnExecute += Sevent_OnExecute3;

            Console.WriteLine(sevent.invoke());

            //var source = new EventObserveProvider();
            //var app = new EventSubcripionApp();
            //app.Init(source);
            //source.Exec();

            //var event_source = new EventSource();
            //var event_app = new EventApp();
            //event_app.Init(event_source);
            //event_source.Exec();


            Console.ReadKey();
        }

        private static int Sevent_OnExecute3()
        {
            sevent.OnExecute += Sevent_OnExecute2;
            return 4;
        }

        private static int Sevent_OnExecute1()
        {
            return 2;
        }

        private static int Sevent_OnExecute()
        {
            return 1;
        }

        private static int Sevent_OnExecute2()
        {
            return 3;
        }
    }

    class sevent
    {
        private static event Func<int> _e;
        public static event Func<int> OnExecute
        {
            add { _e += value; Console.WriteLine(value.Method.ReflectedType.FullName); }
            remove { _e += value; }
        }

        public static int invoke()
        {
            return _e.Invoke();
        }
    }
}
