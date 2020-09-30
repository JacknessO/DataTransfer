using DataTransfer.Jobs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataTransfer.Form
{
    static class Program
    {
        private static System.Threading.Mutex mutex;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mutex = new System.Threading.Mutex(true);
            if (mutex.WaitOne(0, false)) 
            {
                //处理未捕获的异常   
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常   
                Application.ThreadException += Application_ThreadException;

               // Application.Run(new TransferDBMainForm());
            }

           
        }
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var str = "";
            var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
            var error = e.Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                    error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }
            Log.WriteLine(str, @"未处理异常\线程中未处理的异常");
            if (((e.Exception).GetType()).BaseType.Name.ToUpper() == "DBEXCEPTION")
            {

                MessageBox.Show("<b><color=red>客户端与服务器通信时出现问题，网络中断！！！</color></b>\n具体原因：<color=blue>" + e.Exception.Message + "</color>", "数据库通信错误");
            }
            else
            {
                MessageBox.Show(str, "系统错误");
            }
        }
    }
}
