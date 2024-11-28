using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public static class Extensions
    {

        public static void AppenOk(this RichTextBox box, string text)
        {
            AppendText(box, text, Color.Green);
        }

        public static void AppenInfo(this RichTextBox box, string text)
        {
            AppendText(box, text, Color.Black);
        }


        public static void AppenError(this RichTextBox box, string text)
        {
            AppendText(box, text, Color.Red);
        }
        private static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText($"{ DateTime.Now.ToLongTimeString()} :{text} " + "\n");
            box.SelectionColor = box.ForeColor;
        }
    }
}
