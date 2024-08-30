using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MessengerAppForm
{
    public partial class AllChatForm : Form
    {
        private string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;";
        private string currentUser;

        public AllChatForm(string username)
        {
            InitializeComponent();
            currentUser = username;
        }

        private void AllChatForm_Load(object sender, EventArgs e)
        {
            LoadChatHistory();
        }

        private void LoadChatHistory()
        {
            txtChatHistory.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Username, Message, Timestamp FROM ChatMessages ORDER BY Timestamp ASC";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string username = reader.GetString("Username");
                        string message = reader.GetString("Message");
                        DateTime timestamp = reader.GetDateTime("Timestamp");

                        txtChatHistory.AppendText($"[{timestamp}] {username}: {message}{Environment.NewLine}");
                    }
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
    }
}
