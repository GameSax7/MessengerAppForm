using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MessengerAppForm
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();

        }
        private void button_reg_Click(object sender, EventArgs e)
        {
            try
            {
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

                // Проверка корректности email
                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                {
                    ShowErrorMessage("Введите корректный email адрес.");
                    return;
                }

                // Хэширование пароля
                string passwordHash = HashPassword(password);
                string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=root;";
                DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

                // Регистрация пользователя
                dbHelper.RegisterUser(username, passwordHash, email);

                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Закрытие формы регистрации после успешной регистрации
                UserProfileForm userProfileForm = new UserProfileForm(username);
                userProfileForm.Show();
                this.Hide(); // Скрываем текущую форму логина
            }
            catch (InvalidOperationException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка: " + ex.Message);
            }
        }

        // Метод для отображения сообщений об ошибках
        private void ShowErrorMessage(string message)
        {
            lblErrorMessage.Visible = true;
            lblErrorMessage.ForeColor = System.Drawing.Color.Red;
            lblErrorMessage.Text = message;
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
    }
}




