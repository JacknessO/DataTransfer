using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    public class CustomModelInfo
    {
        private static readonly Hashtable dic = Hashtable.Synchronized(new Hashtable());
        public static object GetModelItemValue<T>(T custom, string propertyName)
        {
            string key = string.Format("{0}_{1}", typeof(T).Name, propertyName);
            if (!dic.Contains(key))
            {
                dic.Add(key, typeof(T).GetProperty(propertyName));
            }
            return ReflectionDelegated.GetPropertyGetterWrapper((PropertyInfo)dic[key]).Get(custom);
        }
        public static void SetModelItemValue<T>(T custom, string propertyName, object value)
        {
            string key = string.Format("{0}_{1}", typeof(T).Name, propertyName);
            if (!dic.Contains(key))
            {
                dic.Add(key, typeof(T).GetProperty(propertyName));
            }
            ReflectionDelegated.GetPropertySetterWrapper((PropertyInfo)dic[key]).Set(custom, value);
        }
        public static void SetModelItemValue(object custom, string propertyName, object value)
        {
            string key = string.Format("{0}_{1}", custom.GetType().Name, propertyName);
            if (!dic.Contains(key))
            {
                dic.Add(key, custom.GetType().GetProperty(propertyName));
            }
            ReflectionDelegated.GetPropertySetterWrapper((PropertyInfo)dic[key]).Set(custom, value);
        }

        public static object GetModelItemValue(object custom, string propertyName)
        {
            string key = string.Format("{0}_{1}", custom.GetType().Name, propertyName);
            if (!dic.Contains(key))
            {
                dic.Add(key, custom.GetType().GetProperty(propertyName));
            }
            return ReflectionDelegated.GetPropertyGetterWrapper((PropertyInfo)dic[key]).Get(custom);
        }
    }
}
