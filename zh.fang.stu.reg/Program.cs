namespace zh.fang.stu.reg
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    static class Program
    {
        static void Main(string[] args)
        {
            var pattern = @"^basic\s{0,}(?<root>.*?)$";

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

        }
    }
}
