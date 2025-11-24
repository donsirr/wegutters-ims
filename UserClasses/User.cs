using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.IdentityModel.Tokens;
using System;

namespace WEGutters.UserClasses
{
    public class User
    {
        private int userID;
        private string username;
        private string passwordHash; // In a real app, hash this!
        private string firstName;
        private string lastName;
        private string email;
        private string accessLevel; // "Admin"(2), "Manager"(1), "Staff"(0)
        private string lastModified;
        private string createdDate;

        private int isActive { get; set; }



        public User(string username, string password, string firstName, string lastName, string email, string accessLevel, int isActive, string lastModified, string createdDate)
        {
            Username = username;
            PasswordHash = password;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AccessLevel = accessLevel;
            IsActive = isActive;
            LastModified = lastModified;
            CreatedDate = createdDate;
        }

        public string FullName => $"{FirstName} {LastName}";

        public int UserID
        {
            get { return userID; }
            set
            {
                if (value >= 1)
                {
                    userID = value;
                }
                else
                {
                    throw new ArgumentException("User ID cannot be 0 or lower");
                }
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    username = value;
                }
                else
                {
                    throw new ArgumentException("Username cannot be empty");
                }
            }
        }

        public string PasswordHash
        {
            get { return passwordHash; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    passwordHash = value;
                }
                else
                {
                    throw new ArgumentException("Password cannot be empty");
                }
            }
        }

        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    firstName = value;
                }
                else
                {
                    throw new ArgumentException("First Name cannot be empty");
                }
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    lastName = value;
                }
                else
                {
                    throw new ArgumentException("Last Name cannot be empty");
                }
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    email = value;
                }
                else
                {
                    throw new ArgumentException("Last Name cannot be empty");
                }
            }
        }

        public string AccessLevel
        {
            get { return accessLevel; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    accessLevel = value;
                }
                else
                {
                    throw new ArgumentException("Access Level cannot be empty");
                }
            }
        }

        public int IsActive
        {
            get { return isActive;  }
            set
            {
                if (value == 1 || value == 0)
                {
                    isActive = value;
                }
                else
                {
                    throw new ArgumentException("Activity can only be 0 or 1");
                }
            }
        }

        public string LastModified
        {
            get { return lastModified; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    lastModified = value;
                }
                else
                {
                    throw new ArgumentException("Last Modified date cannot be empty");
                }
            }
        }

        public string CreatedDate
        {
            get { return createdDate; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    createdDate = value;
                }
                else
                {
                    throw new ArgumentException("Created date cannot be empty");
                }
            }
        }

    }
}