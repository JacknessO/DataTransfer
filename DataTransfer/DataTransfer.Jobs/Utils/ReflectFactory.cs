using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    public class ReflectFactory
    {
        public static object CreateFullNameObject(string dllName, string fullName, params object[] args)
        {
            object obj = null;
            try
            {

                obj = Assembly.LoadFrom(dllName).CreateInstance(
                    fullName,
                    true,
                    BindingFlags.Default,
                    null,
                    args,
                    CultureInfo.CurrentCulture, null);

                if (obj == null)
                {
                    throw new TypeLoadException();
                }
                return obj;
            }
            catch (TypeLoadException e)
            {
                throw new TypeLoadException(string.Format("{0}.DLL \n数据提供类{1}没有找到,错误信息:{2}",
                    dllName, fullName, e.Message));
            }
        }

    }
}
