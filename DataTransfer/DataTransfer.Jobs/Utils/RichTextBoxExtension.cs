using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataTransfer.Jobs.Utils
{
    public static class RichTextBoxExtension
    {
        private static int autoClearLength = 100000; 
        public static void SetAutoClearLength(this RichTextBox rtBox, int value)
        {
            if (value != 0)
            { autoClearLength = value; }
            else
            {
                autoClearLength = 9000000;
            } 
        }
        public static int GetAutoClearLength(this RichTextBox rtBox)
        {
            return autoClearLength;
        }
        delegate void SetTextCallBack(RichTextBox rtBox, string text, Color color, bool addNewLine = true);
        delegate void ClearTextCallBack(RichTextBox rtBox);
        public static void AppendTextByAsync(this RichTextBox rtBox, string text, Color color, bool addNewLine = true)
        {
            try
            {
                if (rtBox.IsDisposed) return;
                if (rtBox.InvokeRequired)
                {
                    SetTextCallBack stcb = new SetTextCallBack(AppendTextColorful);
                    rtBox.Invoke(stcb, new object[] { rtBox, text, color, addNewLine });
                }
                else
                {
                    if (addNewLine)
                    {
                        text += Environment.NewLine;
                    }
                    rtBox.SelectionStart = rtBox.TextLength;
                    rtBox.SelectionLength = 0;
                    rtBox.SelectionColor = color;
                    rtBox.AppendText(text);
                    rtBox.SelectionColor = rtBox.ForeColor;
                    rtBox.SelectionStart = rtBox.Text.Length; //Set the current caret position at the end
                    rtBox.ScrollToCaret(); //Now scroll it automatically
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public static void ClearTextByAsync(this RichTextBox rtBox)
        {
            try
            {
                if (rtBox.IsDisposed) return;
                if (rtBox.InvokeRequired)
                {
                    ClearTextCallBack stcb = new ClearTextCallBack(ClearTextColorfull);
                    rtBox.Invoke(stcb, new object[] { rtBox });
                }
                else
                {
                    rtBox.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }
        static void AppendTextColorful(this RichTextBox rtBox, string text, Color color, bool addNewLine = true)
        {
            if (rtBox.TextLength > autoClearLength)
            {
                rtBox.Clear();
            }
            if (addNewLine)
            {
                text += Environment.NewLine;
            }
            rtBox.SelectionStart = rtBox.TextLength;
            rtBox.SelectionLength = 0;
            rtBox.SelectionColor = color;
            rtBox.AppendText(text);
            rtBox.SelectionColor = rtBox.ForeColor;
            rtBox.SelectionStart = rtBox.Text.Length; //Set the current caret position at the end
            rtBox.ScrollToCaret(); //Now scroll it automatically
        }
        static void ClearTextColorfull(this RichTextBox rtBox)
        {
            rtBox.Clear();
        }
    }
}
