namespace MessengerAppForm
{
    partial class RegistrationForm
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
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtEmail = new TextBox();
            lblErrorMessage = new Label();
            button_reg = new Button();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(65, 115);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(100, 23);
            txtUsername.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(65, 180);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(100, 23);
            txtPassword.TabIndex = 1;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(65, 244);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(100, 23);
            txtEmail.TabIndex = 2;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.Location = new Point(65, 298);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(38, 15);
            lblErrorMessage.TabIndex = 4;
            lblErrorMessage.Text = "label1";
            lblErrorMessage.Visible = false;
            // 
            // button_reg
            // 
            button_reg.Location = new Point(346, 188);
            button_reg.Name = "button_reg";
            button_reg.Size = new Size(75, 23);
            button_reg.TabIndex = 5;
            button_reg.Text = "button1";
            button_reg.UseVisualStyleBackColor = true;
            button_reg.Click += button_reg_Click;
            // 
            // RegistrationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(779, 469);
            Controls.Add(button_reg);
            Controls.Add(lblErrorMessage);
            Controls.Add(txtEmail);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Name = "RegistrationForm";
            Text = "RegistrationForm";
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtEmail;
        private Button btnRegister;
        private Label lblErrorMessage;
        private Button btnRegister_Click;
        private Button button_reg;
    }
}