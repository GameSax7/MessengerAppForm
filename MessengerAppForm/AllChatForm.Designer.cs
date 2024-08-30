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
            txtChatHistory = new TextBox();
            SuspendLayout();
            // 
            // txtMessageInput
            // 
            txtMessageInput.Location = new Point(37, 380);
            txtMessageInput.Name = "txtMessageInput";
            txtMessageInput.Size = new Size(465, 23);
            txtMessageInput.TabIndex = 0;
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(698, 379);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(75, 23);
            btnSendMessage.TabIndex = 1;
            btnSendMessage.Text = "button1";
            btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // txtChatHistory
            // 
            txtChatHistory.Location = new Point(37, 37);
            txtChatHistory.Multiline = true;
            txtChatHistory.Name = "txtChatHistory";
            this.txtChatHistory.ReadOnly = true;
            this.txtChatHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtChatHistory.Size = new Size(736, 318);
            txtChatHistory.TabIndex = 2;
            // 
            // AllChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtChatHistory);
            Controls.Add(btnSendMessage);
            Controls.Add(txtMessageInput);
            Name = "AllChatForm";
            Text = "AllChatForm";
            this.Load += new System.EventHandler(this.AllChatForm_Load);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMessageInput;
        private Button btnSendMessage;
        private TextBox txtChatHistory;
    }
}