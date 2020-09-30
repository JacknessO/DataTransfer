using DataTransfer.Jobs.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataTransfer.TransferDB
{
    public partial class TransferDBMainForm : Form
    {
        private ContextMenu notifyiconMnu;
        public TransferDBMainForm()
        {
            InitializeComponent();
            Load += TransferDBMainForm_Load;
            Disposed += TransferDBMainForm_Disposed;
        }

        private void TransferDBMainForm_Disposed(object sender, EventArgs e)
        {
            GCTool.GCDispose();
        }

        private void TransferDBMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true; //取消关闭窗体事件
                NotifyIcon1.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Hide();
                this.ShowInTaskbar = false;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void TransferDBMainForm_Load(object sender, EventArgs e)
        {
            NotifyIcon1.Text = this.Text;
            Init();
            GCTool.GCStart();
        }
        private void TransferDBMainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘
            {
                NotifyIcon1.Visible = true;    //显示托盘图标
                this.WindowState = FormWindowState.Normal;
                this.Hide();
                ShowInTaskbar = false;

            }
        }

        #region 最小化到托盘
        private void Init()
        {

            NotifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            FormClosing += TransferDBMainForm_FormClosing;
            this.Resize += new System.EventHandler(this.TransferDBMainForm_Resize);
            GlobalObject.RichTextLog = richTextLog;
            GlobalObject.RichTextErrorLog = richTextErrorLog;
            GlobalObject.RealDisplay = label1;
            FrameManage.Instance.Init();
            FrameManage.Instance.Start();

            //定义一个MenuItem数组，并把此数组同时赋值给ContextMenu对象 
            MenuItem[] mnuItms = new MenuItem[3];
            mnuItms[0] = new MenuItem();
            mnuItms[0].Text = "显示窗口";
            mnuItms[0].Click += new System.EventHandler(notifyIcon1_showfrom);

            mnuItms[1] = new MenuItem("-");

            mnuItms[2] = new MenuItem();
            mnuItms[2].Text = "退出系统";
            mnuItms[2].Click += new System.EventHandler(this.ExitSelect);
            mnuItms[2].DefaultItem = true;

            notifyiconMnu = new ContextMenu(mnuItms);
            NotifyIcon1.ContextMenu = notifyiconMnu;
            //为托盘程序加入设定好的ContextMenu对象 
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            ShowInTaskbar = true;
            //WindowState = FormWindowState.Normal;
            NotifyIcon1.Visible = false;
        }
        public void notifyIcon1_showfrom(object sender, System.EventArgs e)
        {
            Show();
            ShowInTaskbar = true;
            //WindowState = FormWindowState.Normal;
            NotifyIcon1.Visible = false;
        }
        public void ExitSelect(object sender, System.EventArgs e)
        {
            if (MessageBoxByAsync(string.Format("是否停止采集并退出程序?\n\n进程名:{0}", this.Text), "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //隐藏托盘程序中的图标 
                NotifyIcon1.Visible = false;
                //关闭系统 
                Application.Exit();
                Dispose(true);
                Log.WriteLine("程序退出！", GlobalObject.RunFolderName);
            }
        }

        delegate DialogResult MessageBoxCallBack(string text, string caption, MessageBoxButtons okcancel);
        public DialogResult MessageBoxByAsync(string text, string caption, MessageBoxButtons okcancel)
        {
            DialogResult drt = DialogResult.Cancel;
            try
            {

                MessageBoxCallBack stcb = new MessageBoxCallBack(MsgBox);
                drt = (DialogResult)(this.Invoke(stcb, new object[] { text, caption, okcancel }));

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
            return drt;
        }
        public DialogResult MsgBox(string text, string caption, MessageBoxButtons okcancel)
        {
            return MessageBox.Show(text, caption, okcancel);
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "当前时间为：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }


    }
    /// <summary>
    /// 内存回收
    /// </summary>
    public class GCTool
    {
        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion
        private static System.Windows.Forms.Timer gcTimer = null;
        /// <summary>
        /// 释放
        /// </summary>
        public static void GCDispose()
        {
            if (gcTimer != null)
            {
                gcTimer.Stop();
                gcTimer.Enabled = false;
                gcTimer.Tick -= GcTimer_Tick;
                gcTimer.Dispose();
                gcTimer = null;
            }
        }
        /// <summary>
        /// 定时回收内存，每个6分钟进行一次内存回收
        /// </summary>
        public static void GCStart()
        {
            if (gcTimer == null)
            {
                gcTimer = new System.Windows.Forms.Timer();
                gcTimer.Interval = 1000 * 60 * 6;
                gcTimer.Tick += GcTimer_Tick;
                gcTimer.Enabled = true;
                gcTimer.Start();
            }
        }
        static void GcTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ClearMemory();
            }
            catch (Exception ex)
            {
                Log.WriteLine("GC释放出现问题." + ex.Message);
            }
        }
    }
}
