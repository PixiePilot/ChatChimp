using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Core
{
    public static class GuiHandler
    {
        private static string time
        {
            get { return DateTime.Now.ToString("MM/dd/yyyy HH:mm"); }
        }

        public static RichTextBox logWindow { get { return (RichTextBox)Application.OpenForms["LogWnd"]!.Controls["serverLog"]!; } }

        public static void writeEvent(string msg)
        {
            RichTextBox log = logWindow;
            log.Invoke(new Action(() => { log.AppendText($"{time} [Event] {msg}\r\n"); }));
        } 
        public static void writeError(string msg)
        {
            RichTextBox log = logWindow;
            log.Invoke(new Action(() => { log.AppendText($"{time} [Error] {msg}\r\n"); }));
        } 

    }
}
