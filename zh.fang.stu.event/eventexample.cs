using System;
using System.Threading;
using System.Threading.Tasks;

namespace zh.fang.stu.@event
{
    class EventProvider
    {
        internal event Action OnChanged;

        internal void Exec()
        {
            Console.WriteLine($"provider thread --> {Thread.CurrentThread.ManagedThreadId} .");
            OnChanged?.Invoke();

            Task.Delay(10000).Wait();
        }
    }

    class EventHandler
    {
        public int Id { get; private set; }

        public Guid Uuid { get; private set; } = Guid.NewGuid();

        internal EventHandler(int id)
        {
            Id = id;
        }

        internal void Init(EventProvider provider)
        {
            provider.OnChanged += () => Callback(provider);
        }

        void Callback(EventProvider provider)
        {
            Task.Factory.StartNew(() => CallbackCore(provider));
            //Parallel.Invoke(() => CallbackCore(provider));
        }

        void CallbackCore(EventProvider provider)
        {
            var wait = (new Random(Uuid.GetHashCode())).Next(100, 1999);
            Console.WriteLine($"handler --> {Id}\twait --> {wait}\tthread --> {Thread.CurrentThread.ManagedThreadId}");
            //Task.Delay(wait).Wait();
        }
    }
}
