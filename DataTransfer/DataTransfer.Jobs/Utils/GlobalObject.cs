using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataTransfer.Jobs.Utils
{
    public class GlobalObject
    {
        public static string RunFolderName = ConfigurationManager.AppSettings["RunFolderName"];
        /// <summary>
        /// 日志输出
        /// </summary>
        public static RichTextBox RichTextLog { get; set; }
        /// <summary>
        /// 错误输出
        /// </summary>
        public static RichTextBox RichTextErrorLog { get; set; }

        /// <summary>
        /// 线程数量的实时显示
        /// </summary>
        public static Label RealDisplay { get; set; }
    }
}
