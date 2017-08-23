using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zh.fang.stu.task
{
    class Program
    {
        static ManualResetEvent _eventResetter = new ManualResetEvent(false);
        static List<int> _buffer = new List<int>();
        static readonly object _syncLocker = new object();

        public static char SwitchEndian(char x)
        {
            return (char)(
                (((char)((byte)(x))) << 8) |
                (((char)((byte)(x >> 8))))
                );
        }

        public static short SwitchEndian(short x)
        {
            return (short)(
                (((ushort)((byte)(x))) << 8) |
                (((ushort)((byte)(x >> 8))))
                );
        }

        public static int SwitchEndian(int x)
        {
            return
                (((int)((byte)(x))) << 24) |
                (((int)((byte)(x >> 8))) << 16) |
                (((int)((byte)(x >> 16))) << 8) |
                (((int)((byte)(x >> 24))));
        }

        public static long SwitchEndian(long x)
        {
            return
                (((long)((byte)(x))) << 56) |
                (((long)((byte)(x >> 8))) << 48) |
                (((long)((byte)(x >> 16))) << 40) |
                (((long)((byte)(x >> 24))) << 32) |
                (((long)((byte)(x >> 32))) << 24) |
                (((long)((byte)(x >> 40))) << 16) |
                (((long)((byte)(x >> 48))) << 8) |
                (((long)((byte)(x >> 56))));
        }

        static void Main(string[] args)
        {

            const char @char = 'a';
            Console.WriteLine(@char);
            const byte @byte = (byte)@char;
            Console.WriteLine(@byte);
            const char @charc = (char)@byte;
            Console.WriteLine(@charc);

            Console.WriteLine("----------------------------");
            char @charr = SwitchEndian(@char);
            Console.WriteLine("{0:x}", @charr);

            const short @short = 100;
            short @shortr = SwitchEndian(@short);

            const int @int = int.MaxValue;
            int @intr = SwitchEndian(@int);

            const long @long = long.MaxValue;
            long @longr = SwitchEndian(@long);

            var t = 0;
            Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                Console.WriteLine($"task thread id --> {System.Threading.Thread.CurrentThread.ManagedThreadId} -- > {t}");
            }).ConfigureAwait(false);
            Console.WriteLine($"main thread id --> {System.Threading.Thread.CurrentThread.ManagedThreadId} --> {t}");

            for (int i = 0; i < 999; i++)
            {
                //var thread = new Thread(ThreadInvoke);
                //thread.Start();
                Task.Factory.StartNew(() => ThreadInvoke());
            }

            _eventResetter.Set();
            

            
            Console.ReadKey();
            _eventResetter.Dispose();
            _eventResetter = null;

            var arr = (from v in _buffer group v by v into vs select vs).ToArray().OrderBy(g => g.Count());
            foreach (var item in arr)
            {
                Console.WriteLine("value:{0}, count:{1}", item.Key, item.Count());
            }

            Console.ReadKey();
        }

        static void ThreadInvoke()
        {
            Console.WriteLine("thread {0} is runnig ...", Thread.CurrentThread.ManagedThreadId);
            Task.Factory.StartNew(() => Display(PrimarykeyGenerator.Default.GenPrimaryKey()));
        }

        static void Display(int data)
        {
            lock(_syncLocker)
            {
                Console.Write("{0} ", data);
                _buffer.Add(data);
            }
        }
    }

    internal sealed class PrimarykeyGenerator
    {
        private static readonly object _syncLocker = new object();
        private static PrimarykeyGenerator _instance;

        private PrimarykeyGenerator() { }

        public int GenPrimaryKey()
        {
            var val = (int)RandomByte();
            val <<= 8;
            val |= RandomByte();
            val <<= 8;
            val |= RandomByte();
            val <<= 8;
            val |= RandomByte();

            if(0 > val)
            {
                val = (~val) + 1;
            }

            return val;
        }

        private byte RandomByte()
        {
            var buffer = new byte[1];
            var random = new Random(Guid.NewGuid().GetHashCode());
            random.NextBytes(buffer);

            return buffer[0];
        }

        public static PrimarykeyGenerator Default
        {
            get
            {
                if (null == _instance)
                {
                    lock (_syncLocker)
                    {
                        if (null == _instance)
                        {
                            _instance = new PrimarykeyGenerator();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
