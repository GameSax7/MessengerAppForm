using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MessengerAppForm
{
    public partial class UserProfileForm : Form
    {
        private User currentUser;
        private string username;

        // Конструктор для открытия собственного профиля
        public UserProfileForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            username = user.Username;
            LoadUserData();
        }

        // Конструктор для открытия чужого профиля
        public UserProfileForm(string username)
        {
            InitializeComponent();
            this.username = username;
            LoadUserData();
        }

        private void LoadUserData()
        {
            using (MySqlConnection connection = new MySqlConnection("Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;"))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Email, ProfilePicture, AboutMe, IsOnline FROM Users WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblUsername.Text = username;
                                lblEmail.Text = reader["Email"].ToString();
                                txtAboutMe.Text = reader["AboutMe"].ToString();

                                // Отображение статуса онлайн рядом с именем пользователя
                                bool isOnline = Convert.ToBoolean(reader["IsOnline"]);
                                lblUsername.Text += isOnline ? " (Online)" : " (Offline)";
                                lblUsername.ForeColor = isOnline ? Color.Green : Color.Red;

                                // Загрузка изображения профиля
                                if (reader["ProfilePicture"] != DBNull.Value)
                                {
                                    byte[] pictureData = (byte[])reader["ProfilePicture"];
                                    using (MemoryStream ms = new MemoryStream(pictureData))
                                    {
                                        picProfilePhoto.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    picProfilePhoto.Image = null;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        // Определение, чей профиль просматривается
                        if (currentUser != null && string.Equals(currentUser.Username, username, StringComparison.OrdinalIgnoreCase))
                        {
                            btnSaveInfo.Visible = true;
                            btnUploadPhoto.Visible = true;
                            btnLogout.Visible = true;
                            txtAboutMe.ReadOnly = false;
                            txtAboutMe.BackColor = SystemColors.Window;
                        }
                        else
                        {
                            btnGoToChat.Visible = false;
                            lblEmail.Visible = false;
                            label4.Visible = false;
                            btnViewProfile.Visible = false;
                            txtSearch.Visible = false;
                            btnSearch.Visible = false;
                            btnSaveInfo.Visible = false;
                            btnUploadPhoto.Visible = false;
                            btnLogout.Visible = false;
                            txtAboutMe.ReadOnly = true;
                            txtAboutMe.BackColor = SystemColors.Control;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveUserProfile(string username, byte[] profilePicture, string aboutMe)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;"))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Users SET ProfilePicture = @ProfilePicture, AboutMe = @AboutMe WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@ProfilePicture", profilePicture ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AboutMe", string.IsNullOrEmpty(aboutMe) ? (object)DBNull.Value : aboutMe);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            MessageBox.Show("Не удалось обновить профиль пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Выберите фото профиля";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    picProfilePhoto.Image = Image.FromFile(openFileDialog.FileName);
                }
            }
        }

        private void btnSaveInfo_Click(object sender, EventArgs e)
        {
            string aboutMe = txtAboutMe.Text;
            byte[] photoBytes = null;

            if (picProfilePhoto.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    picProfilePhoto.Image.Save(ms, picProfilePhoto.Image.RawFormat);
                    photoBytes = ms.ToArray();
                }
            }

            SaveUserProfile(username, photoBytes, aboutMe);
            MessageBox.Show("Профиль обновлен успешно.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            currentUser = null;
            MainForm loginForm = new MainForm();
            loginForm.Show();
            this.Close();
        }

        private void btnGoToChat_Click(object sender, EventArgs e)
        {
            AllChatForm allChatForm = new AllChatForm(username); // Передаем имя пользователя
            allChatForm.Show();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchUsername = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchUsername))
            {
                ShowSearchResult("Введите имя пользователя для поиска.");
                return;
            }

            string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;";
            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

            User foundUser = dbHelper.FindUserByUsername(searchUsername);

            if (foundUser != null)
            {
                ShowSearchResult($"Пользователь найден: {foundUser.Username}");
                btnViewProfile.Tag = foundUser.Username; // Сохраняем имя пользователя в свойство Tag кнопки
                btnViewProfile.Visible = true; // Показываем кнопку для перехода в профиль
            }
            else
            {
                ShowSearchResult("Пользователь не найден.");
                btnViewProfile.Visible = false; // Скрываем кнопку, если пользователь не найден
            }
        }

        // Метод для отображения результата поиска
        private void ShowSearchResult(string message)
        {
            lblSearchResult.Visible = true;
            lblSearchResult.ForeColor = Color.Black;
            lblSearchResult.Text = message;
        }

        private void btnViewProfile_Click(object sender, EventArgs e)
        {
            string foundUsername = btnViewProfile.Tag as string; // Retrieve the username from the Tag property

            if (!string.IsNullOrEmpty(foundUsername))
            {
                // Open the UserProfileForm for the found user
                UserProfileForm userProfileForm = new UserProfileForm(foundUsername);
                userProfileForm.Show();
            }
            else
            {
                MessageBox.Show("Не удалось открыть профиль пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void lblEmail_Click(object sender, EventArgs e)
        {

        }

        
    }
}
/*btnGoToChat.Visible = false;
                            lblEmail.Visible = false;
                            label4.Visible = false;
                            btnViewProfile.Visible = false;
                            txtSearch.Visible = false;
                            btnSearch.Visible = false;
                            btnSaveInfo.Visible = false;
                            btnUploadPhoto.Visible = false;
                            btnLogout.Visible = false;
                            txtAboutMe.ReadOnly = true;
                            txtAboutMe.BackColor = SystemColors.Control;*/
