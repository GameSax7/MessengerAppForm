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
        private DateTime lastLoadedMessageTimestamp = DateTime.MinValue;

        private int pageSize = 20; // Количество сообщений, загружаемых за один раз
        private int currentPage = 0; // Текущая страница
        private bool isLoading = false; // Флаг, указывающий, загружаются ли данные

        private const int AvatarSize = 50; // Размер аватара в пикселях
        private const int MessagePanelWidth = 400; // Ширина панели сообщения
        private const int MaxMessagesInCache = 500; // Максимальное количество сообщений в кэше

        private System.Windows.Forms.Timer chatTimer;
        private System.Windows.Forms.Timer statusUpdateTimer;
        private System.Windows.Forms.Timer inactivityTimer; // Таймер для отслеживания бездействия
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
            this.Shown += AllChatForm_Shown; // Подписка на событие Shown
        }


        private void InitializeChat()
        {
            chatTimer = new System.Windows.Forms.Timer();
            chatTimer.Interval = 1000;
            chatTimer.Tick += new EventHandler(ChatTimer_Tick);
            chatTimer.Start();

            statusUpdateTimer = new System.Windows.Forms.Timer();
            statusUpdateTimer.Interval = 5000; // Интервал увеличен для снижения нагрузки
            statusUpdateTimer.Tick += new EventHandler(StatusUpdateTimer_Tick);
            statusUpdateTimer.Start();

            inactivityTimer = new System.Windows.Forms.Timer(); // Инициализация таймера бездействия
            inactivityTimer.Interval = 300000; // 5 минут (300000 миллисекунд)
            inactivityTimer.Tick += new EventHandler(InactivityTimer_Tick);
            inactivityTimer.Start(); // Запуск таймера
        }
        private async void AllChatForm_Shown(object sender, EventArgs e)
        {
            await LoadNewChatMessagesAsync();
        }

        private async void ChatTimer_Tick(object sender, EventArgs e)
        {
            await LoadNewChatMessagesAsync();
        }

        private void flowLayoutPanelMessages_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int scrollPosition = flowLayoutPanel.VerticalScroll.Value;

                // Проверяем, если прокрутили вверх до самого верха и не идет загрузка
                if (scrollPosition == 0 && !isLoading)
                {
                    currentPage++;
                    _ = LoadNewChatMessagesAsync();
                }
            }
        }



        private async void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            await UpdateUsersOnlineStatusAsync();
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            UpdateOnlineStatus(currentUser, false); // Переводим пользователя в оффлайн
            inactivityTimer.Stop(); // Останавливаем таймер после изменения статуса
        }

        private async void AllChatForm_Load(object sender, EventArgs e)
        {
            InitializeChat();
            UpdateOnlineStatus(currentUser, true);
            SendSystemMessage($"{currentUser} вошел в чат.");

            // Убедимся, что форма полностью загружена перед загрузкой сообщений
            await Task.Delay(500); // Небольшая задержка, чтобы убедиться, что форма полностью инициализирована

            await LoadNewChatMessagesAsync();
        }



        private void AllChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateOnlineStatus(currentUser, false);
            SendSystemMessage($"{currentUser} вышел из чата.");
            chatTimer.Stop();
            statusUpdateTimer.Stop();
            inactivityTimer.Stop(); // Останавливаем таймер при закрытии формы
        }

        private void SendSystemMessage(string message)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO ChatMessages (Username, Message, IsSystemMessage) VALUES (@username, @message, @isSystemMessage)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", "Система");
                        cmd.Parameters.AddWithValue("@message", message);
                        cmd.Parameters.AddWithValue("@isSystemMessage", true);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка отправки системного сообщения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task LoadNewChatMessagesAsync()
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action(async () => await LoadNewChatMessagesAsync()));
                return;
            }

            if (isLoading)
                return;

            isLoading = true;

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
                Users.ProfilePicture,
                ChatMessages.IsSystemMessage 
            FROM 
                ChatMessages 
            LEFT JOIN 
                Users 
            ON 
                ChatMessages.Username = Users.Username 
            WHERE 
                ChatMessages.Timestamp > @lastTimestamp
            ORDER BY 
                ChatMessages.Timestamp ASC";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@lastTimestamp", lastLoadedMessageTimestamp);

                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            var messages = new List<(string Username, string Message, Image Avatar, DateTime Timestamp, bool IsOnline, bool IsSystemMessage)>();

                            while (await reader.ReadAsync())
                            {
                                string username = reader["ChatUsername"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime timestamp = Convert.ToDateTime(reader["Timestamp"]);
                                bool isOnline = reader["IsOnline"] != DBNull.Value && Convert.ToBoolean(reader["IsOnline"]);
                                bool isSystemMessage = Convert.ToBoolean(reader["IsSystemMessage"]);

                                Image avatar = null;
                                if (!isSystemMessage)
                                {
                                    if (!avatarCache.TryGetValue(username, out avatar))
                                    {
                                        byte[] avatarBytes = reader["ProfilePicture"] as byte[];
                                        if (avatarBytes != null && avatarBytes.Length > 0)
                                        {
                                            using (var ms = new MemoryStream(avatarBytes))
                                            {
                                                avatar = Image.FromStream(ms);
                                                avatarCache[username] = avatar;
                                            }
                                        }
                                    }
                                }

                                messages.Add((username, message, avatar, timestamp, isOnline, isSystemMessage));

                                // Обновляем последний загруженный Timestamp
                                lastLoadedMessageTimestamp = timestamp;
                            }

                            if (messages.Count > 0)
                            {
                                // Обновление UI
                                this.Invoke(new Action(() =>
                                {
                                    if (this.IsDisposed)
                                        return;

                                    SuspendLayout();
                                    foreach (var msg in messages)
                                    {
                                        DisplayMessage(msg.Username, msg.Message, msg.Avatar, msg.Timestamp, msg.IsOnline, msg.IsSystemMessage);
                                    }
                                    ResumeLayout();
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!this.IsDisposed)
                    {
                        MessageBox.Show($"Ошибка загрузки сообщений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    isLoading = false;
                }
            }
        }





        private async Task UpdateUsersOnlineStatusAsync()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT Username, IsOnline FROM Users";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string username = reader["Username"].ToString();
                                bool isOnline = Convert.ToBoolean(reader["IsOnline"]);
                                // Обновление статуса пользователей в интерфейсе
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления статуса пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            string message = txtMessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                SaveMessage(currentUser, message);
                txtMessageInput.Clear();

                // Перезапуск таймера бездействия при отправке сообщения
                UpdateOnlineStatus(currentUser, true); // Подтверждаем, что пользователь активен
                inactivityTimer.Stop();
                inactivityTimer.Start();
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
        private void UpdateOnlineStatus(string username, bool isOnline)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Users SET IsOnline = @isOnline WHERE Username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@isOnline", isOnline);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления статуса пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void OpenUserProfile(string username)
        {
             // Открытие новой формы с информацией о пользователе
             UserProfileForm profileForm = new UserProfileForm(username);
             profileForm.Show();
        }

        private void DisplayMessage(string username, string message, Image avatar, DateTime timestamp, bool isOnline, bool isSystemMessage)
        {


            if (!this.IsHandleCreated)
            {
                // Если дескриптор не создан, отложите выполнение до момента, когда он будет готов
                this.HandleCreated += (s, e) => DisplayMessage(username, message, avatar, timestamp, isOnline, isSystemMessage);
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() => DisplayMessage(username, message, avatar, timestamp, isOnline, isSystemMessage)));
                return;
            }


            // Создание панели сообщения
            var messagePanel = new Panel
            {
                Width = MessagePanelWidth,
                Padding = new Padding(10),
                Margin = new Padding(5),
                BackColor = isSystemMessage ? Color.LightGray : Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Создание внутренней панели для содержимого сообщения
            var innerPanel = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            // Добавление аватара пользователя, если это не системное сообщение
            if (!isSystemMessage)
            {
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

                avatarPictureBox.Click += (sender, e) => OpenUserProfile(username); // Обработчик клика

                innerPanel.Controls.Add(avatarPictureBox, 0, 0);
                innerPanel.SetRowSpan(avatarPictureBox, 2);
            }

            // Создание и настройка остальных элементов панели сообщения
            var usernameLabel = new Label
            {
                Text = isSystemMessage ? "Система" : username,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Cursor = Cursors.Hand // Устанавливаем курсор в виде руки
            };
            usernameLabel.Click += (sender, e) => OpenUserProfile(username); // Обработчик клика

            var timestampLabel = new Label
            {
                Text = $"{timestamp:HH:mm}",
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            var statusLabel = new Label
            {
                Text = isSystemMessage ? string.Empty : "●",
                ForeColor = isSystemMessage ? Color.Transparent : (isOnline ? Color.Green : Color.Red),
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
                MaximumSize = new Size(MessagePanelWidth - (isSystemMessage ? 20 : AvatarSize + 40), 0),
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(5),
                Margin = new Padding(5),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // Добавление элементов на внутреннюю панель
            innerPanel.Controls.Add(usernameLabel, 1, 0);
            innerPanel.Controls.Add(timestampLabel, 3, 0);
            innerPanel.Controls.Add(messageContentLabel, 1, 1);
            innerPanel.SetColumnSpan(messageContentLabel, 3);
            innerPanel.Controls.Add(statusLabel, 2, 1);

            // Добавление внутренней панели на панель сообщения
            messagePanel.Controls.Add(innerPanel);

            // Добавление панели сообщения на FlowLayoutPanel с приостановкой и возобновлением компоновки для повышения производительности
            flowLayoutPanel.SuspendLayout();
            flowLayoutPanel.Controls.Add(messagePanel);
            flowLayoutPanel.ResumeLayout();

            // Прокрутка к новому сообщению
            flowLayoutPanel.ScrollControlIntoView(messagePanel);
        }
    }
}
