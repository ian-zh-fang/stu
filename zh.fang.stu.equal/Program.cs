using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace zh.fang.stu.equal
{
    public class Program
    {
        static void Main(params string[] args)
        {
            //ResponseType left = null, right = null;
            //Debug.Assert(null == left);
            //Console.WriteLine(null == left);
            //Debug.Assert(left == right);
            //Console.WriteLine(left == right);

            //left = ResponseType.NOTIFY;
            //Console.WriteLine(left == right);
            //Debug.Assert(left != right);

            //right = ResponseType.NOTIFY;
            //Debug.Assert(left == right);
            //Console.WriteLine(left == right);

            //right = ResponseType.CALLBACK;
            //Debug.Assert(left != right);
            //Console.WriteLine(left == right);

            //string name = null;
            //Console.WriteLine(name == left);
            //Console.WriteLine(left == name);

            //name = "NOTIFY";
            //Console.WriteLine(name == left);
            //Console.WriteLine(left == name);

            //var code = 100u;
            //var none = BusinessType.None;
            //BusinessType business = code;
            //Console.WriteLine(code == BusinessType.None);
            //Console.WriteLine(business == BusinessType.QueryPersonalInformation);

            //var timestr = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            //Console.WriteLine("{0} -> {1}", timestr, timestr.Length);

            //var match = (new System.Text.RegularExpressions.Regex(@"^(\d{21})(\d{3})(\d)$")).Match(string.Format("{0}{1}{2}", timestr, "002", 0));
            //foreach (var group in match.Groups)
            //{
            //    Console.WriteLine(group);
            //}

            Console.WriteLine(string.Format("^[a-zA-Z0-9_-]{{0, {0}}}$", 60)); 

            string reqNo = new RequestNumber(BusinessType.QueryPersonalInformation);
            Console.WriteLine(reqNo);

            string reqNo2 = new RequestNumber(BusinessType.QueryPersonalInformation);
            Console.WriteLine(reqNo2);
            Console.WriteLine(reqNo == reqNo2);

            reqNo2 = new RequestNumber(reqNo);
            Console.WriteLine(reqNo2);
            Console.WriteLine(reqNo == reqNo2);
            Console.WriteLine(reqNo.Length);
            
            //string a = null;
            //a.CheckIsNull();

            //int? b = null;
            //b.CheckIsNull();

            Console.ReadKey();
        }
    }

    static class Helper
    {
        public static void CheckIsNull(this String input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException();
        }

        public static void CheckIsNull(this Object input)
        {
            if (Object.Equals(input, null))
                throw new ArgumentNullException();
        }
    }

    public sealed class RequestNumberFormatException : ArgumentException
    {
        public RequestNumberFormatException(string value, string pattern)
            : base(string.Format("{0} must be pattern format {1}", value, pattern))
        { }
    }

    public sealed class RequestNumber
    {
        // request number pattern
        private const string REQUESTNOSTRINGPATTERN = @"^(\d{15})(\d{3})(\d{4})$";
        // 21 bit timestamp pattern
        private const string TIMESTAMPPATTERN = @"^(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{3})$";
        // 21 bit timestamp formater
        private const string TIMESTAMPFORMTER = "yyMMddHHmmssfff";

        public RequestNumber(BusinessType businessType)
        {
            if (null == businessType)
            {
                throw new ArgumentNullException("businessType");
            }

            BusinessType = businessType;
            Timestamp = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
            RandomValue = GetRandom();
        }

        public RequestNumber(string requestNoString)
        {
            var match = Regex.Match(requestNoString, REQUESTNOSTRINGPATTERN);
            if (!match.Success)
            {
                throw new RequestNumberFormatException(requestNoString, REQUESTNOSTRINGPATTERN);
            }

            Timestamp = GetTimestamp(match.Groups[1].Value);
            BusinessType = uint.Parse(match.Groups[2].Value);
            RandomValue = int.Parse(match.Groups[3].Value);
        }

        private DateTime GetTimestamp(string timestampStr)
        {
            var tuple = GetTimestampTuple(timestampStr);
            var timestamp = new DateTime(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);
            return timestamp;
        }

        private Tuple<int, int, int, int, int, int, int> GetTimestampTuple(string timestampstr)
        {
            var match = Regex.Match(timestampstr, TIMESTAMPPATTERN);
            var tuple = new Tuple<int, int, int, int, int, int, int>(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value),
                int.Parse(match.Groups[4].Value),
                int.Parse(match.Groups[5].Value),
                int.Parse(match.Groups[6].Value),
                int.Parse(match.Groups[7].Value));
            return tuple;
        }

        // get one bit random int32
        private int GetRandom()
        {
            var seed = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            var random = new Random(seed);
            return random.Next(1000, 9999);
        }

        public static implicit operator string(RequestNumber requestNo)
        {
            return requestNo.ToString();
        }

        public static bool operator ==(RequestNumber left, RequestNumber right)
        {
            var nullLeft = object.Equals(left, null);
            var nullRight = object.Equals(right, null);
            if (nullLeft && nullRight) return true;

            if (nullLeft) return false;

            return left.Equals(right);
        }

        public static bool operator !=(RequestNumber left, RequestNumber right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null)) return false;

            if (!(obj is RequestNumber)) return false;

            var another = obj as RequestNumber;
            return
                RandomValue == another.RandomValue &&
                BusinessType == another.BusinessType &&
                Timestamp == another.Timestamp;
        }

        public override int GetHashCode()
        {
            return (int)((RandomValue + Timestamp.Millisecond) ^ BusinessType.Code);
        }

        public int RandomValue { get; private set; }

        public DateTime Timestamp { get; private set; }

        public BusinessType BusinessType { get; private set; }

        // Timestamp + BusinessType.Code + RandomValue
        public string Value
        {
            get { return this.ToString(); }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", Timestamp.ToString(TIMESTAMPFORMTER), BusinessType.Code, RandomValue);
        }
    }

    public sealed class BusinessType
    {
        private static readonly List<BusinessType> BusinessTypes = new List<BusinessType>();

        private BusinessType(uint code, string summary)
        {
            Code = code;
            Summary = summary;

            // init
            Init();
        }

        private void Init()
        {
            BusinessTypes.Add(this);
        }

        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null)) return false;

            if (!(obj is BusinessType)) return false;

            var another = obj as BusinessType;

            return
                uint.Equals(Code, another.Code) &&
                string.Equals(Summary, another.Summary);
        }

        public override int GetHashCode()
        {
            var hash = (int)Code;
            if (null != Summary)
            {
                hash ^= Summary.GetHashCode();
            }

            return hash;
        }

        public static bool operator ==(BusinessType left, BusinessType right)
        {
            var ln = object.Equals(left, null);
            var rn = object.Equals(right, null);
            if (ln && rn) return true;

            if (ln) return false;

            return left.Equals(right);
        }

        public static bool operator !=(BusinessType left, BusinessType right)
        {
            return !(left == right);
        }

        // 
        public static implicit operator BusinessType(uint code)
        {
            return Get(code);
        }

        public static BusinessType Get(uint code)
        {
            var business = BusinessTypes.FirstOrDefault(t => t.Code == code);
            return business ?? None;
        }

        public uint Code { get; private set; }

        public string Summary { get; private set; }

        public static readonly BusinessType None = new BusinessType(100u, "None");

        public static readonly BusinessType QueryPersonalInformation = new BusinessType(101u, "查询用户详细信息");

        public static readonly BusinessType QueryProductInfomation = new BusinessType(102u, "查询标的详细信息");
    }


    public sealed class ResponseType
    {
        private ResponseType(string name)
        {
            Name = name;
        }

        public override int GetHashCode()
        {
            if (null == Name)
            {
                return base.GetHashCode();
            }

            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;

            if (!(obj is ResponseType)) return false;

            var another = obj as ResponseType;
            return string.Equals(Name, another.Name);
        }

        public static bool operator ==(ResponseType left, ResponseType right)
        {
            var ln = object.Equals(left, null);
            var rn = object.Equals(right, null);

            if (ln && rn) return true;

            if (ln) return false;

            return left.Equals(right);
        }

        public static bool operator !=(ResponseType left, ResponseType right)
        {
            return !(left == right);
        }

        public static bool operator ==(ResponseType left, string name)
        {
            var right = new ResponseType(name);
            return left == right;
        }

        public static bool operator !=(ResponseType left, string name)
        {
            return !(left == name);
        }

        public static implicit operator string(ResponseType responseType)
        {
            return responseType.Name;
        }

        public string Name { get; private set; }

        /// <summary>
        /// browser return
        /// </summary>
        public static readonly ResponseType CALLBACK = new ResponseType("CALLBACK");

        /// <summary>
        /// async notify
        /// </summary>
        public static readonly ResponseType NOTIFY = new ResponseType("NOTIFY");
    }
}
