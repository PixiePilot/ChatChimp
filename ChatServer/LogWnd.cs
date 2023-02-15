using ChatServer.Core;
using ChatServer.Core.Globals;

namespace ChatServer
{
    public partial class LogWnd : Form
    {
        public LogWnd()
        {
            InitializeComponent();
            new Thread(load_classes).Start();
        }
        public void load_classes()
        {
            Thread.Sleep(3000);
            Globals.env             = new();
            Globals.authentication  = new();
            Globals.chatServer      = new();
            Globals.acceptor        = new();

            new Thread( Globals.acceptor.start ).Start();
        }

        private void cmdInput(object sender, EventArgs e)
        {
            RichTextBox input = (RichTextBox)sender;
            string cmd = input.Text;

            if (cmd.EndsWith('\n'))
            {
                input.Text = string.Empty;
                CommandHandler.handleCommand(cmd);
            }
            
        }
    }
}