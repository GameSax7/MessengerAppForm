using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MessengerAppForm
{
    public partial class AllChatForm : Form
    {
        private System.Windows.Forms.Timer statusTimer;
        private string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;";
        private string currentUser;

        public AllChatForm(string username)
        {
            InitializeComponent();
            currentUser = username;

            // Инициализация таймера для обновления статусов
            statusTimer = new System.Windows.Forms.Timer();
            statusTimer.Interval = 10000; // Интервал в миллисекундах (например, 10000 мс = 10 секунд)
            statusTimer.Tick += StatusTimer_Tick;
            statusTimer.Start();

            this.FormClosing += AllChatForm_FormClosing;
            this.Load += AllChatForm_Load;
        }
        private void LoadChatHistory()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Username, Message, Timestamp FROM ChatMessages ORDER BY Timestamp ASC";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            txtChatHistory.Clear(); // Очистите текущий текст в RichTextBox
                            while (reader.Read())
                            {
                                string username = reader["Username"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime timestamp = Convert.ToDateTime(reader["Timestamp"]);
                                txtChatHistory.AppendText($"{timestamp} {username}: {message}\r\n");
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
        private void AllChatForm_Load(object sender, EventArgs e)
        {
            // Обновляем статус онлайн при входе в приложение
            UpdateOnlineStatus(currentUser, true);
            LoadChatHistory(); // Загружаем историю чата при открытии формы
        }

        private void AllChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Останавливаем таймер и обновляем статус оффлайн
            if (statusTimer != null)
            {
                statusTimer.Stop();
                statusTimer.Tick -= StatusTimer_Tick;
                statusTimer.Dispose();
            }
            UpdateOnlineStatus(currentUser, false);
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            UpdateUserStatuses();
        }

        private void UpdateUserStatuses()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Username, IsOnline FROM Users";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string username = reader["Username"].ToString();
                                bool isOnline = Convert.ToBoolean(reader["IsOnline"]);
                                // Обновите статусы в UI
                                UpdateUserStatusInUI(username, isOnline);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления статусов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateUserStatusInUI(string username, bool isOnline)
        {
            // Реализуйте логику для обновления статуса в UI, например:
            // 1. Найдите элемент в списке пользователей
            // 2. Обновите текст или цвет, чтобы отразить статус (Online/Offline)
            // Пример обновления статуса в списке пользователей:
            foreach (ListViewItem item in .Items)
            {
                if (item.Text == username)
                {
                    item.SubItems[1].Text = isOnline ? "Online" : "Offline";
                    item.SubItems[1].ForeColor = isOnline ? Color.Green : Color.Red;
                    break;
                }
            }
        }

        private void UpdateOnlineStatus(string username, bool isOnline)
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
