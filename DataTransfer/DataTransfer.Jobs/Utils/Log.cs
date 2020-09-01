using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    /// <summary>
    ///文本日志记录辅助类
    /// </summary>
    public class Log
    {
        private static readonly string LogFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\");
        public static bool RecordLog = true;
        public static bool DebugLog = false;
        private static readonly object obj = new object();
        static Log()
        {
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
        }

        public static void WriteDebug(string message)
        {
            var temp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]\r\n\r\n") + message + "\r\n\r\n";
            try
            {
                if (DebugLog)
                {
                    Console.WriteLine("日志输出：" + temp);
                }
            }
            catch
            {
            }
        }

        public static void WriteLine(string message, string directoryName = "Log")
        {
            lock (obj)
            {
                var temp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]\r\n\r\n") + message + "\r\n\r\n";
                var fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                var lagPath = Path.Combine(LogFolder, directoryName);
                try
                {
                    if (RecordLog)
                    {
                        if (!Directory.Exists(lagPath))
                        {
                            Directory.CreateDirectory(lagPath);
                        }
                        File.AppendAllText(Path.Combine(lagPath, fileName), temp, Encoding.Default);
                    }
                    if (DebugLog)
                    {
                        Console.WriteLine("错误日志输出：" + temp);
                    }
                }
                catch
                {
                }
            }
        }

        public static void WriteLine(string className, string funName, string message, string directoryName = "Log")
        {
            WriteLine(string.Format("{0}：{1}\r\n{2}", className, funName, message), directoryName);
        }
    }
}
