using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    /// <summary>
    ///定义读属性操作的接口
    /// </summary>
    public interface IReflectValue
    {
        object Get(object target);
        void Set(object target, object val);
    }

    /// <summary>
    ///创建IGetValue或者ISetValue实例的工厂方法类
    /// </summary>
    public static class ReflectionDelegated
    {
        private static readonly Hashtable dic = Hashtable.Synchronized(new Hashtable());
        internal static IReflectValue GetPropertyGetterWrapper(PropertyInfo propertyInfo)
        {
            IReflectValue property = null;
            if (!dic.Contains(propertyInfo))
            {
                property = CreatePropertyGetterWrapper(propertyInfo);
                dic[propertyInfo] = property;
            }
            else
            {
                property = (IReflectValue)dic[propertyInfo];
            }
            return property;
        }

        internal static IReflectValue GetPropertySetterWrapper(PropertyInfo propertyInfo)
        {
            IReflectValue property = null;
            if (!dic.Contains(propertyInfo))
            {
                property = CreatePropertySetterWrapper(propertyInfo);
                dic[propertyInfo] = property;
            }
            else
            {
                property = (IReflectValue)dic[propertyInfo];
            }
            return property;
        }

        /// <summary>
        ///根据指定的PropertyInfo对象，返回对应的IGetValue实例
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static IReflectValue CreatePropertyGetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (propertyInfo.CanRead == false)
                throw new InvalidOperationException("属性不支持读操作。");

            var mi = propertyInfo.GetGetMethod(true);

            if (mi.GetParameters().Length > 0)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            if (mi.IsStatic)
            {
                var instanceType = typeof(StaticValueDetailInfo<>).MakeGenericType(propertyInfo.PropertyType);
                return (IReflectValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
            else
            {
                var instanceType = typeof(ValueDetailInfo<,>).MakeGenericType(propertyInfo.DeclaringType,
                    propertyInfo.PropertyType);
                return (IReflectValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
        }

        /// <summary>
        ///根据指定的PropertyInfo对象，返回对应的ISetValue实例
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static IReflectValue CreatePropertySetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            var mi = propertyInfo.GetSetMethod(true);

            if (mi.GetParameters().Length > 1)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            if (mi.IsStatic)
            {
                var instanceType = typeof(StaticValueDetailInfo<>).MakeGenericType(propertyInfo.PropertyType);
                return (IReflectValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
            else
            {
                var instanceType = typeof(ValueDetailInfo<,>).MakeGenericType(propertyInfo.DeclaringType,
                    propertyInfo.PropertyType);
                return (IReflectValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
        }
    }
    internal class ValueDetailInfo<TTarget, TValue> : IReflectValue
    {
        private readonly Func<TTarget, TValue> _getter;

        private readonly Action<TTarget, TValue> _setter;

        public ValueDetailInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanRead == false && propertyInfo.CanWrite == false)
                throw new InvalidOperationException("属性读写操作。");
            _getter =
                (Func<TTarget, TValue>)
                    Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, propertyInfo.GetGetMethod(true));
            _setter =
                (Action<TTarget, TValue>)
                    Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, propertyInfo.GetSetMethod(true));
        }

        object IReflectValue.Get(object target)
        {
            return _getter((TTarget)target);
        }

        void IReflectValue.Set(object target, object val)
        {
            _setter((TTarget)target, (TValue)val);
        }

        public TValue GetValue(TTarget target)
        {
            return _getter(target);
        }

        public void SetValue(TTarget target, TValue val)
        {
            _setter(target, val);
        }
    }
    internal class StaticValueDetailInfo<TValue> : IReflectValue
    {
        private readonly Func<TValue> _getter;

        private readonly Action<TValue> _setter;

        public StaticValueDetailInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (propertyInfo.CanRead == false && propertyInfo.CanWrite == false)
                throw new InvalidOperationException("属性不支持读写操作。");
            _getter = (Func<TValue>)Delegate.CreateDelegate(typeof(Func<TValue>), propertyInfo.GetGetMethod(true));
            _setter = (Action<TValue>)Delegate.CreateDelegate(typeof(Action<TValue>), propertyInfo.GetSetMethod(true));
        }

        object IReflectValue.Get(object target)
        {
            return _getter();
        }

        void IReflectValue.Set(object target, object val)
        {
            _setter((TValue)val);
        }

        public TValue GetValue()
        {
            return _getter();
        }

        public void SetValue(TValue val)
        {
            _setter(val);
        } 
    }
}
