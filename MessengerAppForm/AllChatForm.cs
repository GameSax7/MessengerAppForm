using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace MessengerAppForm
{
    public partial class AllChatForm : Form
    {
        private System.Windows.Forms.Timer timer;
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
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Интервал в миллисекундах (например, 5000 мс = 5 секунд)
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            LoadChatHistory();
        }

        private void AllChatForm_Load(object sender, EventArgs e)
        {
            // Обновляем статус онлайн при входе в приложение
            UpdateOnlineStatus(currentUser, true);
            LoadChatHistory(); // Загружаем историю чата при открытии формы
        }
        private void AllChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Обновляем статус оффлайн при закрытии формы
            UpdateOnlineStatus(currentUser, false);
        }

        private bool isUserScrolling = false; // Флаг для отслеживания, когда пользователь прокручивает вручную

        private void LoadChatHistory()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
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
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string username = reader["ChatUsername"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime timestamp = Convert.ToDateTime(reader["Timestamp"]);

                                bool isOnline = Convert.ToBoolean(reader["IsOnline"]);

                                // Вставляем временную метку
                                txtChatHistory.SelectionColor = Color.Black;
                                txtChatHistory.AppendText($"{timestamp} ");

                                // Вставляем имя пользователя
                                txtChatHistory.SelectionColor = Color.Blue;
                                txtChatHistory.AppendText(username);

                                // Вставляем статус (Online/Offline) с соответствующим цветом
                                txtChatHistory.SelectionColor = isOnline ? Color.Green : Color.Red;
                                txtChatHistory.AppendText(isOnline ? " (Online)" : " (Offline)");

                                // Вставляем сообщение
                                txtChatHistory.SelectionColor = Color.Black;
                                txtChatHistory.AppendText($": {message}\r\n");

                                if (timestamp > lastLoadedTimestamp)
                                {
                                    lastLoadedTimestamp = timestamp;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки сообщений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            string message = txtMessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                SaveMessage(currentUser, message);
                txtMessageInput.Clear(); // Очищаем поле ввода сообщения
                LoadChatHistory(); // Обновляем историю чата после отправки сообщения
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
