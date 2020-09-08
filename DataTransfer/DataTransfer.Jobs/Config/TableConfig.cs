using BFES.DataAccess;
using DataTransfer.Jobs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Config
{
    public class TableConfig
    {
        public string FolderName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 配置类型 默认插入新数据  
        /// </summary>
        public JobType JobType = JobType.FilterInsert;
        /// <summary>
        /// 是否为时间时序表 默认为True
        /// </summary>
        public bool IsTimeSeries = true;
        /// <summary>
        /// 删除数据周期时间
        /// </summary>
        public int DelectCycle { get; set; }
        /// <summary>
        /// 巡检周期
        /// </summary>
        public int RefreshCycle { get; set; }
        /// <summary>
        /// 延迟时间
        /// </summary>
        public int DelayedTime { get; set; }
        /// <summary>
        /// 类似BFES.Design 
        /// 可为exe
        /// </summary>
        public string DllName { get; set; }
        /// <summary>
        /// 类似BFES.Design.UserControl.index	
        /// 含命名空间
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 源-表名
        /// </summary>
        public string S_TableName { set; get; }
        /// <summary>
        /// 源-主键
        /// </summary>
        public string S_TablePrimaryKey = "TIMESTAMP";
        /// <summary>
        /// 源-时间时序表时间字段
        /// </summary>
        public string S_TableSequential = "TIMESTAMP";
        /// <summary>
        /// #源-数据库类型
        /// </summary>
        public string S_DBType { get; set; }
        /// <summary>
        /// 源-查询过滤条件
        /// </summary>
        public string S_Filter { get; set; }
        /// <summary>
        /// 源-计算1小时方式
        /// </summary>
        public string S_Insert01HType = "avg";

        /// <summary>
        /// 目标-表名
        /// </summary>
        public string T_TableName { get; set; }
        /// <summary>
        /// 目标-主键
        /// </summary>
        public string T_TablePrimaryKey = "TIMESTAMP";
        /// <summary>
        /// 目标-时间时序表时间字段
        /// </summary>
        public string T_TableSequential = "TIMESTAMP";
        /// <summary>
        /// #目标-数据库类型
        /// </summary>
        public string T_DBType { get; set; }
        /// <summary>
        /// 目标-删除过滤条件
        /// </summary>
        public string T_DeleteFilter { get; set; }

        public DataBaseType t_DBType
        {
            get
            {
                switch (T_DBType.ToUpper())
                {
                    case "ORACLE":
                        return DataBaseType.Oracle;
                    case "SQLSERVER":
                        return DataBaseType.SqlServer;
                    case "MYSQL":
                        return DataBaseType.MySql;
                    case "ORACLE9I":
                        return DataBaseType.Oracle9i;
                    default:
                        return DataBaseType.Oracle;
                }
            }
        }
        public DataBaseType s_DBType
        {
            get
            {
                switch (S_DBType.ToUpper())
                {
                    case "ORACLE":
                        return DataBaseType.Oracle;
                    case "SQLSERVER":
                        return DataBaseType.SqlServer;
                    case "MYSQL":
                        return DataBaseType.MySql;
                    case "ORACLE9I":
                        return DataBaseType.Oracle9i;
                    default:
                        return DataBaseType.Oracle;
                }
            }
        }

        private bool isExistDump = false;
        /// <summary>
        /// 是否存在Dump转换
        /// </summary>
        public bool IsExistDump
        {
            get
            {
                return isExistDump;
            }

            set
            {
                isExistDump = value;
            }
        }
        private bool isExistTri = false;
        /// <summary>
        /// 是否含有关联触发器
        /// </summary>
        public bool IsExistTri
        {
            get
            {
                return isExistTri;
            }

            set
            {
                isExistTri = value;
            }
        }
        public string UpdateSql { get; set; }

        private int hourCycle = 1;
        /// <summary>
        /// 小时周期(用于计算平均值，默认1小时平均值)
        /// </summary>
        public int HourCycle
        {
            get
            {
                return hourCycle;
            }

            set
            {
                hourCycle = value;
                if (hourCycle <= 0)
                {
                    hourCycle = 1;
                }
            }
        }
        /// <summary>
        /// 查询语句
        /// </summary>
        public string SelectSql = "";
        /// <summary>
        /// 插入列名数组
        /// </summary>
        public string[] columnNames;

        /// <summary>
        /// 源数据连接字符串
        /// </summary>
        public string S_DBConnstr = "";
        /// <summary>
        /// 目标数据连接字符串
        /// </summary>
        public string T_DBConnstr = "";
        /// <summary>
        /// 列 集合
        /// </summary>
        public List<ColumnConfig> ColumnConfigList = new List<ColumnConfig>();
    }
}
