namespace zh.fang.stu.reg
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    static class Program
    {
        static void Main(string[] args)
        {
            var match = Regex.Match(
                "serviceName=asdasdas&platformNo=adsfasd&userDevice=pc&respData=sdasdad&sign=sadasdas&responseType=Callback",
                @"^(?<p>(?<kv>\w+[=]\w{0,})[&]?)+$");

            var matches = Regex.Matches(
                "serviceName=asdasdas&platformNo=adsfasd&userDevice=pc&respData=sdasdad&sign=sadasdas&responseType=Callback",
                @"^(?<p>(?<kv>\w+[=]\w{0,})[&]?)+$");

            var sactor = typeof(StaticClsImpl).GetConstructors(BindingFlags.Static);

            StaticClsImpl.Init();
            Console.WriteLine(StaticClsImpl.Get());

            Console.WriteLine(Convert.ChangeType("0", TypeCode.UInt32));
            var data = "a".ConverTo(typeof(Int32));
            data = "111".ConverTo(typeof(Int32));
            data = "002222".ConverTo(typeof(Int32));

            var pattern = @"^(?<name>\w{0,})[=](?<value>\S{0,})$";
            TestPatter("a=b", pattern);
            TestPatter("a=b=", pattern);
            TestPatter("a=", pattern);
            TestPatter("=b", pattern);
            TestPatter("=", pattern);
            TestPatter("==", pattern);
            TestPatter("", pattern);

            Console.WriteLine(Regex.Match("-", @"^[-]\d+$").Success);
            Console.WriteLine(Regex.Match("-0", @"^[-]\d+$").Success);
            Console.WriteLine(Regex.Match("0", @"^[-]\d+$").Success);
            Console.WriteLine(Regex.Match("-111", @"^[-]\d+$").Success);
            Console.WriteLine(Regex.Match("--1", @"^[-]\d+$").Success);

            pattern = @"^basic\s{0,}(?<root>.*?)$";

            var val = "basic   ccccc_da&&943";
            var arr = Regex.Match(val, pattern).Groups;
            var group = arr["root"].Value;
            Debug.Assert(!string.IsNullOrWhiteSpace(group));


            val = "bac ccccc_da&&943";
            arr = Regex.Match(val, pattern).Groups;
            group = arr["root"].Value;
            Debug.Assert(string.IsNullOrWhiteSpace(group));

            var encoding = Encoding.UTF8;
            var str = "a505:0943";
            var buffer = encoding.GetBytes(str);
            var base64Str = Convert.ToBase64String(buffer);

            // mscorlib
            // System
            // Microsoft
        }

        static void TestPatter(string content, string pattern)
        {
            Console.WriteLine("\n--------------------");
            var match = Regex.Match(content, pattern);
            Console.WriteLine(match.Success);
            if (!match.Success) return;

            for (int i = 0; i < match.Groups.Count; i++)
            {
                var grp = match.Groups[i];
                Console.WriteLine(grp.GetType().FullName);
                Console.WriteLine($"{grp.Value}");
            }
        }
    }

    internal abstract class STCls { }

    internal class StaticCls<TCls>:STCls
        where TCls :STCls
    {
        protected static readonly List<TCls> Items = new List<TCls>();

        static StaticCls() { }

        protected StaticCls()
        {
            Items.Add(this as TCls);
        }

        public static TCls Get()
        {
            return Items.FirstOrDefault();
        }

        private static TCls GetDefault(Func<TCls> defaultVal)
        {
            if (null == defaultVal) return null;

            var val = defaultVal.Invoke();
            if(null == val)
            {
                val = Get();
            }

            return val;
        }

        public static void Init() { }
    }

    internal class StaticClsImpl: StaticCls<StaticClsImpl>
    {
        static StaticClsImpl() { }

        private StaticClsImpl():base() { }

        public new static StaticClsImpl Get()
        {
            return Items.FirstOrDefault();
        }

        public static readonly StaticClsImpl None = new StaticClsImpl();

        public static readonly StaticClsImpl Empty = new StaticClsImpl();
    }

    internal static class TypeHelper
    {
        private const String TRYPARSEMETHODNAME = "TryParse";
        private const String WHITESPACEPATTERN = @"^\s+$";

        public static MethodInfo GetTrypareMethod(this Type inputType)
        {
            if (null == inputType) return null;
            var method = inputType.GetMethod(TRYPARSEMETHODNAME,
                BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder,
                new Type[] { typeof(String), inputType.MakeByRefType() },
                new ParameterModifier[] { new ParameterModifier(2) });
            return method;
        }

        public static Object ConverTo(this String input, Type conversionType)
        {
            if (input.IsNullOrEmptyOrWhitespace()) return null;
            if (null == conversionType) return null;

            var data =
                GetDataByTryParse(input, conversionType) ??
                GetDataByConvert(input, conversionType);
            return data;
        }

        private static Object GetDataByTryParse(String data, Type conversionType)
        {
            try
            {
                var method = conversionType.GetTrypareMethod();
                if (null == method) return null;

                if (method.ReturnType != typeof(Boolean)) return null;

                var parameters = new Object[] { data, Activator.CreateInstance(conversionType) };
                var isOk = (Boolean)method.Invoke(null, parameters);
                if (isOk) return parameters[1];

                return null;
            }
            catch (Exception) { return null; }
        }

        private static object GetDataByConvert(String data, Type conversionType)
        {
            try
            {
                return Convert.ChangeType(data, conversionType);
            }
            catch (Exception) { return null; }
        }

        public static Boolean IsNullOrEmptyOrWhitespace(this String input)
        {
            if (input == null) return true;
            if (input == string.Empty) return true;
            if (Regex.IsMatch(input, WHITESPACEPATTERN)) return true;

            return false;
        }
    }
}
