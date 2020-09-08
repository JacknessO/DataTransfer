using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Config
{
    /// <summary>
    /// 源通用配置
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// #配置类型(暂时只有1库对库类型)
        /// </summary>
        public int Config_TYPE { get; set; }
        /// <summary>
        /// #源-数据库IP
        /// </summary>
        public string S_DBServerIP { get; set; }
        /// <summary>
        /// #源-数据库服务名称
        /// </summary>
        public string S_DBServerName { get; set; }
        /// <summary>
        /// #源-数据库用户
        /// </summary>
        public string S_DBUid { get; set; }
        /// <summary>
        /// #源-数据库密码
        /// </summary>
        public string S_DBPwd { get; set; }
        /// <summary>
        /// #源-数据库端口
        /// </summary>
        public string S_DBPort { get; set; }
        /// <summary>
        /// #源-数据库类型
        /// </summary>
        public string S_DBType { get; set; }

        /// <summary>
        /// #目标-数据库IP
        /// </summary>
        public string T_DBServerIP { get; set; }
        /// <summary>
        /// #目标-数据库服务名称
        /// </summary>
        public string T_DBServerName { get; set; }
        /// <summary>
        /// #目标-数据库用户
        /// </summary>
        public string T_DBUid { get; set; }
        /// <summary>
        /// #目标-数据库密码
        /// </summary>
        public string T_DBPwd { get; set; }
        /// <summary>
        /// #目标-数据库端口
        /// </summary>
        public string T_DBPort { get; set; }
        /// <summary>
        /// #目标-数据库类型
        /// </summary>
        public string T_DBType { get; set; }
        /// <summary>
        /// #逗号(,)替代符号
        /// </summary>
        public string ReplaceComma { get; set; }
        /// <summary>
        /// #开始时间替代字符
        /// </summary>
        public string ReplaceStartTime { get; set; }
        /// <summary>
        /// #结束时间替代字符
        /// </summary>
        public string ReplaceEndTime { get; set; }
        /// <summary>
        /// #当前时间替代字符
        /// </summary>
        public string ReplaceRealTime { get; set; }
    }
}
