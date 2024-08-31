namespace MessengerAppForm
{
    partial class UserProfileForm
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
            lblUsername = new Label();
            lblEmail = new Label();
            btnLogout = new Button();
            picProfilePhoto = new PictureBox();
            txtAboutMe = new TextBox();
            btnUploadPhoto = new Button();
            btnSaveInfo = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            btnGoToChat = new Button();
            btnSearch = new Button();
            txtSearch = new TextBox();
            lblSearchResult = new Label();
            btnViewProfile = new Button();
            lblOnlineStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)picProfilePhoto).BeginInit();
            SuspendLayout();
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.BackColor = Color.Transparent;
            lblUsername.Font = new Font("Segoe UI", 20F);
            lblUsername.Location = new Point(1, 45);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(246, 37);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "Имя пользователя";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.BackColor = Color.Transparent;
            lblEmail.Font = new Font("Segoe UI", 15F);
            lblEmail.Location = new Point(619, 363);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(65, 28);
            lblEmail.TabIndex = 1;
            lblEmail.Text = "label2";
            lblEmail.Click += lblEmail_Click;
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(627, 401);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(161, 23);
            btnLogout.TabIndex = 2;
            btnLogout.Text = "Выйти из профиля";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            // 
            // picProfilePhoto
            // 
            picProfilePhoto.BackgroundImageLayout = ImageLayout.Stretch;
            picProfilePhoto.Location = new Point(1, 85);
            picProfilePhoto.Name = "picProfilePhoto";
            picProfilePhoto.Size = new Size(278, 220);
            picProfilePhoto.TabIndex = 3;
            picProfilePhoto.TabStop = false;
            // 
            // txtAboutMe
            // 
            txtAboutMe.Location = new Point(322, 124);
            txtAboutMe.Multiline = true;
            txtAboutMe.Name = "txtAboutMe";
            txtAboutMe.Size = new Size(450, 134);
            txtAboutMe.TabIndex = 4;
            // 
            // btnUploadPhoto
            // 
            btnUploadPhoto.Location = new Point(40, 311);
            btnUploadPhoto.Name = "btnUploadPhoto";
            btnUploadPhoto.Size = new Size(161, 23);
            btnUploadPhoto.TabIndex = 5;
            btnUploadPhoto.Text = "Загрузить фото";
            btnUploadPhoto.UseVisualStyleBackColor = true;
            btnUploadPhoto.Click += btnUploadPhoto_Click;
            // 
            // btnSaveInfo
            // 
            btnSaveInfo.Location = new Point(40, 340);
            btnSaveInfo.Name = "btnSaveInfo";
            btnSaveInfo.Size = new Size(161, 23);
            btnSaveInfo.TabIndex = 6;
            btnSaveInfo.Text = "Подтвердить изменения";
            btnSaveInfo.UseVisualStyleBackColor = true;
            btnSaveInfo.Click += btnSaveInfo_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 20F);
            label2.Location = new Point(480, 77);
            label2.Name = "label2";
            label2.Size = new Size(125, 37);
            label2.TabIndex = 8;
            label2.Text = "Обо мне";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 32F);
            label3.Location = new Point(40, -6);
            label3.Name = "label3";
            label3.Size = new Size(400, 59);
            label3.TabIndex = 9;
            label3.Text = "Мой гачи профиль";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label4.Location = new Point(521, 370);
            label4.Name = "label4";
            label4.Size = new Size(92, 21);
            label4.TabIndex = 10;
            label4.Text = "Мой email";
            label4.Click += label4_Click;
            // 
            // btnGoToChat
            // 
            btnGoToChat.Location = new Point(231, 401);
            btnGoToChat.Name = "btnGoToChat";
            btnGoToChat.Size = new Size(307, 23);
            btnGoToChat.TabIndex = 11;
            btnGoToChat.Text = "Войти в общий чат";
            btnGoToChat.UseVisualStyleBackColor = true;
            btnGoToChat.Click += btnGoToChat_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(687, 32);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 13;
            btnSearch.Text = "Найти";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(672, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(100, 23);
            txtSearch.TabIndex = 14;
            // 
            // lblSearchResult
            // 
            lblSearchResult.AutoSize = true;
            lblSearchResult.Location = new Point(513, 6);
            lblSearchResult.Name = "lblSearchResult";
            lblSearchResult.Size = new Size(38, 15);
            lblSearchResult.TabIndex = 15;
            lblSearchResult.Text = "label1";
            lblSearchResult.Visible = false;
            // 
            // btnViewProfile
            // 
            btnViewProfile.Location = new Point(540, 30);
            btnViewProfile.Name = "btnViewProfile";
            btnViewProfile.Size = new Size(141, 23);
            btnViewProfile.TabIndex = 16;
            btnViewProfile.Text = "Перейти к профилю";
            btnViewProfile.UseVisualStyleBackColor = true;
            btnViewProfile.Click += btnViewProfile_Click;
            // 
            // lblOnlineStatus
            // 
            lblOnlineStatus.AutoSize = true;
            lblOnlineStatus.Location = new Point(253, 61);
            lblOnlineStatus.Name = "lblOnlineStatus";
            lblOnlineStatus.Size = new Size(87, 15);
            lblOnlineStatus.TabIndex = 17;
            lblOnlineStatus.Text = "lblOnlineStatus";
            // 
            // UserProfileForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources._171848470090491;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(lblOnlineStatus);
            Controls.Add(btnViewProfile);
            Controls.Add(lblSearchResult);
            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
            Controls.Add(btnGoToChat);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnSaveInfo);
            Controls.Add(btnUploadPhoto);
            Controls.Add(txtAboutMe);
            Controls.Add(picProfilePhoto);
            Controls.Add(btnLogout);
            Controls.Add(lblEmail);
            Controls.Add(lblUsername);
            Name = "UserProfileForm";
            Text = "Мой гачи профиль";
            ((System.ComponentModel.ISupportInitialize)picProfilePhoto).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUsername;
        private Label lblEmail;
        private Button btnLogout;
        private PictureBox picProfilePhoto;
        private TextBox txtAboutMe;
        private Button btnUploadPhoto;
        private Button btnSaveInfo;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button btnGoToChat;
        private Button btnSearch;
        private TextBox txtSearch;
        private Label lblSearchResult;
        private Button btnViewProfile;
        private Label lblOnlineStatus;
    }
}