using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace zh.fang.stu.serializa
{
    public class Program
    {
        static void Main(params string[] args)
        {
            var str = "{\"rawName\":\"iaim\",\"balance\":0.0064}";
            for (int i = 0; i < 5000; i++)
            {
                Test(str);
            }

            Console.ReadKey();
        }

        private static void Test(string str)
        {
            var watch = new Stopwatch();
            watch.Start();
            var example = new Example();
            //var json = JsonConvert.SerializeObject(example);
            //Console.WriteLine(json);

            //var a = new { rawName = "iaim", balance = 0.0064 };
            //json = JsonConvert.SerializeObject(a);
            //Console.WriteLine(json);

            example = JsonConvert.DeserializeObject<Example>(str);
            //var json = JsonConvert.SerializeObject(example);
            //Console.WriteLine(json);
            watch.Stop();
            Console.Write(watch.ElapsedMilliseconds+" ");
        }
    }

    public abstract class BaseSerializable : ISerializable
    {
        protected BaseSerializable() { }

        protected BaseSerializable(SerializationInfo info, StreamingContext context)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value = GetPropertyValue(property, info);
                if (null == value) continue;

                property.SetValue(this, value, null);
            }
        }

        private object GetPropertyValue(PropertyInfo property, SerializationInfo info)
        {
            var value = GetPropertyValue(property.Name, property.PropertyType, info);
            if (null != value) return value;

            var name = GetPropertyName(property);
            value = GetPropertyValue(name, property.PropertyType, info);
            return value;
        }

        protected object GetPropertyValue(string name, Type valueType, SerializationInfo info)
        {
            try
            {
                return info.GetValue(name, valueType);
            }
            catch { return null; }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);    
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                info.AddValue(GetPropertyName(property), GetPropertyValueString(property));
            }
        }

        private object GetPropertyValueString(PropertyInfo property)
        {
            var value = property.GetValue(this, null);
            if (object.Equals(value, null)) return null;

            var attr = property.GetCustomAttributes().FirstOrDefault(t => t is IPropertyValueAttribute) as IPropertyValueAttribute;
            if (null == attr) return value.ToString();

            return attr.GetValue(value);
        }

        private string GetPropertyName(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes<ParameterNameAttribute>().FirstOrDefault();
            if (null == attr) return property.Name.FirstLetterToLower();

            return attr.RawName;
        }
    }
    
    public class Example:BaseSerializable
    {
        public Example() : base() { }

        protected Example(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [DecimalValue()]
        [DecimalValueToString()]
        [ParameterName("myBalance")]
        public decimal balance { get; set; } = 2.00542M;

        public string RawName { get; set; } = "aimy";
    }

    public interface IParameterNameAttribute
    {
        string RawName { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ParameterNameAttribute : Attribute, IParameterNameAttribute
    {
        public ParameterNameAttribute(string name)
        {
            name.CheckIsNullOrWhiteSpaceAndThrow("name");
            RawName = name;
        }

        // 参数实际名称
        public string RawName { get; private set; }
    }

    public interface IPropertyValueToStringAttribute
    {
        string ToString(Type propertyType, object value);
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class PropertyValueToStringAttribute : Attribute, IPropertyValueToStringAttribute
    {
        protected PropertyValueToStringAttribute(Type propertyType)
        {
            propertyType.CheckIsNullAndThrow("propertyType");
            PropertyType = propertyType;
        }

        string IPropertyValueToStringAttribute.ToString(Type propertyType, object value)
        {
            if (object.Equals(value, null)) return null;

            propertyType.CheckTypeNotEqualsAndThrow(PropertyType);
            return ToString(value);
        }

        protected virtual string ToString(object value)
        {
            return string.Format("{0}", value);
        }

        public Type PropertyType { get; private set; }
    }



    public interface IPropertyValueAttribute
    {
        object GetValue(object data);
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class PropertyTypeCheckAndGetValueAttribute : Attribute, IPropertyValueAttribute
    {
        protected PropertyTypeCheckAndGetValueAttribute(Type propertyType)
        {
            propertyType.CheckIsNullAndThrow("propertyType");
            PropertyType = propertyType;
        }

        object IPropertyValueAttribute.GetValue(object data)
        {
            if (object.Equals(data, null)) return data;

            PropertyType.CheckTypeNotEqualsOrSubClassOrImplInterfaceAndThrow(data.GetType());
            return GetValue(data);
        }

        protected abstract object GetValue(object data);

        public Type PropertyType { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DecimalValueAttribute : PropertyTypeCheckAndGetValueAttribute
    {

        // decimals 小数位数，默认 2 位，最小值 0
        public DecimalValueAttribute(int decimals = 2) : base(typeof(Decimal))
        {
            decimals.CheckInt32LessThenThrow(0, "decimals");
            Decimals = decimals;
        }

        protected override object GetValue(object data)
        {
            var value = (Decimal)data;
            value = Math.Round(value, Decimals);
            return value;
        }

        // 小数位数
        public int Decimals { get; private set; }
    }

    // Decimal 类型数据转换为 String 对象
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DecimalValueToStringAttribute : PropertyValueToStringAttribute
    {
        // decimals 小数位数，默认 2 位，最小值 0
        public DecimalValueToStringAttribute(int decimals = 2) : base(typeof(Decimal))
        {
            decimals.CheckInt32LessThenThrow(0, "decimals");
            Decimals = decimals;
        }

        protected override string ToString(object value)
        {
            value.CheckIsNullAndThrow("value");
            var data = (Decimal)value;
            data = Math.Round(data, Decimals);

            return base.ToString(data);
        }

        // 小数位数
        public int Decimals { get; private set; }
    }

    public interface IPropertyValueHandleAttribute
    {
        object GetData(Type propertyType, object value);

        Type OutputType { get; }
    }



    internal static class CheckHelper
    {
        // string check

        public static void CheckIsNullOrWhiteSpaceAndThrow(this String input, String inputName)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(inputName);
            }
        }

        // object check

        public static void CheckIsNullAndThrow(this Object input, String inputName)
        {
            if (Object.Equals(input, null))
            {
                throw new ArgumentNullException(inputName);
            }
        }

        // int32 check

        public static void CheckInt32LessThenThrow(this Int32 input, Int32 maxValue, String inputName)
        {
            if (input < maxValue)
            {
                throw new ArgumentOutOfRangeException(inputName);
            }
        }

        public static void CheckInt32MoreThenThrow(this Int32 input, Int32 minValue, String inputName)
        {
            if (input > minValue)
            {
                throw new ArgumentOutOfRangeException(inputName);
            }
        }

        // regex match

        public static Match CheckStringMatchAndThrow(this String input, String pattern)
        {
            input.CheckIsNullOrWhiteSpaceAndThrow("input");
            var match = Regex.Match(input, pattern);
            if (match.Success)
            {
                return match;
            }

            throw new DataFormatException(input, pattern);
        }

        public static void CheckStringIsMatchAndThrow(this String input, String pattern)
        {
            if (!Regex.IsMatch(input, pattern))
            {
                throw new DataFormatException(input, pattern);
            }
        }

        // type check

        public static void CheckTypeNotEqualsAndThrow(this Type input, Type target)
        {
            input.CheckIsNullAndThrow("input");
            target.CheckIsNullAndThrow("target");

            if (!string.Equals(input.FullName, target.FullName))
            {
                ThrowTypeNotMatchException(input, target);
            }
        }

        public static void CheckTypeNotSubClassAndThrow(this Type input, Type target)
        {
            input.CheckIsNullAndThrow("input");
            target.CheckIsNullAndThrow("target");

            if (!input.IsSubclassOf(target))
            {
                ThrowTypeNotMatchException(input, target);
            }
        }

        public static void CheckTypeNotImplInterfaceAndThrow(this Type input, Type target)
        {
            input.CheckIsNullAndThrow("input");
            target.CheckIsNullAndThrow("target");

            if (!target.IsInterface)
            {
                throw new ArgumentException(string.Format("type {0} must be interface", target));
            }

            if (!target.IsAssignableFrom(input))
            {
                ThrowTypeNotMatchException(input, target);
            }
        }

        public static void CheckTypeNotEqualsOrSubClassAndThrow(this Type input, Type target)
        {
            input.CheckIsNullAndThrow("input");
            target.CheckIsNullAndThrow("target");

            if (string.Equals(input.FullName, target.FullName)) return;
            if (input.IsSubclassOf(target)) return;
            ThrowTypeNotMatchException(input, target);
        }

        public static void CheckTypeNotEqualsOrSubClassOrImplInterfaceAndThrow(this Type input, Type target)
        {
            input.CheckIsNullAndThrow("input");
            target.CheckIsNullAndThrow("target");

            if (string.Equals(input.FullName, target.FullName)) return;
            if (input.IsSubclassOf(target)) return;
            if (target.IsAssignableFrom(input)) return;

            ThrowTypeNotMatchException(input, target);
        }

        private static void ThrowTypeNotMatchException(Type type, Type target)
        {
            throw new ArgumentException(string.Format("type {0} can not convert to target type {1}", type, target));
        }
    }

    [Serializable]
    internal class DataFormatException : Exception
    {
        private string input;
        private string pattern;

        public DataFormatException()
        {
        }

        public DataFormatException(string message) : base(message)
        {
        }

        public DataFormatException(string input, string pattern)
        {
            this.input = input;
            this.pattern = pattern;
        }

        public DataFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    internal static class StringHelper
    {
        // first letter to lower
        public static String FirstLetterToLower(this String input)
        {
            if (string.Equals(input, null)) return null;
            if (0 == input.Length) return null;
            if (1 == input.Length) return input.ToLower();

            return String.Format("{0}{1}", input.Substring(0, 1).ToLower(), input.Substring(1));
        }
    }
}
