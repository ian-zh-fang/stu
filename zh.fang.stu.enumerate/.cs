using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zh.fang.stu.enumerate
{
    static class Program
    {
        static void Main(params string[] args)
        {
            //var a = A.a;
            //var b = B.b;

            AccessTypeEnum at1 = null;
            AccessTypeEnum at2 = AccessTypeEnum.FULL_CHECKED;
            Console.WriteLine(at1);
            Console.WriteLine(at2);
            Console.WriteLine(null == at1);
            Console.WriteLine(at1 == at2);

            Console.ReadKey();
        }
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
