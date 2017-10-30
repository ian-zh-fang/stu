using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;

namespace zh.fang.stu.enumerate
{
    static class Program
    {
        static void Main(params string[] args)
        {
            Console.WriteLine("0x03 & 0x01 --> {0:x2}", 0x03 & 0x01);
            Console.WriteLine("0x03 & 0x02 --> {0:x2}", 0x03 & 0x02);
            Console.WriteLine("0x03 & 0xff --> {0:x2}", 0x03 & 0xff);
            Console.WriteLine("0x03 | 0xff --> {0:x2}", 0x03 | 0xff);

            var s = StringEnum.Get(null);
            var i = Int32Enum.Get(0);

            var a = new A();
            C(ref a);
            
        }

        static void C(ref A a)
        {
            a = null;
        }

        private class A { }
    }

    /// <summary>
    /// 旨在定义一种可枚举类型的基础结构
    /// </summary>
    /// <typeparam name="TEnum">内部枚举值类型</typeparam>
    public abstract class EnumDependancy<TEnum>
        where TEnum : IEquatable<TEnum>, IComparable<TEnum>
    {
        /// <summary>
        /// 一种收保护机制的构造函数
        /// </summary>
        /// <param name="value">枚举值</param>
        protected EnumDependancy(TEnum value)
            : base()
        {
            OnCheck(value);
            Value = value;
        }

        /// <summary>
        /// 指定值的有效性验证
        /// </summary>
        /// <param name="value">需要验证的值</param>
        protected virtual void OnCheck(TEnum value) { }

        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// 枚举值
        /// </summary>
        public TEnum Value { get; private set; }
    }

    /// <summary>
    /// 旨在定义一种可枚举类型的基础结构关系映射
    /// </summary>
    /// <typeparam name="TEnum">内部枚举值类型</typeparam>
    /// <typeparam name="TValue">枚举关系映射类型</typeparam>
    public abstract class EnumDependancy<TEnum, TValue>
        : EnumDependancy<TEnum>
        where TValue : EnumDependancy<TEnum>
        where TEnum : IEquatable<TEnum>, IComparable<TEnum>
    {
        // 线程安全同步锁
        private static readonly object _SyncLocker = new object();

        // 线程安全的枚举值映射对象集合
        private static ConcurrentBag<TValue> _enumValues;

        /// <summary>
        /// 一种收保护机制的构造函数
        /// </summary>
        /// <param name="value">枚举值</param>
        protected EnumDependancy(TEnum value)
            : base(value)
        { }

        /// <summary>
        /// 从枚举值映射对象集合中查询和指定枚举值匹配的枚举上下文实例，并返回匹配的枚举上下文实例。若匹配失败，返回 null。
        /// </summary>
        /// <param name="value">需要匹配的枚举值</param>
        /// <returns></returns>
        protected static TValue GetEnum(TEnum value)
        {
            var arr = InitEnum();
            if (arr == null) { return null; }

            return arr.FirstOrDefault(t => object.Equals(t.Value, value));
        }

        // 初始化枚举值映射对象集合
        private static ConcurrentBag<TValue> InitEnum()
        {
            if (_enumValues == null)
            {
                lock (_SyncLocker)
                {
                    if (_enumValues == null)
                    {
                        try
                        {
                            var arr =
                                typeof(TValue).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                                .Select(t => t.GetValue(null) as TValue)
                                .Where(t => t != null);
                            _enumValues = new ConcurrentBag<TValue>(arr);
                        }
                        catch (Exception) { }
                    }
                }
            }

            return _enumValues;
        }
    }

    class StringEnum:EnumDependancy<string, StringEnum>
    {
        private StringEnum(string value)
            :base(value)
        { }

        public static StringEnum Get(string v)
        {
            return GetEnum(v);
        }

        public static readonly StringEnum None = new StringEnum(null);

        public static readonly StringEnum Post = new StringEnum("post");
    }

    class Int32Enum:EnumDependancy<Int32, Int32Enum>
    {
        private Int32Enum(int value)
            :base(value)
        { }

        public static Int32Enum Get(int v)
        {
            return GetEnum(v);
        }

        public static readonly Int32Enum None = new Int32Enum(0);
    }

    public abstract class EnumContext
    {
        protected EnumContext(string name, string summary)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            Name = name;
            Summary = summary;
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            if (null != Summary)
            {
                hash += Summary.GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null)) return false;
            if (!(obj is EnumContext)) return false;

            var another = obj as EnumContext;

            return
                string.Equals(Name, another.Name) &&
                string.Equals(Summary, another.Summary);
        }

        public static implicit operator string(EnumContext context)
        {
            if (null == context) return null;

            return context.Name;
        }

        public static bool operator ==(EnumContext left, EnumContext right)
        {
            var nullLeft = object.Equals(left, null);
            var nullRight = object.Equals(right, null);
            if (nullLeft && nullRight) return true;

            if (nullLeft) return false;

            return left.Equals(right);
        }

        public static bool operator !=(EnumContext left, EnumContext right)
        {
            return !(left == right);
        }

        public string Name { get; private set; }

        public string Summary { get; private set; }
    }

    public abstract class EnumContext<TEnumContext> : EnumContext
        where TEnumContext : EnumContext
    {
        private static readonly List<TEnumContext> Types = new List<TEnumContext>();

        protected EnumContext(string name, string summary) 
            : base(name, summary)
        {
            Types.Add(this as TEnumContext);
        }

        protected static TEnumContext Get(string name)
        {
            return Types.FirstOrDefault(t => t.Name == name);
        }
    }

    public sealed class AccessTypeEnum : EnumContext<AccessTypeEnum>
    {
        public AccessTypeEnum(string name, string summary)
            : base(name, summary) { }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator AccessTypeEnum(string name)
        {
            return Get(name) ?? None;
        }

        public static bool operator ==(AccessTypeEnum left, AccessTypeEnum right)
        {
            return EnumContext.Equals(left, right);
        }

        public static bool operator !=(AccessTypeEnum left, AccessTypeEnum right)
        {
            return !(left == right);
        }

        public static readonly AccessTypeEnum None = new AccessTypeEnum("None", null);

        public static readonly AccessTypeEnum FULL_CHECKED = new AccessTypeEnum("FULL_CHECKED", "四要素验证通过 ");

        public static readonly AccessTypeEnum NOT_AUTH = new AccessTypeEnum("NOT_AUTH", "未鉴权 ");

        public static readonly AccessTypeEnum AUDIT_AUTH = new AccessTypeEnum("AUDIT_AUTH", "特殊用户认证");
    }

    public sealed class A : EnumContext<A>
    {
        public A(string name, string summary = null) : base(name, summary) { }

        public static implicit operator A(string name)
        {
            return Get(name);
        }

        public static readonly A a = new A("none");
    }

    public sealed class B : EnumContext<B>
    {
        private B(string name, string summary = null) : base(name, summary) { }

        public static implicit operator B(string name)
        {
            return Get(name);
        }

        public static readonly B b = new B("none");
    }
}
