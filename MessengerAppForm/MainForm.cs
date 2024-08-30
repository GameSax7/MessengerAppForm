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


            // ��������� ����������
            Application.Exit();
        }


        public void SetCurrentUser(User user)
        {
            currentUser = user;
        }

        // ���������� ������ �����������
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // �������� � ����� ����� �����������
            RegistrationForm registrationForm = new RegistrationForm();

            // �������� ������� �����
            this.Hide();

            // ����� ����� �����������
            registrationForm.Show();
        }

        // ���������� ������ �����
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // �������� ��������� ������������� ������
            string passwordHash = HashPassword(password);
            string connectionString = "Server=188.225.45.127;Port=3306;Database=MessengerDB;User ID=root;Password=MessengerDB;";

            // ������� ��������� DatabaseHelper � ���������, ���������� �� ������������
            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);
            bool isAuthenticated = dbHelper.AuthenticateUser(username, passwordHash);

            if (isAuthenticated)
            {
                // ��������� ���������� � ������������ ��� �������� ������� User
                User user = dbHelper.GetUserByUsername(username);
                if (user != null)
                {
                    MessageBox.Show("�� ������� ����� � �������!", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // �������� � ����� ����� ������� � �������� User
                    UserProfileForm userProfileForm = new UserProfileForm(user);
                    userProfileForm.Show();
                    this.Hide(); // �������� ������� ����� ������
                }
                else
                {
                    ShowErrorMessage("�� ������� �������� ���������� � ������������.");
                }
            }
            else
            {
                ShowErrorMessage("������������ ��� ������������ ��� ������.");
            }
        }


        // ����� ��� ����������� ������
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // ����� ��� �������� ������������ email
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        // ����� ��� �������� ������������ ����� ������������
        private bool IsValidUsername(string username)
        {
            string usernamePattern = @"^[a-zA-Z0-9_.-]+$";
            return Regex.IsMatch(username, usernamePattern);
        }

        // ����� ��� �������� ������������ ������
        private bool IsValidPassword(string password)
        {
            string passwordPattern = @"^[a-zA-Z0-9_.-]+$";
            return Regex.IsMatch(password, passwordPattern);
        }

        // ����� ��� ����������� ��������� �� �������
        private void ShowErrorMessage(string message)
        {
            lblErrorMessage.Visible = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Text = message;
        }

        // ���������� ������ ������
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
