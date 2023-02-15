using ChatServer.Core.Globals;

namespace ChatServer
{
    partial class LogWnd
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serverLog = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // serverLog
            // 
            this.serverLog.AccessibleName = "serverLog";
            this.serverLog.Location = new System.Drawing.Point(12, 12);
            this.serverLog.Name = "serverLog";
            this.serverLog.Size = new System.Drawing.Size(776, 439);
            this.serverLog.TabIndex = 0;
            this.serverLog.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(12, 466);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(776, 29);
            this.richTextBox2.TabIndex = 1;
            this.richTextBox2.Text = "";
            this.richTextBox2.TextChanged += new System.EventHandler(this.cmdInput);
            // 
            // LogWnd
            // 
            this.AccessibleName = "LogWnd";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 507);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.serverLog);
            this.Name = "LogWnd";
            this.ResumeLayout(false);

        }

        #endregion

        private RichTextBox serverLog;
        private RichTextBox richTextBox2;
    }
}