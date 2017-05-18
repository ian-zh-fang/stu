using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;

namespace zh.fang.stu.clone
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            HttpApplication app = new HttpApplication();
            var app1 = app.Clone();
        }

        public static object Clone(this object instance)
        {
            if (null == instance)
                return instance;

            var type = instance.GetType();
            if (type.IsValueType)
                return instance;

            var newInstance = Activator.CreateInstance(type);

            var members = type.GetMembers(BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic);
            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Field)
                {
                    SetMember((FieldInfo)member, instance, ref newInstance);                    
                }

                if(member.MemberType == MemberTypes.Property)
                {
                    SetMember((PropertyInfo)member, instance, ref newInstance);
                }

                if(member.MemberType == MemberTypes.Event)
                {
                    SetMember((EventInfo)member, instance, ref newInstance);
                }

                if(member.MemberType == MemberTypes.Method)
                {
                    SetMember((MethodInfo)member, instance, ref newInstance);
                }                
            }
            return newInstance;
        }

        private static object CloneInstance(object instance)
        {
            if (instance is ICloneable)
                return ((ICloneable)instance).Clone();

            return instance.Clone();
        }

        private static void SetMember(FieldInfo member, object instance, ref object newInstance)
        {
            var val = member.GetValue(instance);
            var cloneVal = CloneInstance(val);
            if (null != cloneVal)
                member.SetValue(newInstance, cloneVal);
        }

        private static void SetMember(PropertyInfo member, object instace, ref object newInstance)
        {

            var val = member.GetValue(instace);
            var cloneVal = CloneInstance(val);
            if (null != cloneVal)
                member.SetValue(newInstance, cloneVal);
        }

        private static void SetMember(EventInfo member, object instace, ref object newInstance)
        {
            
        }

        private static void SetMember(MethodInfo member, object instace, ref object newInstance)
        {
            
        }
    }
}
