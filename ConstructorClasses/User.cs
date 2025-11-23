using System;

namespace WEGutters.ConstructorClasses
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // In a real app, hash this!
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AccessLevel { get; set; } // "Admin", "Manager", "Staff"
        public bool IsActive { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public User() { }

        public User(string username, string password, string firstName, string lastName, string email, string accessLevel, bool isActive)
        {
            Username = username;
            PasswordHash = password;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AccessLevel = accessLevel;
            IsActive = isActive;
        }
    }
}