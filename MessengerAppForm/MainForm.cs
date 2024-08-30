using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MessengerAppForm
{

    public partial class MainForm : Form

    {
        private User currentUser;

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {


            // Завершаем приложение
            Application.Exit();
        }


        public void SetCurrentUser(User user)
        {
            currentUser = user;
        }

        // Обработчик кнопки регистрации
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Создание и показ формы регистрации
            RegistrationForm registrationForm = new RegistrationForm();

            // Скрываем текущую форму
            this.Hide();

            // Показ формы регистрации
            registrationForm.Show();
        }

        // Обработчик кнопки входа
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Хэшируем введенный пользователем пароль
            string passwordHash = HashPassword(password);
            string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;";

            // Создаем экземпляр DatabaseHelper и проверяем, существует ли пользователь
            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);
            bool isAuthenticated = dbHelper.AuthenticateUser(username, passwordHash);

            if (isAuthenticated)
            {
                // Получение информации о пользователе для создания объекта User
                User user = dbHelper.GetUserByUsername(username);
                if (user != null)
                {
                    MessageBox.Show("Вы успешно вошли в систему!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Создание и показ формы профиля с объектом User
                    UserProfileForm userProfileForm = new UserProfileForm(user);
                    userProfileForm.Show();
                    this.Hide(); // Скрываем текущую форму логина
                }
                else
                {
                    ShowErrorMessage("Не удалось получить информацию о пользователе.");
                }
            }
            else
            {
                ShowErrorMessage("Неправильное имя пользователя или пароль.");
            }
        }


        // Метод для хэширования пароля
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Метод для проверки правильности email
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Метод для проверки правильности имени пользователя
        private bool IsValidUsername(string username)
        {
            string usernamePattern = @"^[a-zA-Z0-9_.-]+$";
            return Regex.IsMatch(username, usernamePattern);
        }

        // Метод для проверки правильности пароля
        private bool IsValidPassword(string password)
        {
            string passwordPattern = @"^[a-zA-Z0-9_.-]+$";
            return Regex.IsMatch(password, passwordPattern);
        }

        // Метод для отображения сообщений об ошибках
        private void ShowErrorMessage(string message)
        {
            lblErrorMessage.Visible = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Text = message;
        }

        // Обработчик кнопки выхода
        private void btnLogout_Click(object sender, EventArgs e)
        {
            currentUser = null;
            Form loginForm = new MainForm();
            loginForm.Show();
            this.Close();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {

        }

    }
}
