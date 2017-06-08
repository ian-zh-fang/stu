using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Newtonsoft.Json;

namespace zh.fang.stu._dynamic
{
    public class Program
    {
        static void Main(params string[] args)
        {
            var expando = new ExpandoObject();
            dynamic obj = expando;
            obj.a = 1;
            obj.timestamp = DateTime.Now;
            obj.st = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Console.WriteLine(obj);

            obj.data = new { a=1, b = 2};
            Console.WriteLine(obj);

            obj = JsonConvert.SerializeObject(obj);
            Console.WriteLine(obj);
        }
    }
}
