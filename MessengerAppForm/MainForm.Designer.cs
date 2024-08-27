namespace MessengerAppForm
{
    partial class MainForm
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
            lblUsername = new Label();
            lblPassword = new Label();
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnRegister = new Button();
            btnLogin = new Button();
            label4 = new Label();
            lblErrorMessage = new Label();
            flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.BackColor = Color.White;
            lblUsername.Location = new Point(3, 0);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(31, 15);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "Имя";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.BackColor = Color.White;
            lblPassword.Location = new Point(3, 44);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(49, 15);
            lblPassword.TabIndex = 1;
            lblPassword.Text = "Пароль";
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.White;
            txtUsername.Location = new Point(3, 18);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(161, 23);
            txtUsername.TabIndex = 3;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(3, 62);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(161, 23);
            txtPassword.TabIndex = 4;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(56, 529);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(161, 35);
            btnRegister.TabIndex = 7;
            btnRegister.Text = "Зарегистрироваться";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(56, 479);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(161, 34);
            btnLogin.TabIndex = 8;
            btnLogin.Text = "Вход";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Georgia", 43F);
            label4.ForeColor = Color.Red;
            label4.Location = new Point(53, 106);
            label4.Name = "label4";
            label4.Size = new Size(263, 66);
            label4.TabIndex = 9;
            label4.Text = "Гачи чат";
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.BackColor = Color.Transparent;
            lblErrorMessage.Font = new Font("Segoe UI", 16F);
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(56, 425);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(154, 30);
            lblErrorMessage.TabIndex = 10;
            lblErrorMessage.Text = "Текст ошибки";
            lblErrorMessage.Visible = false;
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.BackColor = Color.Transparent;
            flowLayoutPanel.Controls.Add(lblUsername);
            flowLayoutPanel.Controls.Add(txtUsername);
            flowLayoutPanel.Controls.Add(lblPassword);
            flowLayoutPanel.Controls.Add(txtPassword);
            flowLayoutPanel.Location = new Point(53, 279);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(169, 90);
            flowLayoutPanel.TabIndex = 11;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.maxresdefault;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1099, 576);
            Controls.Add(flowLayoutPanel);
            Controls.Add(lblErrorMessage);
            Controls.Add(label4);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);
            Name = "Form1";
            Text = "Гачи чат";
            flowLayoutPanel.ResumeLayout(false);
            flowLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnRegister;
        private Button btnLogin;
        private Label label4;
        private Label lblErrorMessage;
        private FlowLayoutPanel flowLayoutPanel;
    }
}
