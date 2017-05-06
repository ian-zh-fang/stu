using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zh.fang.stu.@event
{
    internal delegate void EventObserveHandler(object eventArg);

    internal class EventObserveProvider
    {
        public event EventObserveHandler OnEvent1;

        public event EventObserveHandler OnEvent2;

        internal void Exec()
        {
            Console.WriteLine($"main --> {Thread.CurrentThread.ManagedThreadId}");

            Parallel.Invoke(
                () => OnEvent1?.Invoke(this),
                () => OnEvent2?.Invoke(this)
            );
        }
    }

    internal class EventSubcripionApp
    {
        List<EventSubcripionHandle> EventHandlers = new List<EventSubcripionHandle>();
        readonly Dictionary<string, List<Action<object>>> TaskCollection = new Dictionary<string, List<Action<object>>>();
        readonly Dictionary<string, EventInfo> EventRegisted = new Dictionary<string, EventInfo>();

        internal void Init(EventObserveProvider context)
        {
            for (int i = 0; i < 10; i++)
            {
                var handler = new EventSubcripionHandle(i);
                handler.Init(context, this);
                EventHandlers.Add(handler);
            }
        }

        internal void RegisterSubcriptionEvent(EventObserveProvider eventSource, string eventName, Action<object> eventHandler)
        {
            var eventInfo = eventSource.GetType()
                .GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(e => string.Equals(eventName, e.Name));

            if (null == eventInfo)
                throw new ArgumentNullException($"can not found {eventName} event from {eventSource}");

            var eventType = typeof(EventObserveHandler);
            if (!string.Equals(eventInfo.EventHandlerType.FullName, eventType.FullName) && !eventInfo.EventHandlerType.IsSubclassOf(eventType))
                throw new NotSupportedException("event type not supported .");

            if (!TaskCollection.ContainsKey(eventName))
                TaskCollection.Add(eventName, new List<Action<object>>());

            TaskCollection[eventName].Add(eventHandler);

            if (!EventRegisted.ContainsKey(eventName))
            {
                EventRegisted.Add(eventName, eventInfo);
                var handler = new EventObserveHandler(
                    data => Callback(new EventCallbackContext { EventCallArg = data, EventName = eventName }));
                eventInfo.AddEventHandler(eventSource, handler);
            }
        }

        void Callback(EventCallbackContext context)
        {
            TaskCollection[context.EventName].AsParallel().ForAll(t => t.Invoke(context.EventCallArg));
        }


        private struct EventCallbackContext
        {
            public string EventName { get; set; }

            public object EventCallArg { get; set; }
        }
    }

    internal class EventSubcripionHandle
    {
        public int  Id { get; private set; }

        public string Uuid { get; private set; } = Guid.NewGuid().ToString("N").Replace("-", "");

        public EventSubcripionHandle(int id)
        {
            Id = id;
        }

        internal void Init(EventObserveProvider eventSource, EventSubcripionApp app)
        {
            app.RegisterSubcriptionEvent(eventSource, nameof(eventSource.OnEvent1), data => Event1CallbackCore(data));
            app.RegisterSubcriptionEvent(eventSource, nameof(eventSource.OnEvent2), data => Event2CallbaclCore(data));
        }

        void Event1CallbackCore(object arg)
        {
            var wait = (new Random(Uuid.GetHashCode())).Next(100, 1999);
            Console.WriteLine($"client: id --> {Id}\tuuid --> {Uuid}\tthread --> {Thread.CurrentThread.ManagedThreadId}\twait --> {wait}\tevent --> 1");
            Task.Delay(wait).Wait();
            Console.WriteLine($"client: id --> {Id}\tevent --> 1 waited .");
        }

        void Event2CallbaclCore(object arg)
        {
            var wait = (new Random(Uuid.GetHashCode())).Next(100, 1999);
            Console.WriteLine($"client: id --> {Id}\tuuid --> {Uuid}\tthread --> {Thread.CurrentThread.ManagedThreadId}\twait --> {wait}\tevent --> 2");
            Task.Delay(wait).Wait();
            Console.WriteLine($"client: id --> {Id}\tevent --> 2 waited .");
        }
    }
}
