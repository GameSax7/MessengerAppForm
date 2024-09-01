using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace MessengerAppForm
{
    public partial class AllChatForm : Form
    {
        private const int AvatarSize = 50; // Размер аватара в пикселях
        private const int MessagePanelWidth = 400; // Ширина панели сообщения

        private System.Windows.Forms.Timer chatTimer;
        private System.Windows.Forms.Timer statusUpdateTimer;
        private string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;";
        private string currentUser;
        private DateTime lastLoadedTimestamp = DateTime.MinValue;
        private bool isFirstLoad = true;
        private ConcurrentDictionary<string, Image> avatarCache = new ConcurrentDictionary<string, Image>();

        public AllChatForm(string username)
        {
            InitializeComponent();
            currentUser = username;
            this.FormClosing += AllChatForm_FormClosing;
            this.Load += AllChatForm_Load;
        }

        private void InitializeChat()
        {
            chatTimer = new System.Windows.Forms.Timer();
            chatTimer.Interval = 1000;
            chatTimer.Tick += new EventHandler(ChatTimer_Tick);
            chatTimer.Start();

            statusUpdateTimer = new System.Windows.Forms.Timer();
            statusUpdateTimer.Interval = 100;
            statusUpdateTimer.Tick += new EventHandler(StatusUpdateTimer_Tick);
            statusUpdateTimer.Start();
        }

        private async void ChatTimer_Tick(object sender, EventArgs e)
        {
            await LoadNewChatMessagesAsync();
        }

        private async void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            await UpdateUsersOnlineStatusAsync();
        }

        private void AllChatForm_Load(object sender, EventArgs e)
        {
            InitializeChat();
            UpdateOnlineStatus(currentUser, true);
            LoadNewChatMessagesAsync();
        }

        private void AllChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateOnlineStatus(currentUser, false);
            chatTimer.Stop();
            statusUpdateTimer.Stop();
        }

        private async Task LoadNewChatMessagesAsync()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = @"
                    SELECT 
                        ChatMessages.Username AS ChatUsername, 
                        ChatMessages.Message, 
                        ChatMessages.Timestamp, 
                        Users.IsOnline, 
                        Users.ProfilePicture 
                    FROM 
                        ChatMessages 
                    JOIN 
                        Users 
                    ON 
                        ChatMessages.Username = Users.Username 
                    WHERE 
                        ChatMessages.Timestamp > @lastLoadedTimestamp 
                    ORDER BY 
                        ChatMessages.Timestamp ASC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@lastLoadedTimestamp", lastLoadedTimestamp);
                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            SuspendLayout();
                            while (await reader.ReadAsync())
                            {
                                string username = reader["ChatUsername"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime timestamp = Convert.ToDateTime(reader["Timestamp"]);
                                bool isOnline = Convert.ToBoolean(reader["IsOnline"]);
                                byte[] avatarBytes = reader["ProfilePicture"] as byte[];

                                Image avatar = null;
                                if (avatarBytes != null && avatarBytes.Length > 0)
                                {
                                    using (var ms = new MemoryStream(avatarBytes))
                                    {
                                        avatar = Image.FromStream(ms);
                                    }
                                }

                                DisplayMessage(username, message, avatar, timestamp, isOnline);

                                if (timestamp > lastLoadedTimestamp)
                                {
                                    lastLoadedTimestamp = timestamp;
                                }
                            }

                            if (isFirstLoad)
                            {
                                flowLayoutPanel.AutoScrollPosition = new Point(0, flowLayoutPanel.VerticalScroll.Maximum);
                                isFirstLoad = false;
                            }

                            ResumeLayout();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки сообщений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task UpdateUsersOnlineStatusAsync()
        {
            // Если потребуется, можно добавить логику здесь
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            string message = txtMessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                SaveMessage(currentUser, message);
                txtMessageInput.Clear();
            }
        }

        private void txtMessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSendMessage_Click(sender, e);
            }
        }

        private void SaveMessage(string username, string message)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO ChatMessages (Username, Message) VALUES (@username, @message)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Image ResizeImage(Image originalImage, int width, int height)
        {
            var newImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(originalImage, 0, 0, width, height);
            }
            return newImage;
        }

        private void DisplayMessage(string username, string message, Image avatar, DateTime timestamp, bool isOnline)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DisplayMessage(username, message, avatar, timestamp, isOnline)));
                return;
            }

            var messagePanel = new Panel
            {
                Width = MessagePanelWidth,
                Padding = new Padding(10),
                Margin = new Padding(5),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            var innerPanel = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AvatarSize));
            innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
            innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

            var avatarPictureBox = new PictureBox
            {
                Image = avatar != null ? ResizeImage(avatar, AvatarSize, AvatarSize) : Properties.Resources.defaultAvatar,
                Size = new Size(AvatarSize, AvatarSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.None
            };

            var usernameLabel = new Label
            {
                Text = username,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            var timestampLabel = new Label
            {
                Text = $"{timestamp:HH:mm}",
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            var statusLabel = new Label
            {
                Text = "●",
                ForeColor = isOnline ? Color.Green : Color.Red,
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(5)
            };

            var messageContentLabel = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                AutoSize = true,
                MaximumSize = new Size(MessagePanelWidth - AvatarSize - 40, 0),
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(5),
                Margin = new Padding(5),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            innerPanel.Controls.Add(avatarPictureBox, 0, 0);
            innerPanel.SetRowSpan(avatarPictureBox, 2);

            innerPanel.Controls.Add(usernameLabel, 1, 0);
            innerPanel.Controls.Add(timestampLabel, 3, 0);

            innerPanel.Controls.Add(messageContentLabel, 1, 1);
            innerPanel.SetColumnSpan(messageContentLabel, 3);

            innerPanel.Controls.Add(statusLabel, 2, 1);

            messagePanel.Controls.Add(innerPanel);

            flowLayoutPanel.SuspendLayout();
            flowLayoutPanel.Controls.Add(messagePanel);
            flowLayoutPanel.ResumeLayout();

            flowLayoutPanel.ScrollControlIntoView(messagePanel);
        }

        public void UpdateOnlineStatus(string username, bool isOnline)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Users SET IsOnline = @IsOnline WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@IsOnline", isOnline ? 1 : 0);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления статуса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
