using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zh.fang.stu.encryption
{
    using System.Security.Cryptography;

    class Program
    {
        static void Main(string[] args)
        {
            var buffstr = "aaa";
            Console.WriteLine($"target string is {buffstr}");

            var buffer = Encoding.UTF8.GetBytes(buffstr);
            Console.WriteLine($"encoding string is {BitConverter.ToString(buffer)}");

            var md5 = MD5.Create();
            Console.WriteLine($"{md5.ToString()} string is {BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "")}");

            md5 = new MD5CryptoServiceProvider();
            Console.WriteLine($"{md5.ToString()} string is {BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "")}");

            Console.ReadKey();
        }
    }
}
