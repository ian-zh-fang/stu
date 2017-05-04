namespace zh.fang.stu.timestamp
{
    using System;

    static class Program
    {
        static void Main(string[] args)
        {
            var length = 7200000;
            var offset = DateTime.Now.ToUnixtMillisecond() + length;
            Console.WriteLine(offset);
            Console.WriteLine((new DateTime(offset * 10000)).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }        
    }

    static class DateTimeHelper
    {
        public static long ToUnixt(this DateTime datetime)
        {
            return (DateTime.Now.ToUniversalTime() - UnixSeed().ToUniversalTime()).Ticks;
        }

        public static long ToUnixtMillisecond(this DateTime datetime)
        {
            return datetime.ToUnixt() / 10000;
        }

        private static DateTime UnixSeed()
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
        }
    }
}
