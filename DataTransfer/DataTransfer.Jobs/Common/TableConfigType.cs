using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Common
{
    public enum TableConfigType
    {
        /// <summary>
        /// 删除 所有数据
        /// </summary>
        DelectAll = 1,
        /// <summary>
        /// 删除 时间周期数据
        /// </summary>
        DelectTime = 2,
        /// <summary>
        /// 插入 最新数据
        /// </summary>
        InsertNow = 3
    }
}
