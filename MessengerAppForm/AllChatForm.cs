﻿using System;
using System.Data;
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
            InitializeChat();
            currentUser = username;
            txtMessageInput.KeyDown += new KeyEventHandler(txtMessageInput_KeyDown);
            this.FormClosing += AllChatForm_FormClosing;
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
            LoadChatHistory();
        }

        private void LoadChatHistory()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Username, Message, Timestamp FROM ChatMessages WHERE Timestamp > @lastLoadedTimestamp ORDER BY Timestamp ASC";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@lastLoadedTimestamp", lastLoadedTimestamp);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string username = reader["Username"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime timestamp = Convert.ToDateTime(reader["Timestamp"]);
                                txtChatHistory.AppendText($"{timestamp} {username}: {message}\r\n");
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
                LoadChatHistory(); // Refresh chat after sending a new message
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

        private void AllChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer.Dispose();
            }
        }
    }



}
