using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zh.fang.stu.@event
{
    class EventSource
    {
        public event Action action;
        public event Action action_another;

        internal void Exec()
        {
            Console.WriteLine($"source thread --> {Thread.CurrentThread.ManagedThreadId} .");
            action?.Invoke();
            Console.WriteLine();
            action_another?.Invoke();
        }
    }

    class EventApp
    {
        List<EventHandle> EventHandlers = new List<EventHandle>();
        Dictionary<int, List<Action>> TaskCollection = new Dictionary<int, List<Action>>();
        Dictionary<int, EventInfo> EventRegisted = new Dictionary<int, EventInfo>();  

        internal void Init(EventSource eventSoure)
        {
            for (int i = 0; i < 10; i++)
            {
                var handler = new EventHandle(i);
                handler.Init(eventSoure, this);
                EventHandlers.Add(handler);
            }
        }

        internal void RegisterEvent(EventSource eventSource, string eventName, Action task)
        {
            var event_info = eventSource.GetType()
                .GetEvents(BindingFlags.Instance| BindingFlags.Public)
                .FirstOrDefault(t => string.Equals(eventName, t.Name));

            if (null != event_info)
            {
                var hash = eventName.GetHashCode();

                if (!TaskCollection.ContainsKey(hash))
                    TaskCollection.Add(hash, new List<Action>());

                TaskCollection[hash].Add(task);

                if (!EventRegisted.ContainsKey(hash))
                {
                    EventRegisted.Add(hash, event_info);
                    var method = this.GetType().GetMethod("Callback", BindingFlags.Instance | BindingFlags.NonPublic);
                    //var handler = Delegate.CreateDelegate(typeof(Action<Int32>), hash, method);
                    var handler = new Action(() => Callback(hash));
                    event_info.AddEventHandler(eventSource, handler);
                }
            }
        }

        void Callback(int hashCode)
        {
            //Parallel.Invoke(TaskCollection[hashCode].ToArray());
            TaskCollection[hashCode].AsParallel().ForAll(t => t.Invoke());
        }
    }

    class EventHandle
    {
        public int Id { get; private set; }

        public string Uuid { get; private set; } = Guid.NewGuid().ToString("N").Replace("-", "");

        internal EventHandle(int id)
        {
            Id = id;
        }

        internal void Init(EventSource eventSource, EventApp app)
        {
            app.RegisterEvent(eventSource, nameof(eventSource.action), () => CallbackCore());
            app.RegisterEvent(eventSource, nameof(eventSource.action_another), () => AnotherCallbackCore());
        }

        void AnotherCallbackCore()
        {
            var wait = (new Random(Uuid.GetHashCode())).Next(100, 1999);
            Console.WriteLine($"client: id --> {Id}\tuuid --> {Uuid}\tthread --> {Thread.CurrentThread.ManagedThreadId}\twait --> {wait}\tevent --> another_action");
            Task.Delay(wait).Wait();
            Console.WriteLine($"client: id --> {Id}\tevent --> another_action waited .");
        }

        void CallbackCore()
        {
            var wait = (new Random(Uuid.GetHashCode())).Next(100, 1999);
            Console.WriteLine($"client: id --> {Id}\tuuid --> {Uuid}\tthread --> {Thread.CurrentThread.ManagedThreadId}\twait --> {wait}\tevent --> action");
            Task.Delay(wait).Wait();
            Console.WriteLine($"client: id --> {Id}\tevent --> action waited .");
        }
    }
}
