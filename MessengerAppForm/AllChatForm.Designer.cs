namespace MessengerAppForm
{
    partial class AllChatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtMessageInput = new TextBox();
            btnSendMessage = new Button();
            flowLayoutPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // txtMessageInput
            // 
            txtMessageInput.Location = new Point(37, 405);
            txtMessageInput.Name = "txtMessageInput";
            txtMessageInput.Size = new Size(645, 23);
            txtMessageInput.TabIndex = 0;
            txtMessageInput.KeyDown += txtMessageInput_KeyDown;
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(698, 405);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(75, 23);
            btnSendMessage.TabIndex = 1;
            btnSendMessage.Text = "Отправить";
            btnSendMessage.UseVisualStyleBackColor = true;
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.Dock = DockStyle.Top;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.Location = new Point(0, 0);
            flowLayoutPanel.WrapContents = false;
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(800, 383);
            flowLayoutPanel.TabIndex = 3;
            // 
            // AllChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.c1bd1867478f40319407319e9a38;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSendMessage);
            Controls.Add(txtMessageInput);
            Controls.Add(flowLayoutPanel);
            Name = "AllChatForm";
            Text = "AllChatForm";
            Load += AllChatForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMessageInput;
        private Button btnSendMessage;
        private FlowLayoutPanel flowLayoutPanel;
    }
}