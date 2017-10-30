using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace zh.fang.stu.attribute
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic = new Dictionary<int, List<int>>();
            var rdm = new Random();
            for (int i = 0; i < 100; i++)
            {
                var k = rdm.Next(1, 10);
                var val = rdm.Next(10, 100);
                
                if(!dic.ContainsKey(k))
                {
                    dic.Add(k, new List<int>());
                }

                dic[k].Add(val);
                Console.Write("{0}, ", val);
            }

            Console.WriteLine("\n----------------------------");
            var q = from v in dic.Values
                    from vv in v
                    select vv;
            Console.WriteLine(string.Join(", ", q.Distinct()));




            var e = new Demo();
            e.MyProperty = "    ";
            e.GetType().GetProperties().AsParallel().ForAll(t => {
                var v = t.GetMethod.Invoke(e, null);
                Console.WriteLine("value: {0}", v);
                var arr = t.GetCustomAttributes<IgnoreAttribute>();
                foreach (var attr in arr)
                {
                    Console.WriteLine("{0} --> {1}", attr.IsIgnore(v), attr.ToString());
                }
            });

            Console.ReadKey();
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple =true, Inherited = true)]
    class IgnoreAttribute:Attribute
    {
        private readonly object _value;

        public IgnoreAttribute(object ignorableValue)
        {
            _value = ignorableValue;
        }

        public bool IsIgnore(object value)
        {
            return IsIgnore(value, _value);
        }

        protected virtual bool IsIgnore(object value, object ignore)
        {
            return object.Equals(value, ignore);
        }
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    sealed class IgnorableNullAttribute : IgnoreAttribute
    {
        public IgnorableNullAttribute()
            : base(null)
        { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple =false, Inherited = true)]
    sealed class IgnorableRegexMatchAttribute:IgnoreAttribute
    {
        public IgnorableRegexMatchAttribute(string pattern)
            :base(new Regex(pattern))
        { }

        protected override bool IsIgnore(object value, object ignore)
        {
            var rgx = ignore as Regex;

            return rgx.IsMatch(value.ToString());
        }
    }

    class Demo
    {
        [Ignore(1)]
        [Ignore("")]
        [IgnorableRegexMatch(@"^\s+$")]
        [IgnorableNull()]
        public Object MyProperty { get; set; }
    }
}
