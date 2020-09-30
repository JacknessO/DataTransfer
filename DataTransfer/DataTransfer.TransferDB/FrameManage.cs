using DataTransfer.Jobs.Config;
using DataTransfer.Jobs.Job;
using DataTransfer.Jobs.Utils;
using DataTransfer.Jobs.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DataTransfer.TransferDB
{
    internal class FrameManage
    {
        Timer mainTimer = null;
        /// <summary>
        /// 异步线程队列
        /// </summary>
        List<Task> taskCollection = new List<Task>();
        /// <summary>
        /// 作业集合
        /// </summary>
        List<BaseJob> taskModelList = new List<BaseJob>();

        private static FrameManage instance = null;

        /// <summary>
        /// 唯一实例
        /// </summary>
        internal static FrameManage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FrameManage();
                }
                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        internal FrameManage()
        {
        }
        /// <summary>
        /// 初始化
        /// 包括 定时器的初始化 作业的初始化 配置的初始化
        /// </summary>
        internal void Init()
        {
            #region 清空
            if (mainTimer != null)
            {
                mainTimer.Stop();
                mainTimer.Elapsed -= MainTimer_Elapsed;
                mainTimer.Dispose();
                mainTimer = null;
            }
            mainTimer = new Timer
            {
                Interval = 1000,
                Enabled = false,
            };

            if (taskModelList != null)
            {
                foreach (BaseJob item in taskModelList)
                {
                    item.Dispose();
                }
            }
            if (taskCollection != null)
            {
                foreach (Task item in taskCollection)
                {
                    item.Dispose();
                }
            }
            #endregion

            ReadConfig readConfig = new ReadConfig();
            //判断是否有异常
            if (readConfig.IsException)
            {
                return;
            }
            GlobalObject.RichTextLog.AppendTextByAsync("配置加载完成！", Color.Black);
            BaseJob model = null;
            //根据配置数量进行初始化作业
            foreach (SourceConfig item in readConfig._SourceConfig)
            {
                foreach (TableConfig tableConfig in item.TableConfigList)
                {
                    model = JobFactory.CreateJob(tableConfig);
                    model.InitTask();
                    taskModelList.Add(model);
                }
            }

            GlobalObject.RichTextLog.AppendTextByAsync("任务初始化完成！", Color.Black);

            mainTimer.Elapsed += MainTimer_Elapsed;
        }
        /// <summary>
        /// 定时器触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            int currentNum = currentTime.Hour * 3600 + currentTime.Minute * 60 + currentTime.Second;
            int count = 0;
            Task oneTask = null;
            if (taskModelList.Exists(m => (m.IsTaskBusy == false && (currentNum - m.Task_DelayedTime) % m.Task_Fre == 0)))
            {
                List<BaseJob> filterJobs = taskModelList.FindAll(m => (m.IsTaskBusy == false && (currentNum - m.Task_DelayedTime) % m.Task_Fre == 0));
                foreach (var waitingItem in filterJobs)
                {
                    waitingItem.IsTaskBusy = true;
                    oneTask = CreatTask(waitingItem, waitingItem.Task_DelayedTime == 0 ? currentTime : currentTime.AddSeconds(0 - waitingItem.Task_DelayedTime));
                    taskCollection.Add(oneTask);
                    oneTask.Start();
                    count++;
                }
            }
            //移除和释放
            var l = taskCollection.FindAll(item => item.Status == TaskStatus.Faulted || item.Status == TaskStatus.Canceled || item.Status == TaskStatus.RanToCompletion);
            if (l.Count > 0)
            {
                taskCollection.RemoveAll(item => item.Status == TaskStatus.Faulted || item.Status == TaskStatus.Canceled || item.Status == TaskStatus.RanToCompletion);
                foreach (var item in l)
                {
                    if (item.Status == TaskStatus.Faulted || item.Status == TaskStatus.Canceled || item.Status == TaskStatus.RanToCompletion)
                    {
                        item.Dispose();
                    }
                }
                l.Clear();
                l = null;
            }
            //正在运行的个数
            int num = taskCollection.Count;
            //做委托
            GlobalObject.RealDisplay.TextByAsync("运行状态：正在运行中的个数为" + num + "个");
        }
        static Task CreatTask(BaseJob tmodel, DateTime currentTime)
        {
            var task1 = new Task(() =>
            {
                string slog = "任务进程(" + currentTime.ToString("yyyy-MM-dd :HH:mm:ss") + ")：" + tmodel.tableConfig.FolderName + "->" + tmodel.tableConfig.FileName;
                if (tmodel.LastDateTime != DateTime.MinValue && Convert.ToDateTime(currentTime.ToString("yyyy-MM-dd HH:mm:ss")) < Convert.ToDateTime((tmodel.LastDateTime.AddSeconds(tmodel.Task_Fre)).ToString("yyyy-MM-dd HH:mm:ss")))
                {
                    //   GlobalObject.RichTextLog.AppendTextByAsync(string.Format("{0},上次时间戳{1},本次时间戳{2}", slog, tmodel.LastDateTime, currentTime), Color.Brown);
                    tmodel.IsTaskBusy = false;
                    return;
                }
                DateTime startWatch = DateTime.Now;
                try
                {
                    string delayedString = null;
                    if (tmodel.Task_DelayedTime != 0)
                    {
                        delayedString = string.Format("延迟执行时间为: {0} ", tmodel.Task_DelayedTime);
                    }
                    GlobalObject.RichTextLog.AppendTextByAsync(slog + " 启动! " + delayedString, Color.Blue);
                    //Log.WriteLine(slog + "启动!", GlobalObject.RunFolderName);
                    tmodel.RunTask(currentTime);
                }
                catch (Exception ex)
                {
                    tmodel.RunTaskException(currentTime, ex);
                }
                finally
                {
                    tmodel.LastDateTime = currentTime;
                    TimeSpan ts = DateTime.Now - startWatch;
                    slog += string.Format(" 耗时{0}ms 结束!\n", ts.TotalMilliseconds);
                    GlobalObject.RichTextLog.AppendTextByAsync(slog, Color.Blue);
                    //Log.WriteLine(slog, GlobalObject.RunFolderName);
                    tmodel.IsTaskBusy = false;
                }
            });
            return task1;
        }
        internal void Start()
        {
            if (mainTimer != null)
            {
                mainTimer.Enabled = true;
                mainTimer.Start();
            }
        }

        internal void Stop()
        {
            if (mainTimer != null)
            {
                mainTimer.Enabled = false;
                mainTimer.Stop();
            }
        }
    }
}
