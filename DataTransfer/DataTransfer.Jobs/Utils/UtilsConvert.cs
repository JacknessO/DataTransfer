using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    public class UtilsConvert
    {
        /// <summary>
        /// 转换Oracle时间
        /// to_date('{0}','yyyy-mm-dd hh24:mi:ss')
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetOraString(DateTime time)
        {
            return string.Format("to_date('{0}','yyyy-mm-dd hh24:mi:ss')", time.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 转换Oracle时间
        /// to_date('{0}','yyyy-mm-dd hh24:mi:ss')
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetOraString(string time)
        {
            return string.Format("to_date('{0}','yyyy-mm-dd hh24:mi:ss')", time);
        }

        /// <summary>
        /// 转换MySql时间
        /// str_to_date('{0}','%Y-%m-%d %H:%I:%S')
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetMySqlString(DateTime time)
        {
            return string.Format("str_to_date('{0}','%Y-%m-%d %H:%I:%S')", time.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        /// <summary>
        ///转换MySql时间
        ///str_to_date('{0}','%Y-%m-%d %H:%I:%S')
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetMySqlString(string time)
        {
            return string.Format("str_to_date('{0}','%Y-%m-%d %H:%I:%S')", time);
        }
        public static string GetOracleStrToData(string Field)
        {
            return string.Format("to_date({0},'yyyy-mm-dd hh24:mi:ss')", Field);
        }
        public static string GetOracleDataToStr(string Field)
        {
            return string.Format("to_char({0},'yyyy-mm-dd hh24:mi:ss')", Field);
        }
        public static string GetSqlStrToData(string Field)
        {
            return string.Format("CONVERT(datetime,{0})", Field);
        }
        public static string GetSqlDataToStr(string Field)
        {
            //convert(datetime,列名)
            return string.Format("CONVERT(varchar(100),{0},20)", Field);
        }
        public static string GetMySqlStrToData(string Field)
        {
            return string.Format("str_to_date({0},'%Y-%m-%d %H:%I:%S')", Field);
        }
        public static string GetMySqlDataToStr(string Field)
        { //date_format
            return string.Format("date_format({0},'%Y-%m-%d %H:%I:%S')", Field);
        }
        /// <summary>
        /// dump转字符串
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string DumpToString(object d)
        {
            if (d == null || string.IsNullOrEmpty(d.ToString()) || d.ToString() == "NULL")
            {
                return "";
            }
            string s = d.ToString().Substring(d.ToString().IndexOf("Len=") + 4);
            int l = Convert.ToInt32(s.Substring(0, s.IndexOf(": ")));
            byte[] b = new byte[l];
            s = s.Substring(s.IndexOf(": ") + 2, s.Length - s.IndexOf(": ") - 2);
            for (int m = 0; m < l - 1; m++)
            {
                b[m] = Convert.ToByte(s.Substring(0, s.IndexOf(",")), 16);
                s = s.Substring(s.IndexOf(",") + 1, s.Length - s.IndexOf(",") - 1);
            }
            b[l - 1] = Convert.ToByte(s, 16);
            return Encoding.GetEncoding(936).GetString(b);
        } 
    }
}
