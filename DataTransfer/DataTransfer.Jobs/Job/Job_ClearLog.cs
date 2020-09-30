using BFES.DataAccess;
using DataTransfer.Jobs.Models;
using DataTransfer.Jobs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Job
{
    /// <summary>
    /// 定期清理日志
    /// </summary>
    public class Job_ClearLog : BaseJob
    {

        public override void InitTask()
        {
        }

        public override void RunTask(DateTime currentTime)
        {
            try
            {
                List<ConfigClearLog> ConfigClearLog_List = new List<ConfigClearLog>();
                bool IsExistConfig = false;
                string Selectsql = "select count(*) from user_tables where table_name = 'CONFIGCLEARLOG'";
                //1.查询配置
                using (IDataBase iDataBase = DalFactory.GreateIDataBase(S_DBSource))
                {
                    if (iDataBase.GetInt(Selectsql) == 1)
                    {
                        IsExistConfig = true;
                        ConfigClearLog_List = iDataBase.GetList<ConfigClearLog>("select * from CONFIGCLEARLOG");
                    }
                    else
                        Outputlog("没有表CONFIGCLEARLOG");
                }
                if (IsExistConfig)
                {
                    //2.清理文件
                    if (ConfigClearLog_List.Count > 0)
                    {
                        foreach (var item in ConfigClearLog_List)
                        {
                            List<FileInfo> FileInfo_List = Utils.FileHelp.GetAllFilesInDirectory(item.LOGLOCATION);
                            List<FileInfo> delectFile = FileInfo_List.FindAll(m => (currentTime - m.CreationTime).TotalHours > item.SAVETIME);
                            foreach (var delectitem in delectFile)
                            {
                                File.Delete(delectitem.FullName);
                            }
                        }
                        Outputlog("日志清理完成！");
                    }
                    else
                        Outputlog("未配置表CONFIGCLEARLOG");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {

            }

        }

        public override void RunTaskException(DateTime currentTime, Exception exception)
        {
            Utils.GlobalObject.RichTextErrorLog.AppendTextByAsync("任务进程(" + currentTime.ToString("yyyy-MM-dd :HH:mm:ss") + ")：" + tableConfig.FolderName + "->" + tableConfig.FileName + "\r\n " + exception.Message, System.Drawing.Color.Red);
            Writelog(exception.Message + exception.StackTrace);
        }
    }
}
