using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessengerAppForm
{
    public partial class AllChatForm : Form
    {
        private User currentUser;

        public AllChatForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadMessages();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();

            if (!string.IsNullOrEmpty(message))
            {
                SendMessageToChat(currentUser.Username, message);
                txtMessage.Clear();
                LoadMessages(); // Обновляем чат
            }
        }

        private void SendMessageToChat(string username, string message)
        {
            string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=root;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO ChatMessages (Username, Message, Timestamp) VALUES (@Username, @Message, @Timestamp)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void LoadMessages()
        {
            string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=root;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Username, Message, Timestamp FROM ChatMessages ORDER BY Timestamp ASC";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    lstMessages.Items.Clear();

                    while (reader.Read())
                    {
                        string username = reader.GetString("Username");
                        string message = reader.GetString("Message");
                        DateTime timestamp = reader.GetDateTime("Timestamp");
                        lstMessages.Items.Add($"{timestamp} [{username}]: {message}");
                    }
                }
            }
        }
    }

}