using BFES.DataAccess;
using DataTransfer.Jobs.Common;
using DataTransfer.Jobs.Config;
using SharpConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    public class ReadConfig
    {
        public List<SourceConfig> _SourceConfig = null;
        public bool IsException = false;

        int S_NO = 0;
        /// <summary>
        /// 初始化
        /// </summary>
        public ReadConfig()
        {
            try
            {
                GetParamConfig();
                _SourceConfig = new List<SourceConfig>();
                //获取本地程序路径
                string apppath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\");
                string[] FolderArray = null;
                string[] debugFiles = GetDebugNames();
                if (DataTransfer.Jobs.Base.Gloabls.IsDebug && debugFiles != null && debugFiles.Length > 0)
                {

                    FolderArray = new string[debugFiles.Length];
                    for (int i = 0; i < debugFiles.Length; i++)
                    {
                        FolderArray[i] = apppath + debugFiles[i];
                        GlobalObject.RichTextErrorLog.AppendTextByAsync(@"启动调试Job:" + debugFiles[i], System.Drawing.Color.DarkBlue);
                    }
                    debugFiles = null;
                }
                else
                {
                    FolderArray = Directory.GetDirectories(apppath);
                }

                SourceConfig config = null;
                foreach (string folder in FolderArray)
                {
                    S_NO++;
                    config = new SourceConfig();
                    Configuration configuration = Configuration.LoadFromFile(folder + @"\AppConfig.ini");
                    config.appConfig = configuration[typeof(AppConfig).Name].CreateObject<AppConfig>();
                    config.FolderName = Path.GetFileNameWithoutExtension(folder);
                    config.TableConfigList = GetTableConfigList(folder, config.appConfig);
                    _SourceConfig.Add(config);
                }
                List<CheckClass> CheckClassList = new List<CheckClass>();
                //校验
                //apppath
                CheckClass checkClass = null;
                foreach (SourceConfig item in _SourceConfig)
                {
                    foreach (TableConfig table in item.TableConfigList)
                    {
                        string ss = item.appConfig.T_DBServerIP + item.appConfig.T_DBServerName + table.T_TableName;
                        checkClass = new CheckClass();
                        checkClass.T_DBServerIP = item.appConfig.T_DBServerIP.ToUpper();
                        checkClass.T_DBServerName = item.appConfig.T_DBServerName.ToUpper();
                        checkClass.T_TableName = table.T_TableName.ToUpper();
                        checkClass.T_UserId = item.appConfig.T_DBUid;
                        checkClass.FolderName = table.FolderName;
                        checkClass.FileName = table.FileName;
                        CheckClassList.Add(checkClass);
                    }
                }
                foreach (CheckClass item in CheckClassList)
                {
                    List<CheckClass> find = CheckClassList.FindAll(
                        t => t.T_DBServerIP == item.T_DBServerIP
                          && t.T_DBServerName == item.T_DBServerName
                          && t.T_TableName == item.T_TableName
                          && t.T_UserId == item.T_UserId);
                    if (find.Count > 1)
                    {
                        string log = "";
                        foreach (CheckClass check in find)
                        {
                            log += apppath + check.FolderName + @"\" + check.FileName + ".csv\r\n";
                        }
                        log += "错误类型:数据库及IP及用户名及目标表一致！\r\n";
                        throw new Exception(log);
                    }
                }
            }
            catch (Exception ee)
            {
                IsException = true;
                GlobalObject.RichTextErrorLog.AppendTextByAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n" + ee.Message, System.Drawing.Color.Red);
                Log.WriteLine(ee.Message, GlobalObject.RunFolderName);
            }
        }

        void GetParamConfig()
        {
            DataTransfer.Jobs.Base.Gloabls.IsDebug = System.Configuration.ConfigurationManager.AppSettings["IsDebug"] == null ? false : System.Configuration.ConfigurationManager.AppSettings["IsDebug"].ToBool();
            Log.DebugLog = DataTransfer.Jobs.Base.Gloabls.IsDebug;
            var isSaveSQL = System.Configuration.ConfigurationManager.AppSettings["SaveSQL"] == null ? false : System.Configuration.ConfigurationManager.AppSettings["SaveSQL"].ToBool();
            DataBaseFactory.IsSaveLog = isSaveSQL;
            if (isSaveSQL)
            {
                DataBaseFactory.IsEnableLogEvent = isSaveSQL;
            }
            if (DataTransfer.Jobs.Base.Gloabls.IsDebug)
            {
                DataBaseFactory.IsEnableLogEvent = DataTransfer.Jobs.Base.Gloabls.IsDebug;
                GlobalObject.RichTextErrorLog.AppendTextByAsync("程序开启调试模式！！！", System.Drawing.Color.DarkOrange);
                GlobalObject.RichTextErrorLog.AppendTextByAsync(@"调试模式只运行Config\debug.ini内配置的文件夹内的Job，文件夹以逗号分隔！", System.Drawing.Color.DarkOrange);
            }
        }
        string[] GetDebugNames()
        {
            string[] names = null;
            if (DataTransfer.Jobs.Base.Gloabls.IsDebug)
            {
                string debugFile = AppDomain.CurrentDomain.BaseDirectory + @"Config\debug.ini";
                if (File.Exists(debugFile))
                {
                    using (FileStream fs = new FileStream(debugFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        StreamReader sr = new StreamReader(fs, Encoding.Default);
                        string text = sr.ReadToEnd();
                        if (text != null && !string.IsNullOrEmpty(text.Trim()))
                        {
                            GlobalObject.RichTextErrorLog.AppendTextByAsync("调试Job包含:" + text, System.Drawing.Color.DarkOrange);
                            names = text.Split(',');
                        }
                        else
                        {
                            GlobalObject.RichTextErrorLog.AppendTextByAsync(@"警告：由于Config\debug.ini文件为空！所以运行全部Job", System.Drawing.Color.DarkOrange);
                        }
                        sr.Close();
                        sr.Dispose();
                        sr = null;
                    }
                }
            }
            return names;
        }

        private List<TableConfig> GetTableConfigList(string folder, AppConfig appConfig)
        {
            string FolderName = Path.GetFileNameWithoutExtension(folder);
            try
            {
                List<TableConfig> TableConfigList = new List<TableConfig>();
                string[] ConfigPathArray = Directory.GetFiles(folder);
                foreach (string path in ConfigPathArray)
                {
                    if (path.Contains(".csv"))
                    {
                        TableConfig t = GetTableConfig(path, appConfig);
                        t.FolderName = FolderName;

                        TableConfigList.Add(t);
                    }
                }
                return TableConfigList;
            }
            catch (Exception ee)
            {
                throw new Exception("路径：" + folder + "  \r\n" + ee.Message);
            }

        }
        public TableConfig GetTableConfig(string path, AppConfig appConfig)
        {
            string FileName = Path.GetFileNameWithoutExtension(path);
            try
            {

                TableConfig tableConfig = null;
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                    {
                        if (sr != null)
                        {
                            tableConfig = ReadFileContext(sr, appConfig);
                            tableConfig.FileName = FileName;
                        }
                    }
                }
                return tableConfig;
            }
            catch (Exception ee)
            {

                throw new Exception("配置文件:" + FileName + "  \r\n" + ee.Message);
            }
        }
        private TableConfig ReadFileContext(StreamReader sr, AppConfig appConfig)
        {
            int lineNo = 0;
            try
            {
                bool IsS_ColumnField = false;
                TableConfig tableConfig = new TableConfig();
                string line;
                string[] Split;
                List<ColumnConfig> columnConfigList = new List<ColumnConfig>();
                ColumnConfig columnConfig = null;
                while ((line = sr.ReadLine()) != null)
                {
                    lineNo++;
                    Split = line.Split(',');
                    if (Split[0] == "配置类型")
                    {
                        tableConfig.JobType = (JobType)(Convert.ToInt32(Split[1]));
                        continue;
                    }
                    else if (Split[0] == "是否时间时序表")
                    {
                        tableConfig.IsTimeSeries = Convert.ToBoolean(Split[1]);
                        continue;
                    }
                    else if (Split[0] == "删除数据周期时间")
                    {
                        tableConfig.DelectCycle = Convert.ToInt32(Split[1]);
                        continue;
                    }
                    else if (Split[0] == "刷新周期")
                    {
                        tableConfig.RefreshCycle = Convert.ToInt32(Split[1]);
                        continue;
                    }
                    else if (Split[0] == "延迟时间")
                    {
                        tableConfig.DelayedTime = Convert.ToInt32(Split[1]);
                        continue;
                    }
                    else if (Split[0] == "小时周期")
                    {
                        tableConfig.HourCycle = Convert.ToInt32(Split[1]);
                        continue;
                    }
                    else if (Split[0] == "源-表名")
                    {
                        tableConfig.S_TableName = string.Format(" {0} ", Split[1].Replace(appConfig.ReplaceComma, ",").Replace("\"\"\"", "\""));
                        continue;
                    }
                    else if (Split[0] == "源-主键")
                    {
                        tableConfig.S_TablePrimaryKey = Split[1].Replace("\"\"\"", "\"");
                        continue;
                    }
                    else if (Split[0] == "源-时间时序表时间字段")
                    {
                        tableConfig.S_TableSequential = Split[1].Replace(appConfig.ReplaceComma, ",").Replace("\"\"\"", "\"");
                        continue;
                    }
                    else if (Split[0] == "源-查询过滤条件")
                    {
                        tableConfig.S_Filter = Split[1].Replace(appConfig.ReplaceComma, ",").Replace("\"\"\"", "\"");
                        continue;
                    }
                    else if (Split[0] == "源-计算1小时方式")
                    {
                        tableConfig.S_Insert01HType = Split[1];
                        continue;
                    }

                    else if (Split[0] == "目标-是否含有关联触发器")
                    {
                        tableConfig.IsExistTri = Split[1].ToUpper() == "TRUE" ? true : false;
                        continue;
                    }
                    else if (Split[0] == "目标-表名")
                    {
                        tableConfig.T_TableName = Split[1];
                        continue;
                    }
                    else if (Split[0] == "目标-主键")
                    {
                        tableConfig.T_TablePrimaryKey = Split[1];
                        continue;
                    }
                    else if (Split[0] == "目标-时间时序表时间字段")
                    {
                        tableConfig.T_TableSequential = Split[1].Replace(appConfig.ReplaceComma, ",");
                        continue;
                    }
                    else if (Split[0] == "目标-删除过滤条件")
                    {
                        tableConfig.T_DeleteFilter = Split[1];
                        continue;
                    }
                    else if (Split[0] == "插入完成后执行语句")
                    {
                        tableConfig.UpdateSql = Split[1].ToUpper().Replace(appConfig.ReplaceComma, ",");
                        continue;
                    }
                    else if (Split[0] == "DLL名称")
                    {
                        tableConfig.DllName = Split[1];
                        continue;
                    }
                    else if (Split[0] == "类名称")
                    {
                        tableConfig.ClassName = Split[1];
                        continue;
                    }



                    else if (Split[0] == "源-列字段")
                    {
                        IsS_ColumnField = true;
                        continue;
                    }
                    if (IsS_ColumnField)
                    {
                        if (Split[0] == "/")
                            break;
                        columnConfig = new ColumnConfig();
                        //替换逗号
                        columnConfig.S_DBField = Split[0].Replace(appConfig.ReplaceComma, ",").Replace("\"\"\"", "\"");

                        columnConfig.S_ColumnDataType = (ColumnDataType)Enum.Parse(typeof(ColumnDataType), Split[1].ToUpper(), true);

                        columnConfig.T_DBField = Split[2];

                        columnConfig.T_ColumnDataType = (ColumnDataType)Enum.Parse(typeof(ColumnDataType), Split[3].ToUpper(), true);
                        if (Split.Length > 4)
                            columnConfig.Describe = Split[4];

                        if (Split.Length > 5)
                        {
                            columnConfig.FilterString = Split[5].ToUpper().Trim();
                        }
                        else
                        {
                            columnConfig.FilterString = string.Format("{0}<>-9999", columnConfig.T_DBField.ToUpper());
                        }
                        columnConfigList.Add(columnConfig);
                    }
                }
                tableConfig.ColumnConfigList = columnConfigList;
                tableConfig.S_DBType = appConfig.S_DBType;
                tableConfig.T_DBType = appConfig.T_DBType;
                tableConfig.SelectSql = GetSelectSql(tableConfig);
                tableConfig.IsExistDump = columnConfigList.Exists(m => m.S_ColumnDataType == ColumnDataType.DUMP);
                if (tableConfig.ColumnConfigList.Count > 0)
                {
                    tableConfig.columnNames = new string[tableConfig.ColumnConfigList.Count];
                    for (int i = 0; i < tableConfig.ColumnConfigList.Count; i++)
                    {
                        tableConfig.columnNames[i] = tableConfig.ColumnConfigList[i].T_DBField.ToUpper();
                    }
                }
                GetDBConn(appConfig, ref tableConfig.S_DBConnstr, ref tableConfig.T_DBConnstr);
                return tableConfig;
            }
            catch (Exception ee)
            {
                throw new Exception("行号：" + lineNo.ToString() + " \r\nError:" + ee.Message);
            }
        }
        /// <summary>
        /// 拼接查询语句
        /// </summary>
        /// <param name="tableConfig"></param>
        /// <returns></returns>
        private string GetSelectSql(TableConfig tableConfig)
        {
            string S_DBField = "";
            string selectSql = "SELECT {0} FROM {1} alias ";
            string Fields = "";
            foreach (ColumnConfig item in tableConfig.ColumnConfigList)
            {
                S_DBField = item.S_DBField;
                //类型转换
                if (item.S_ColumnDataType == ColumnDataType.STRING)
                {
                    if (item.T_ColumnDataType == ColumnDataType.DATE)
                    {
                        if (tableConfig.s_DBType == DataBaseType.Oracle || tableConfig.s_DBType == DataBaseType.Oracle9i)
                        {
                            S_DBField = UtilsConvert.GetOracleStrToData(S_DBField);
                        }
                        else if (tableConfig.s_DBType == DataBaseType.MySql)
                        {
                            S_DBField = UtilsConvert.GetMySqlStrToData(S_DBField);
                        }
                        else if (tableConfig.s_DBType == DataBaseType.SqlServer)
                        {
                            S_DBField = UtilsConvert.GetSqlStrToData(S_DBField);
                        }
                    }
                }
                if (item.S_ColumnDataType == ColumnDataType.DATE)
                {
                    if (item.T_ColumnDataType == ColumnDataType.STRING)
                    {
                        if (tableConfig.s_DBType == DataBaseType.Oracle || tableConfig.s_DBType == DataBaseType.Oracle9i)
                        {
                            S_DBField = UtilsConvert.GetOracleDataToStr(S_DBField);
                        }
                        else if (tableConfig.s_DBType == DataBaseType.MySql)
                        {
                            S_DBField = UtilsConvert.GetMySqlDataToStr(S_DBField);
                        }
                        else if (tableConfig.s_DBType == DataBaseType.SqlServer)
                        {
                            S_DBField = UtilsConvert.GetSqlDataToStr(S_DBField);
                        }
                    }
                }
                Fields += S_DBField + " as " + item.T_DBField + " ,";
            }
            if (Fields.Length > 0)
                Fields = Fields.Remove(Fields.Length - 1, 1);
            return string.Format(selectSql, Fields, tableConfig.S_TableName);
        }
        /// <summary>
        /// 拼接源数据库连接字符串
        /// </summary>
        /// <param name="appConfig"></param>
        /// <returns></returns>

        private void GetDBConn(AppConfig appConfig, ref string S_DBConnstr, ref string T_DBConnstr)
        {
            #region 源连接字符串拼接
            if (appConfig.S_DBType.ToUpper() == "ORACLE" || appConfig.S_DBType.ToUpper() == "ORACLE9I")
            {
                S_DBConnstr += @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=";
                S_DBConnstr += appConfig.S_DBServerIP;
                S_DBConnstr += @")(PORT=" + appConfig.S_DBPort + "))(CONNECT_DATA =(SERVICE_NAME = " + appConfig.S_DBServerName + ")))";
                S_DBConnstr += @";Connect Timeout=60;User Id=";
                S_DBConnstr += appConfig.S_DBUid;
                S_DBConnstr += @";Password=";
                S_DBConnstr += appConfig.S_DBPwd;

            }
            else if (appConfig.S_DBType.ToUpper() == "SQLSERVER")
            {
                S_DBConnstr = string.Format(@"server ={0}; Database ={1}; Integrated Security = False;Connect Timeout=60; Pooling = True; user ={2}; pwd ={3}", appConfig.S_DBServerIP, appConfig.S_DBServerName, appConfig.S_DBUid, appConfig.S_DBPwd);
            }
            else if (appConfig.S_DBType.ToUpper() == "MYSQL")
            {
                //Server=Server; Port=1234; Database=Test; Uid=UserName; Pwd=asdasd; 
                S_DBConnstr = string.Format(@"Server ={0}; Database ={1}; Port ={2};Uid ={3}; Pwd={4}; "
                     , appConfig.S_DBServerIP, appConfig.S_DBServerName, appConfig.S_DBPort, appConfig.S_DBUid, appConfig.S_DBPwd);
            }
            #endregion

            #region 目标连接字符串拼接
            if (appConfig.T_DBType.ToUpper() == "ORACLE" || appConfig.S_DBType.ToUpper() == "ORACLE9I")
            {
                T_DBConnstr += @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=";
                T_DBConnstr += appConfig.T_DBServerIP;
                T_DBConnstr += @")(PORT=" + appConfig.T_DBPort + "))(CONNECT_DATA =(SERVICE_NAME = " + appConfig.T_DBServerName + ")))";
                T_DBConnstr += @";Connect Timeout=60;User Id=";
                T_DBConnstr += appConfig.T_DBUid;
                T_DBConnstr += @";Password=";
                T_DBConnstr += appConfig.T_DBPwd;

            }
            else if (appConfig.T_DBType.ToUpper() == "SQLSERVER")
            {
                T_DBConnstr = string.Format(@"server ={0}; Database ={1}; Integrated Security = False;Connect Timeout=60; Pooling = True; user ={2}; pwd ={3}", appConfig.T_DBServerIP, appConfig.T_DBServerName, appConfig.T_DBUid, appConfig.T_DBPwd);
            }
            else if (appConfig.T_DBType.ToUpper() == "MYSQL")
            {
                T_DBConnstr = string.Format(@"Server ={0}; Database ={1}; Port ={2};Uid ={3}; Pwd={4}; "
                     , appConfig.T_DBServerIP, appConfig.T_DBServerName, appConfig.T_DBPort, appConfig.T_DBUid, appConfig.T_DBPwd);

            }
            #endregion
        }

        public Dictionary<string, TableConfig> GetDictionary()
        {
            Dictionary<string, TableConfig> tableConfigDic = new Dictionary<string, TableConfig>();
            foreach (SourceConfig item in _SourceConfig)
            {
                foreach (TableConfig table in item.TableConfigList)
                {
                    tableConfigDic.Add(item.FolderName + "." + table.FileName, table);
                }
            }
            return tableConfigDic;
        }

    }
    public class CheckClass
    {
        public string T_DBServerIP { get; set; }
        public string T_DBServerName { get; set; }
        public string T_TableName { get; set; }
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string T_UserId { get; set; }
    }
}
namespace DataTransfer.Jobs.Base
{
    public class Gloabls
    {
        private static bool isDebug = false;
        /// <summary>
        /// 是否调试模式
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                return isDebug;
            }

            set
            {
                isDebug = value;
            }
        }
    }
}
