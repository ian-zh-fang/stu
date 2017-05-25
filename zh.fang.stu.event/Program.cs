﻿namespace zh.fang.stu.@event
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    static class Program
    {
        static void Main(string[] args)
        {
            var source = new EventObserveProvider();
            var app = new EventSubcripionApp();
            app.Init(source);
            source.Exec();


            //var event_source = new EventSource();
            //var event_app = new EventApp();
            //event_app.Init(event_source);
            //event_source.Exec();


            Console.ReadKey();
        }
    }
}