using DataTransfer.Jobs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Config
{
    public class ColumnConfig
    {
        /// <summary>
        /// 源-列字段
        /// </summary>
        public string S_DBField { get; set; }
        /// <summary>
        /// 源-列数据类型(date number string) 
        /// </summary>
        public ColumnDataType S_ColumnDataType = ColumnDataType.NUMBER;
        /// <summary>
        /// 目标-列字段
        /// </summary>
        public string T_DBField { get; set; }
        /// <summary>
        /// 目标-列数据类型(date number string) 
        /// </summary>
        public ColumnDataType T_ColumnDataType = ColumnDataType.NUMBER;
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        //// 列值过滤得字符串表达式(类似 <*VAL*>>0 and <*VAL*><1000 )
        /// </summary>
        public string FilterString { get; set; }
    }
}
