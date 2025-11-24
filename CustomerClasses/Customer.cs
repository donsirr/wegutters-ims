using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters.CustomerClasses
{
    public class Customer
    {
        private int customerID;
        private string name;
        private string address;
        private string contactNumber;
        private string email;
        private string comments;
        private string lastModified;
        private string createdDate;

        public Customer(string name, string address, string contactNumber, string email, string comments, string lastModified, string createdDate)
        {
            Name = name;
            Address = address;
            ContactNumber = contactNumber;
            Email = email;
            Comments = comments;
            LastModified = lastModified;
            CreatedDate = createdDate;
        }

        public int CustomerID
        {
            get { return customerID; }
            set
            {
                if (value >= 1)
                {
                    customerID = value;
                }
                else
                {
                    throw new ArgumentException("Customer ID cannot be 0 or lower");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    name = value;
                }
                else
                {
                    throw new ArgumentException("Name cannot be empty");
                }
            }
        }

        public string Address
        {
            get { return address; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    address = value;
                }
                else
                {
                    throw new ArgumentException("Address cannot be empty");
                }
            }
        }

        public string ContactNumber
        {
            get { return contactNumber; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    contactNumber = value;
                }
                else
                {
                    throw new ArgumentException("Contact Number cannot be empty");
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
                    throw new ArgumentException("Email cannot be empty");
                }
            }
        }

        public string Comments
        {
            get { return comments; }
            set
            {
                comments = value ?? string.Empty;
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
  
