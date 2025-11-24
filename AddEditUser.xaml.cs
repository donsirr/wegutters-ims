using DocumentFormat.OpenXml.Vml.Office;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WEGutters.DatabaseAccess;
using WEGutters.UserClasses;

namespace WEGutters
{
    /// <summary>
    /// Interaction logic for AddEditUser.xaml
    /// </summary>
    public partial class AddEditUser : Window
    {
        private bool isNew = true;
        private string createdDate;
        public User ReturnUser { get; set; }

        public AddEditUser(bool isNew, User currentUser = null)
        {
            InitializeComponent();

            this.isNew = isNew;
            ReturnUser = currentUser;
            DataContext = this;
            if (!isNew)
            {
                SetEditValues();
            }
        }

        private void SetEditValues()
        {
            if (ReturnUser == null) return;

            FirstNameBox.Text = ReturnUser.FirstName;
            LastNameBox.Text = ReturnUser.LastName;
            UsernameBox.Text = ReturnUser.Username;
            EmailBox.Text = ReturnUser.Email;
            PasswordBox.Text = ReturnUser.PasswordHash;
            AccessLevelBox.SelectedItem = ReturnUser.AccessLevel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs before saving
            if (!ValidateInputs(out string validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isNew)
            {
                // set created date to now for new items
                createdDate = DateTime.Now.ToString("yyyy-MM-dd_HH:mm");
            }
            else
            {
                // keep original created date for edits
                createdDate = ReturnUser.CreatedDate;
            }

            try
            {
                // If editing, update the existing instance in-place so its InventoryId remains intact.
                if (!isNew && ReturnUser != null)
                {
                    ReturnUser.FirstName = FirstNameBox.Text;
                    ReturnUser.LastName = LastNameBox.Text;
                    ReturnUser.Username = UsernameBox.Text;
                    ReturnUser.Email = EmailBox.Text;
                    ReturnUser.PasswordHash = PasswordBox.Text;
                    ReturnUser.AccessLevel = AccessLevelBox.Text;
                    ReturnUser.LastModified = DateTime.Now.ToString("yyyy-MM-dd_HH:mm");
                    // CreatedDate remains as-is (createdDate variable preserves it)
                }
                else
                {
                    // New item: create new instance (InventoryId will be assigned by DB on insert)
                    User user = new User(
                        UsernameBox.Text,
                        PasswordBox.Text,
                        FirstNameBox.Text,
                        LastNameBox.Text,
                        EmailBox.Text,
                        AccessLevelBox.Text,
                        1, // default as for now since isActive is currently featureless
                        DateTime.Now.ToString("yyyy-MM-dd_HH:mm"),
                        createdDate);
                    ReturnUser = user;
                }

                this.DialogResult = true; // tell caller we saved
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window.
            this.DialogResult = false; // This tells the MainWindow that we cancelled.
            this.Close();
        }

        private bool ValidateInputs(out string validationError)
        {
            var errors = new StringBuilder();

            // first name
            var firstName = (FirstNameBox.Text ?? string.Empty).Trim();
            FirstNameBox.Text = firstName;
            if (string.IsNullOrEmpty(firstName))
            {
                errors.AppendLine("- First Name cannot be blank.");
            }

            //Last name
            var lastName = (LastNameBox.Text ?? string.Empty).Trim();
            LastNameBox.Text = lastName;
            if (string.IsNullOrEmpty(lastName))
            {
                errors.AppendLine("- Last Name cannot be blank.");
            }

            //Username
            var username = (UsernameBox.Text ?? string.Empty).Trim();
            UsernameBox.Text = username;
            if (string.IsNullOrEmpty(username))
            {
                errors.AppendLine("- Username cannot be blank.");
            }
            else if (UserDBAccess.UserExists(username))
            {
                // If editing, allow the same name if it's the current customer
                if (isNew || (ReturnUser != null && !string.Equals(ReturnUser.Username, username, StringComparison.OrdinalIgnoreCase)))
                {
                    errors.AppendLine("- Username already in use.");
                }
            }

            // Password
            var userPassword = (PasswordBox.Text ?? string.Empty).Trim();
            PasswordBox.Text = userPassword;
            if (string.IsNullOrEmpty(userPassword))
            {
                errors.AppendLine("- Password cannot be blank.");
            }

            // Email
            var userEmail = (EmailBox.Text ?? string.Empty).Trim();
            EmailBox.Text = userEmail;
            if (string.IsNullOrEmpty(userEmail))
            {
                errors.AppendLine("- User Email cannot be blank.");
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(userEmail);
                    if (addr.Address != userEmail)
                    {
                        errors.AppendLine("- Email is not valid.");
                    }
                    else
                    {


                    }
                }
                catch
                {
                    errors.AppendLine("- Email is not valid.");
                }
                if (UserDBAccess.EmailExists(userEmail))
                {
                    // If editing, allow the same name if it's the current customer
                    if (isNew || (ReturnUser != null && !string.Equals(ReturnUser.Email, userEmail, StringComparison.OrdinalIgnoreCase)))
                    {
                        errors.AppendLine("- Email already in use.");
                    }
                }
            }

            validationError = errors.ToString().TrimEnd();
            return validationError.Length == 0;
        }
    }
}
