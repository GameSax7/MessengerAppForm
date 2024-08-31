using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace MessengerAppForm
{
    public partial class AllChatForm : Form
    {
        private System.Windows.Forms.Timer chatTimer;
        private System.Windows.Forms.Timer statusUpdateTimer;
        private string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;";
        private string currentUser;
        private DateTime lastLoadedTimestamp = DateTime.MinValue;

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
            chatTimer.Interval = 1000; // Интервал в миллисекундах для обновления чата
            chatTimer.Tick += new EventHandler(ChatTimer_Tick);
            chatTimer.Start();

            statusUpdateTimer = new System.Windows.Forms.Timer();
            statusUpdateTimer.Interval = 100; // Интервал для обновления статусов
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
            InitializeChat(); // Инициализация таймеров
            // Обновляем статус онлайн при входе в приложение
            UpdateOnlineStatus(currentUser, true);
            LoadNewChatMessagesAsync(); // Загружаем новые сообщения при открытии формы
        }

        private void AllChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Обновляем статус оффлайн при закрытии формы
            UpdateOnlineStatus(currentUser, false);
            chatTimer.Stop(); // Останавливаем таймер обновления чата
            statusUpdateTimer.Stop(); // Останавливаем таймер обновления статусов
        }
        private bool isFirstLoad = true;
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
                    Message, 
                    Timestamp, 
                    Users.IsOnline 
                FROM 
                    ChatMessages 
                JOIN 
                    Users 
                ON 
                    ChatMessages.Username = Users.Username 
                WHERE 
                    Timestamp > @lastLoadedTimestamp 
                ORDER BY 
                    Timestamp ASC";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@lastLoadedTimestamp", lastLoadedTimestamp);
                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            SuspendLayout(); // Отключаем перерисовку
                            while (await reader.ReadAsync())
                            {
                                string username = reader["ChatUsername"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime timestamp = Convert.ToDateTime(reader["Timestamp"]);
                                bool isOnline = Convert.ToBoolean(reader["IsOnline"]);

                                // Вставляем временную метку
                                txtChatHistory.SelectionColor = Color.Black;
                                txtChatHistory.AppendText($"{timestamp} ");

                                // Вставляем имя пользователя с синим цветом
                                txtChatHistory.SelectionColor = Color.Blue;
                                txtChatHistory.AppendText(username);

                                // Вставляем точку статуса (зеленую или красную) рядом с именем пользователя
                                txtChatHistory.SelectionColor = isOnline ? Color.Green : Color.Red;
                                txtChatHistory.AppendText(" ● "); // Цветной символ точки

                                // Вставляем сообщение
                                txtChatHistory.SelectionColor = Color.Black;
                                txtChatHistory.AppendText($": {message}\r\n");

                                if (timestamp > lastLoadedTimestamp)
                                {
                                    lastLoadedTimestamp = timestamp;
                                }
                            }

                            // Прокручиваем к концу текста только при первой загрузке
                            if (isFirstLoad)
                            {
                                txtChatHistory.ScrollToCaret();
                                isFirstLoad = false; // Сбрасываем флаг после первой загрузки
                            }

                            ResumeLayout(); // Включаем перерисовку
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
            // В данном примере метод остается пустым, так как статусы пользователей обновляются вместе с новыми сообщениями.
            // Вы можете добавить логику для обновления статусов пользователей, если это необходимо.
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            string message = txtMessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                SaveMessage(currentUser, message);
                txtMessageInput.Clear(); // Очищаем поле ввода сообщения
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