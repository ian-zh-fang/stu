using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zh.fang.stu.equalable
{
    class Program
    {
        static void Main(string[] args)
        {
            const int size = 15;
            var arr = new IExample[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = new Example();
            }

            arr.OrderByDescending(t => t);
            arr.OrderBy(t => t).ToList().ForEach(t => Console.Write("{0} ", t));

            //Console.WriteLine();
            //arr.Select(t => t as Example).OrderBy(t => t).ToList().ForEach(t => Console.Write("{0} ", t));

            //Console.WriteLine();
            //arr.GroupBy(t => t).Select(t => t.Key).ToList().ForEach(t => Console.Write("{0} ", t));

            //Console.WriteLine();
            //Console.WriteLine(arr[0] == arr[1]);

            Console.ReadKey();
        }
    }

    public interface IExample
        :IEquatable<IExample>
        ,IComparable<IExample>
    { }

    public class Example
        : EqualityDependancy<Example>
        , IExample
        , IEqualityDependancy<Example>
        , IEquatable<Example>
        , IComparable<Example>
    {
        private static readonly Random _Random = new Random();

        public Example()
        {
            Data = _Random.Next(0, 10);
        }

        protected override int Compare(Example other)
        {
            return Data.CompareTo(other.Data);
        }

        protected override bool Equals(Example instance)
        {
            Console.WriteLine("equals check .");
            return Data == instance.Data;
        }

        protected override int GetHash()
        {
            Console.WriteLine("hash --> {0}", Data);
            return Data;
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        bool IEquatable<IExample>.Equals(IExample other)
        {
            Console.WriteLine("bool IEquatable<IExample>.Equals(IExample other)");
            return ((IEquatable<Example>)this).Equals(other as Example);
        }

        int IComparable<IExample>.CompareTo(IExample other)
        {
            Console.WriteLine("int IComparable<IExample>.CompareTo(IExample other)");
            return base.CompareTo(other as Example);
        }

        public int Data { get; private set; }
    }

    public interface IEqualityDependancy<TSource>
        : IEquatable<TSource>
        , IComparable<TSource>
        where TSource : IEqualityDependancy<TSource>
    { }

    // 定义对象相等依赖的基础处理
    //  指定的对象应该派生至 EqualityDependancy 
    //  TSource 源对象类型
    public abstract class EqualityDependancy<TSource>
        : IEqualityDependancy<TSource>
        , IEquatable<TSource>
        , IComparable<TSource>
        where TSource : EqualityDependancy<TSource>
    {
        protected EqualityDependancy() { }

        public Int32 CompareTo(TSource other)
        {
            if (other.IsNull()) { throw new ArgumentNullException(); }
            return Compare(other);
        }

        Boolean IEquatable<TSource>.Equals(TSource other)
        {
            if (other.IsNull()) { return false; }

            return Equals(other);
        }

        public sealed override bool Equals(object obj)
        {
            var oth = obj as TSource;
            return ((IEquatable<TSource>)this).Equals(oth);
        }

        public sealed override int GetHashCode()
        {
            return GetHash();
        }

        public static Boolean operator ==(EqualityDependancy<TSource> left, EqualityDependancy<TSource> right)
        {
            var nol = left.IsNull();
            if (nol && right.IsNull()) { return true; }
            if (nol) { return false; }

            return left.Equals(right);
        }

        public static Boolean operator !=(EqualityDependancy<TSource> left, EqualityDependancy<TSource> right)
        {
            return !(left == right);
        }

        protected abstract Int32 GetHash();

        protected abstract Boolean Equals(TSource other);

        protected abstract Int32 Compare(TSource other);
    }

    public static class ObjectExt
    {
        /// <summary>
        /// 目标对象不存在时，返回 true ; 否则，返回 false .
        /// </summary>
        public static Boolean IsNull(this Object input)
        {
            return Object.Equals(input, null);
        }

        /// <summary>
        /// 当前值是否大于指定的最小值，在当前值存在，并且大于最小值时，返回 true；否则，返回 false
        /// </summary>
        /// <typeparam name="TSource">当前值类型，一种可比较的类型</typeparam>
        /// <param name="source">当前值</param>
        /// <param name="minVal">最小值</param>
        /// <returns></returns>
        public static Boolean IsMoreThen<TSource>(this TSource source, TSource minVal)
            where TSource : IComparable<TSource>
        {
            if (source.IsNull()) return false;

            return 0 < source.CompareTo(minVal);
        }

        /// <summary>
        /// 当前值是否小于指定的最大值，在当前值存在，并且小于最大值时，返回 true；否则，返回 false
        /// </summary>
        /// <typeparam name="TSource">当前值类型，一种可比较的类型</typeparam>
        /// <param name="source">当前值</param>
        /// <param name="maxVal">最大值</param>
        /// <returns></returns>
        public static Boolean IsLessThen<TSource>(this TSource source, TSource maxVal)
            where TSource : IComparable<TSource>
        {
            if (source.IsNull()) return false;
            return 0 > source.CompareTo(maxVal);
        }

        /// <summary>
        /// 当前值是否大于或者等于指定的最小值，在当前值存在，并且大于或者等于最小值时，返回 true；否则，返回 false
        /// </summary>
        /// <typeparam name="TSource">当前值类型，一种可比较的类型</typeparam>
        /// <param name="source">当前值</param>
        /// <param name="minVal">最小值</param>
        /// <returns></returns>
        public static Boolean IsMoreThenOrEqual<TSource>(this TSource source, TSource minVal)
            where TSource : IComparable<TSource>
        {
            if (source.IsNull()) return false;
            return 0 <= source.CompareTo(minVal);
        }

        /// <summary>
        /// 当前值是否小于或者等于指定的最大值，在当前值存在，并且小于或者等于最大值时，返回 true；否则，返回 false
        /// </summary>
        /// <typeparam name="TSource">当前值类型，一种可比较的类型</typeparam>
        /// <param name="source">当前值</param>
        /// <param name="maxVal">最大值</param>
        /// <returns></returns>
        public static Boolean IsLessThenOrEqual<TSource>(this TSource source, TSource maxVal)
            where TSource : IComparable<TSource>
        {
            if (source.IsNull()) return false;
            return 0 >= source.CompareTo(maxVal);
        }
    }
}
