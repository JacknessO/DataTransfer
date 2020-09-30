using DataTransfer.Jobs.Config;
using DataTransfer.Jobs.Job;
using DataTransfer.Jobs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Common
{
    public class JobFactory
    {
        public static BaseJob CreateJob(TableConfig item)
        {
            BaseJob baseJob = null;
            if (item == null)
            {
                throw new Exception("配置对象为空！");
            }
            if (item.RefreshCycle == 0)
            {
                throw new Exception("配置刷新频率不能为零！");
            }
            if (item.RefreshCycle == 0)
            {
                throw new Exception("配置刷新频率不能为零！");
            }
            if (string.IsNullOrEmpty(item.S_DBConnstr) || string.IsNullOrEmpty(item.S_DBConnstr))
            {
                throw new Exception("连接字符串不能为空！");
            }
            if (string.IsNullOrEmpty(item.SelectSql))
            {
                throw new Exception("表字段配置错误！");
            }
            switch (item.JobType)
            {
                case JobType.FullCopy:
                    //baseJob = new Plugin.Job_FullCopy();
                    break;
                case JobType.FilterInsert:
                    //baseJob = new Plugin.Job_FilterInsert();
                    break;
                case JobType.FilterInsertByDel:
                    //baseJob = new Plugin.Job_FilterInsertByDel();
                    break;
                ///定制的需要反射出来
                case JobType.CustomJob:
                    baseJob = (BaseJob)ReflectFactory.CreateFullNameObject(item.DllName, item.ClassName);
                    break;
                default:
                    throw new Exception("未知类型的任务");
            }
            baseJob.Task_Fre = item.RefreshCycle;
            baseJob.Task_DelayedTime = item.DelayedTime;
            baseJob.S_DBSource = new DatabaseSource { Connstr = item.S_DBConnstr, DBType = item.s_DBType };
            baseJob.T_DBSource = new DatabaseSource { Connstr = item.T_DBConnstr, DBType = item.t_DBType };
            baseJob.SelectSql = item.SelectSql;
            baseJob.Columns = item.columnNames;
            baseJob.tableConfig = item;
            return baseJob;
        }
    }
}
