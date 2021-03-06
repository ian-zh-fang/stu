﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zh.fang.stu.rabbitmq
{
    class Program
    {
        private static readonly object _syncLocker = new object();

        static void Main(string[] args)
        {
            ManualResetEventSlim eset = new ManualResetEventSlim(false);
            var statusBuffer = new List<bool>();
            Parallel.Invoke(
                () => {
                    var index = 0;
                    lock (_syncLocker)
                    {
                        index = statusBuffer.Count;
                        statusBuffer.Add(false);
                        Console.WriteLine("============================================================> {0}", index);
                    }

                    var outlen = 3;
                    var length = 0;
                    var offset = 0;
                    var outoffset = 0;
                    while (length > 0 && outlen > 0)
                    {
                        outoffset++;
                        for (int j = 0; j < length; j++)
                        {
                            //var thread = new Thread(() =>
                            //{
                            //    eset.Wait();
                            //    using (var s = new Sender())
                            //    {
                            //        s.Run();

                            //        Task.Factory.StartNew(() =>
                            //        {
                            //            lock (_syncLocker)
                            //            {
                            //                offset++;
                            //                Console.WriteLine("------------------------------------------------------------------> {0}:{1} -- {2}", index, length, offset);
                            //            }

                            //            if (length == offset)
                            //            {
                            //                statusBuffer[index] = true;
                            //            }
                            //        });
                            //    }
                            //});
                            //thread.Start();

                            Task.Run(() =>
                            {
                                //eset.Wait();
                                using (var s = new Sender())
                                {
                                    //s.Run();

                                    Task.Factory.StartNew(() =>
                                    {
                                        lock (_syncLocker)
                                        {
                                            offset++;
                                            Console.WriteLine("------------------------------------------------------------------> {0}:{1} -- {2}", index, outlen * length, offset);
                                        }

                                        if (outoffset * length == offset)
                                        {
                                            statusBuffer[index] = true;
                                        }
                                    });
                                }
                            });

                            Console.WriteLine("sender is ready --> {0}", j);
                        }

                        if(outoffset == outlen)
                        {
                            break;
                        }

                        while(true)
                        {
                            Console.WriteLine("check out ... ");
                            if(statusBuffer[index])
                            {
                                statusBuffer[index] = false;
                                break;
                            }

                            Task.Delay(1000).Wait();
                        }
                    }
                    //eset.Set();
                },
                () => {
                    var index = 0;
                    lock (_syncLocker)
                    {
                        index = statusBuffer.Count;
                        statusBuffer.Add(false);
                        Console.WriteLine("============================================================> {0}", index);
                    }
                    (new Receiver()).Run();

                    statusBuffer[index] = true;
                },
                () => { }
                );

            while(true)
            {
                Task.Factory.StartNew(() => Console.WriteLine(string.Join(" ", statusBuffer.Select((val, index) => $"{index}:{val}"))));

                var key = Console.ReadKey();
                if(key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.Q)
                {
                    Console.WriteLine(string.Join(" ", statusBuffer));

                    while (statusBuffer.Any(t => !t))
                    {
                        Task.Delay(1000).Wait();
                    }
                    break;
                }
            }
            eset.Dispose();
        }

        static void SenderRun(ManualResetEventSlim eventReseter, Action callback)
        {

        }
    }

    abstract class Client:IDisposable
    {
        private bool _isDisable = false;
        private ConnectionFactory _factory;

        protected virtual void Init()
        {
            _isDisable = false;
            InitConnectionFactory();
        }

        private void InitConnectionFactory()
        {
            _factory = new ConnectionFactory();
            _factory.HostName = "192.168.0.147";
            _factory.UserName = "root";
            _factory.Password = "root!";
        }

        public void Run()
        {
            Init();
            Run(_factory);
        }

        protected abstract void Run(ConnectionFactory factory);

        void IDisposable.Dispose()
        {
            if (_isDisable) return;

            _isDisable = true;
            _factory = null;
        }
    }

    sealed class Sender:Client
    {
        protected override void Run(ConnectionFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //while(true)
                    //{
                    //    Console.WriteLine("type key 'C' or 'Enter' to send message, 'CTRL + C' to exit.");
                    //    Console.WriteLine();

                    //    var key = Console.ReadKey();
                    //    if(key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                    //    {
                    //        break;
                    //    }

                    //    var msg = GetMessage();
                    //    SendMessage(msg, channel);
                    //}

                    var msg = Guid.NewGuid().ToString("N");
                    SendMessage(msg, channel);

                    Console.WriteLine("sender is shutdown .");
                }
            }
        }

        private string GetMessage()
        {
            Console.WriteLine("type in a message context:");
            Console.WriteLine();
            var msg = Console.ReadLine();
            while(string.IsNullOrEmpty(msg))
            {
                Console.WriteLine("empty message, retype in a message context:");
                Console.WriteLine();
                msg = Console.ReadLine();
            }

            return msg;
        }

        private void SendMessage(string message, IModel channel)
        {
            channel.QueueDeclare("hello", false, false, false, null);

            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "hello", null, body);
            Console.WriteLine(" set {0}", message);
        }
    }

    sealed class Receiver:Client
    {
        private byte[] buffer = null;
        private System.Threading.AutoResetEvent eventRest = new System.Threading.AutoResetEvent(false);

        protected override void Run(ConnectionFactory factory)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);
                    
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("hello", true, consumer);

                    Console.WriteLine(" waiting for message.");
                    consumer.Received += (s, e) => {
                        var body = buffer = e.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Received {0}", message);

                        eventRest.Reset();
                    };

                    eventRest.WaitOne();

                    Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer));

                    while (true)
                    {
                        var key = Console.ReadKey();
                        if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                        {
                            break;
                        }
                    }
                    Console.WriteLine("receiver is shutdown .");
                }
            }
        }
    }
}
