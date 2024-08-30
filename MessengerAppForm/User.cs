using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerAppForm
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string AboutMe { get; set; }
        public byte[] ProfilePicture { get; set; }

        // Конструктор без параметров
        public User() { }

        // Конструктор с параметрами
        public User(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }
}