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
