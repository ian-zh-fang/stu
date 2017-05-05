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

        internal void Exec()
        {
            Console.WriteLine($"source thread --> {Thread.CurrentThread.ManagedThreadId} .");
            action?.Invoke();
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
                    var handler = Delegate.CreateDelegate(event_info.EventHandlerType, hash, method);
                    event_info.AddEventHandler(this, handler);
                }
            }
        }

        void Callback(int hashCode)
        {
            Parallel.Invoke(TaskCollection[hashCode].ToArray());
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
            app.RegisterEvent(eventSource, "action", () => CallbackCore());
        }

        void CallbackCore()
        {
            Console.WriteLine($"client: id --> {Id}\tuuid --> {Uuid}\tthread --> {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
