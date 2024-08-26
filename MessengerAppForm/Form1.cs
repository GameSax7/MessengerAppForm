using Microsoft.VisualBasic.ApplicationServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MessengerAppForm
{
    public partial class Form1 : Form
    {
        private User currentUser;

        public Form1()
        {
            InitializeComponent();
        }

        public void SetCurrentUser(User user)
        {
            currentUser = user;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                flowLayoutPanel.Visible = true;
                txtEmail.Visible = true;
                lblEmail.Visible = true;
                lblErrorMessage.Visible = false;

                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();
                string email = txtEmail.Text.Trim();

                // Проверка минимальной длины для имени пользователя
                if (username.Length < 3)
                {
                    ShowErrorMessage("Имя пользователя должно содержать минимум 3 символа.");
                    return;
                }

                // Проверка на недопустимые символы в имени пользователя
                if (!IsValidUsername(username))
                {
                    ShowErrorMessage("Имя пользователя может содержать только буквы, цифры и символы _ . -");
                    return;
                }

                // Проверка минимальной длины для пароля
                if (password.Length < 6)
                {
                    ShowErrorMessage("Пароль должен содержать минимум 6 символов.");
                    return;
                }

                // Проверка на недопустимые символы в пароле
                if (!IsValidPassword(password))
                {
                    ShowErrorMessage("Пароль может содержать только буквы, цифры и символы _ . -");
                    return;
                }

                // Проверка на наличие email
                if (string.IsNullOrWhiteSpace(email))
                {
                    ShowErrorMessage("Введите корректный email адрес.");
                    return;
                }

                // Проверка корректности формата email
                if (!IsValidEmail(email))
                {
                    ShowErrorMessage("Введите корректный email адрес.");
                    return;
                }

                // Хэшируем пароль
                string passwordHash = HashPassword(password);
                string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=root;";
                DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

                // Пытаемся зарегистрировать нового пользователя
                dbHelper.RegisterUser(username, passwordHash, email);

                // Если регистрация прошла успешно, показываем сообщение
                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                // Обработка случая, когда пользователь уже существует
                ShowErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                // Обработка других ошибок
                ShowErrorMessage("Ошибка: " + ex.Message);
            }
        }

        private void ShowErrorMessage(string message)
        {
            lblErrorMessage.Visible = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Text = message;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            flowLayoutPanel.Visible = true;
            txtEmail.Visible = false;
            lblEmail.Visible = false;

            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Хэшируем введенный пользователем пароль
            string passwordHash = HashPassword(password);
            string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=root;";

            // Создаем экземпляр DatabaseHelper и проверяем, существует ли пользователь
            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);
            bool isAuthenticated = dbHelper.AuthenticateUser(username, passwordHash);

            if (isAuthenticated)
            {
                MessageBox.Show("Вы успешно вошли в систему!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UserProfileForm userProfileForm = new UserProfileForm(username);
                userProfileForm.Show();
                this.Hide(); // Скрываем текущую форму логина
            }
            else
            {
                MessageBox.Show("Неправильное имя пользователя или пароль.", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidUsername(string username)
        {
            string usernamePattern = @"^[a-zA-Z0-9_.-]+$";
            return Regex.IsMatch(username, usernamePattern);
        }

        private bool IsValidPassword(string password)
        {
            string passwordPattern = @"^[a-zA-Z0-9_.-]+$";
            return Regex.IsMatch(password, passwordPattern);
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
