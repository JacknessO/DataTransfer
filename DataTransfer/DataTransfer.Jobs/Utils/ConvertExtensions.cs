using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    public static class ConvertExtensions
    {
        /// <summary>
        /// 强转成int,如果失败返回0
        /// </summary>
        public static int ToInt(this object thisValue)
        {
            int reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(),out reval))
            {
                return reval;
            }
            return reval;
        } 
        /// <summary>
        /// 强转成double,如果失败返回0
        /// </summary> 
        public static double ToDouble(this object thisValue)
        {
            double reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        } 
        /// <summary>
        /// 强转成float,如果失败返回0
        /// </summary> 
        public static float ToFloat(this object thisValue)
        {
            float reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && float.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        } 
        /// <summary>
        /// 强转成Decimal,如果失败返回0
        /// </summary> 
        public static Decimal ToDecimal(this object thisValue)
        {
            Decimal reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && Decimal.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }
        /// <summary>
        /// 强转成DateTime,如果失败返回DateTime.MinValue
        /// </summary>
        public static DateTime ToDateTime(this object thisValue)
        {
            DateTime dt = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out dt))
            {
                return dt;
            }
            return dt;
        }
        /// <summary>
        /// 强转成bool,如果失败返回false
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool ToBool(this object thisValue)
        {
            bool b = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out b))
            {
                return b;
            }
            return b;
        } 
    }
}
