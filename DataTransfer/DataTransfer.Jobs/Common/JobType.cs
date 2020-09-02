using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Common
{
    public enum JobType
    {
        /// <summary>
        /// 整表复制
        /// </summary>
        FullCopy = 1,
        /// <summary>
        /// 过滤插入
        /// </summary>
        FilterInsert = 3,
        /// <summary>
        /// 删除之后过滤插入
        /// </summary>
        FilterInsertByDel = 2,
        /// <summary>
        /// 最大时间插入
        /// </summary>
        MaxDataInsert = 4,
        /// <summary>
        /// 自定义插入
        /// </summary>
        CustomJob = 99
    }
}
