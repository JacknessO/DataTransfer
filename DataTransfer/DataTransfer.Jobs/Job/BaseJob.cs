using BFES.DataAccess;
using DataTransfer.Jobs.Common;
using DataTransfer.Jobs.Config;
using DataTransfer.Jobs.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Job
{
    public abstract class BaseJob : Interface.ITransferDBTask, IDisposable
    {
        public TableConfig tableConfig { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int Task_ID { get; set; }
        /// <summary>
        /// 任务执行频率
        /// </summary>
        public int Task_Fre { get; set; }

        /// <summary>
        /// 任务延迟时间
        /// </summary>
        public int Task_DelayedTime { get; set; }
        /// <summary>
        /// 分组标识（刷新频率+延迟时间）
        /// </summary>
        public int TaskGroupFlag
        {
            get
            {

                return Task_Fre + Task_DelayedTime;
            }

        }

        private bool isTaskBusy = false;
        /// <summary>
        /// 任务是否在执行
        /// </summary>
        public bool IsTaskBusy
        {
            get
            {
                return isTaskBusy;
            }

            set
            {
                isTaskBusy = value;
            }
        }
        private DateTime lastDateTime = DateTime.MinValue;
        /// <summary>
        /// 上次执行时间戳
        /// </summary>
        public DateTime LastDateTime
        {
            get
            {
                return lastDateTime;
            }

            set
            {
                lastDateTime = value;
            }
        }
        public DatabaseSource S_DBSource { get; set; }
        public DatabaseSource T_DBSource { get; set; }
        public string SelectSql { get; set; }

        public string[] Columns { get; set; }

        /// <summary>
        /// 任务初始化
        /// </summary>
        public abstract void InitTask();
        /// <summary>
        /// 处理任务
        /// </summary>
        /// <param name="currentTime">执行任务的时间戳</param>
        public abstract void RunTask(DateTime currentTime);
        /// <summary>
        /// 任务异常
        /// </summary>
        /// <param name="currentTime">执行任务的时间戳</param>
        /// <param name="exception">截获的异常</param>
        public virtual void RunTaskException(DateTime currentTime, Exception exception)
        {
            //输入错误到界面
            GlobalObject.RichTextErrorLog.AppendTextByAsync("时间:" + currentTime.ToString("yyyy-MM-dd HH:mm:ss") + " 任务进程：" + tableConfig.FolderName + "->" + tableConfig.FileName + "\r\n " + exception.Message, System.Drawing.Color.Red);
            //输出错误到日志
            Writelog(exception.Message + exception.StackTrace);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {

        }
        /// <summary>
        /// 主线程报错日志 
        /// </summary>
        /// <param name="log"></param>
        public void Outputlog(string log)
        {
            Utils.GlobalObject.RichTextLog.AppendTextByAsync("任务进程(" + DateTime
                .Now.ToString("yyyy-MM-dd :HH:mm:ss") + ")：" + tableConfig.FolderName + "->" + tableConfig.FileName + "\r\n" + log, System.Drawing.Color.Green);
            //Log.WriteLine(log, GlobalObject.RunFolderName);
        }
        /// <summary>
        /// 线程报错日志
        /// </summary>
        /// <param name="log"></param>
        public void Writelog(string log)
        {
            Log.WriteLine(log, tableConfig.FolderName + @"\" + tableConfig.FileName + @"\");
        }

        public void UpdateFlag(IDataBase iDataBase, bool issuccess, string sql, ref string log)
        {
            if (issuccess && !string.IsNullOrEmpty(sql))
            {
                try
                {
                    iDataBase.ExecuteCommand(sql);
                    log += string.Format("{0}:更新标识完成！\r\n", tableConfig.T_TableName);
                }
                catch
                {
                    log += string.Format("{0}:更新标识错误！\r\n", tableConfig.T_TableName);
                    throw new Exception(string.Format("{0}:更新标识错误！{1}\r\n", tableConfig.T_TableName, sql));
                }
            }
        }
        public DataTable RemoveDump(DataTable dt)
        {
            try
            {
                if (tableConfig.IsExistDump && dt != null && dt.Rows.Count > 0)
                {
                    foreach (ColumnConfig column in tableConfig.ColumnConfigList)
                    {
                        if (column.S_ColumnDataType == ColumnDataType.DUMP)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dt.Rows[i][column.T_DBField.ToUpper()] = Utils.UtilsConvert.DumpToString(dt.Rows[i][column.T_DBField.ToUpper()]);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("*&*&{0}Dump转换错误", tableConfig.S_TableName));
            }
            return dt;
        }
    }
}
public class DatabaseSource
{
    public string Connstr { get; set; }
    public DataBaseType DBType;
}

