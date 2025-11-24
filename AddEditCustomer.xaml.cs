using DocumentFormat.OpenXml.Vml.Office;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using WEGutters.CustomerClasses;
using WEGutters.DatabaseAccess;

namespace WEGutters
{
    /// <summary>
    /// Interaction logic for AddEditCustomer.xaml
    /// </summary>
    public partial class AddEditCustomer : Window
    {
        private bool isNew = true;
        private string createdDate;
        public Customer ReturnCustomer { get; set; }


        public AddEditCustomer(bool isNew, Customer currentCustomer = null)
        {
            InitializeComponent();

            this.isNew = isNew;
            ReturnCustomer = currentCustomer;
            DataContext = this;
            if (!isNew)
            {
                SetEditValues();
            }
        }

        private void SetEditValues()
        {
            if (ReturnCustomer == null) return;
      
            CustomerNameBox.Text = ReturnCustomer.Name;
            CustomerAddressBox.Text = ReturnCustomer.Address;
            ContactNumberBox.Text = ReturnCustomer.ContactNumber;
            CustomerEmailBox.Text = ReturnCustomer.Email;
            CustomerCommentsBox.Text = ReturnCustomer.Comments;
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
                createdDate = ReturnCustomer.CreatedDate;
            }

            try
            {
                // If editing, update the existing instance in-place so its InventoryId remains intact.
                if (!isNew && ReturnCustomer != null)
                {
                    ReturnCustomer.Name = CustomerNameBox.Text;
                    ReturnCustomer.Address = CustomerAddressBox.Text;
                    ReturnCustomer.ContactNumber = ContactNumberBox.Text;
                    ReturnCustomer.Email = CustomerEmailBox.Text;
                    ReturnCustomer.Comments = CustomerCommentsBox.Text;
                    ReturnCustomer.LastModified = DateTime.Now.ToString("yyyy-MM-dd_HH:mm");
                    // CreatedDate remains as-is (createdDate variable preserves it)
                }
                else
                {
                    // New item: create new instance (InventoryId will be assigned by DB on insert)
                    Customer customer = new Customer(
                        CustomerNameBox.Text,
                        CustomerAddressBox.Text,
                        ContactNumberBox.Text,
                        CustomerEmailBox.Text,
                        CustomerCommentsBox.Text,
                        DateTime.Now.ToString("yyyy-MM-dd_HH:mm"),
                        createdDate);
                    ReturnCustomer = customer;
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

            //Customer Name
            var customerName = (CustomerNameBox.Text ?? string.Empty).Trim();
            CustomerNameBox.Text = customerName;
            if (string.IsNullOrEmpty(customerName))
            {
                errors.AppendLine("- Customer Name cannot be blank.");
            }
            else if (CustomerDBAccess.CustomerExists(customerName))
            {
                 // If editing, allow the same name if it's the current customer
                if (isNew || (ReturnCustomer != null && !string.Equals(ReturnCustomer.Name, customerName, StringComparison.OrdinalIgnoreCase)))
                {
                    errors.AppendLine("- A customer with this name already exists.");
                }
            }

            // Customer Address
            var customerAddress = (CustomerAddressBox.Text ?? string.Empty).Trim();
            CustomerAddressBox.Text = customerAddress;
            if (string.IsNullOrEmpty(customerAddress))
            {
                errors.AppendLine("- Customer Address cannot be blank.");
            }


            // Contact Number
            var contactNumber = (ContactNumberBox.Text ?? string.Empty).Trim();
            ContactNumberBox.Text = contactNumber;
            if (string.IsNullOrEmpty(contactNumber))
            {
                errors.AppendLine("- Contact Number cannot be blank.");
            }
            else
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                try
                {
                    // Parse the number as Canada-based
                    var number = phoneUtil.Parse(contactNumber, "CA");

                    // Validate the number
                    if (!phoneUtil.IsValidNumber(number))
                    {
                        errors.AppendLine("- Contact Number is not a valid number.");
                    }
                }
                catch (NumberParseException)
                {
                    errors.AppendLine("- Contact Number is invalid or incorrectly formatted.");
                }
            }

            // Email
            var customerEmail = (CustomerEmailBox.Text ?? string.Empty).Trim();
            CustomerEmailBox.Text = customerEmail;
            if (string.IsNullOrEmpty(customerEmail))
            {
                errors.AppendLine("- Customer Email cannot be blank.");
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(customerEmail);
                    if (addr.Address != customerEmail)
                    {
                        errors.AppendLine("- Customer Email is not valid.");
                    }
                }
                catch
                {
                    errors.AppendLine("- Customer Email is not valid.");
                }
            }

            // Comments
            var customerComments = (CustomerCommentsBox.Text ?? string.Empty).Trim();
            CustomerCommentsBox.Text = customerComments;
            if (string.IsNullOrEmpty(customerComments))
            {
                errors.AppendLine("- Comments cannot be blank.");
            }

            validationError = errors.ToString().TrimEnd();
            return validationError.Length == 0;
        }

    }
}
