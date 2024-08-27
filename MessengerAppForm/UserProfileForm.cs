using Microsoft.VisualBasic.ApplicationServices;
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
        private string email;

        public UserProfileForm(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        public UserProfileForm(string username)
        {
            InitializeComponent();
            this.username = username;
            LoadUserData();
        }

        private void LoadUserData()
        {
            using (MySqlConnection connection = new MySqlConnection("Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=root;"))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Email, ProfilePicture, AboutMe FROM Users WHERE Username = @Username";
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
                                    picProfilePhoto.Image = null; // Установите изображение по умолчанию, если нужно
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void SaveUserProfile(string username, byte[] profilePicture, string aboutMe)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=root;"))
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
                openFileDialog.Title = "Select a Profile Photo";

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
            Form loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }
    }
}
