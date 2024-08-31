using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using MessengerAppForm;

public class DatabaseHelper
{
    private string _connectionString;

    // Конструктор принимает строку подключения
    public DatabaseHelper(string connectionString)
    {
        _connectionString = connectionString;
    }
    public User FindUserByUsername(string username)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Username, Email FROM Users WHERE Username = @Username";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString()
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
    // Метод для регистрации пользователя
    public void RegisterUser(string username, string passwordHash, string email)
    {
        // Проверяем, существует ли уже пользователь
        if (UserExists(username))
        {
            throw new InvalidOperationException("Пользователь с таким именем уже существует.");
        }
        if (MailExists(email))
        {
            throw new InvalidOperationException("Email уже используется.");
        }

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Users (Username, PasswordHash, Email) VALUES (@Username, @PasswordHash, @Email)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@Email", email);

                command.ExecuteNonQuery();
            }

        }
    }

    // Метод для авторизации пользователя
    public bool AuthenticateUser(string username, string passwordHash)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0)
                {
                    // Пользователь авторизован, устанавливаем статус онлайн
                    SetUserOnlineStatus(username, true);
                    return true;
                }
                return false;
            }
        }
    }

    public void SetUserOnlineStatus(string username, bool isOnline)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
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
    }

    public void LogoutUser(string username)
    {
        SetUserOnlineStatus(username, false);
    }
    public User GetUserByUsername(string username)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Username, Email, ProfilePicture, AboutMe FROM Users WHERE Username = @Username";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            AboutMe = reader["AboutMe"].ToString(),
                            ProfilePicture = reader["ProfilePicture"] as byte[]
                        };
                    }
                    return null;
                }
            }
        }
    }

    public bool UserExists(string username)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }

    public bool MailExists(string email)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT COUNT(*) FROM Users WHERE email = @Email";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }

    public void SaveUserProfile(string username, byte[] profilePhoto, string aboutMe)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "UPDATE Users SET ProfilePicture = @ProfilePicture, AboutMe = @AboutMe WHERE Username = @Username";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@ProfilePicture", profilePhoto ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AboutMe", string.IsNullOrEmpty(aboutMe) ? (object)DBNull.Value : aboutMe);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    MessageBox.Show("Не удалось обновить профиль пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    public (string PhotoPath, string AboutMe) GetUserProfile(string username)
    {
        string photoPath = null;
        string aboutMe = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ProfilePhotoPath, AboutMe FROM Users WHERE Username = @Username";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        photoPath = reader["ProfilePhotoPath"] as string;
                        aboutMe = reader["AboutMe"] as string;
                    }
                }
            }
        }
        return (photoPath, aboutMe);
    }
    public void SendMessage(string username, string message)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "INSERT INTO ChatMessages (Username, Message) VALUES (@Username, @Message)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Message", message);
                command.ExecuteNonQuery();
            }

        }


    }
}
