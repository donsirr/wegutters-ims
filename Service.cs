using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEGutters.ConstructorClasses;

namespace WEGutters
{
    public class Service
    {
        private int serviceID;
        private Customer customer;
        private string serviceDetails;
        private ServiceCategory serviceCategory;
        private float materialCost;
        private float invoicePrice;
        private string details;
        private string lastModified;
        private string createdDate;

        public Service(Customer customer, string serviceDetails, ServiceCategory serviceCategory, float materialCost, float invoicePrice, string details, string lastModified, string createdDate)
        {
            Customer = customer;
            ServiceDetails = serviceDetails;
            ServiceCategory = serviceCategory;
            MaterialCost = materialCost;
            InvoicePrice = invoicePrice;
            Details = details;
            LastModified = lastModified;
            CreatedDate = CreatedDate;
        }

        public int ServiceID
        {
            get { return serviceID; }
            set
            {
                if (value >= 0)
                {
                    serviceID = value;
                }
                else
                {
                    throw new ArgumentException("Service ID cannot be negative");
                }
            }
        }

        public Customer Customer
        {
            get { return customer; }
            set
            {
                if (value != null)
                {
                    customer = value;
                }
                else
                {
                    throw new ArgumentException("Customer cannot be null");
                }
            }
        }

        public string CustomerName
        {
            get { return customer.Name; }
        }

        public string CustomerAddress
        {
            get { return customer.Address; }
        }

        public string CustomerEmail
        {
            get { return customer.Email; }
        }

        public string CustomerContactNumber
        {
            get { return customer.ContactNumber; }
        }

        //public string ContactNumber
        //{
        //    get { return contactNumber; }
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            contactNumber = value;
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Contact Number cannot be empty");
        //        }
        //    }
        //}

        //public string Address
        //{
        //    get { return address; }
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            address = value;
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Address cannot be empty");
        //        }
        //    }
        //}

        public string ServiceDetails
        {
            get { return serviceDetails; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    serviceDetails = value;
                }
                else
                {
                    throw new ArgumentException("Service Details cannot be empty");
                }
            }
        }

        public ServiceCategory ServiceCategory
        {
            get { return serviceCategory; }
            set
            {
                if (value != null)
                {
                    serviceCategory = value;
                }
                else
                {
                    throw new ArgumentException("Service Category cannot be null");
                }
            }
        }

        public float MaterialCost
        {
            get { return materialCost; }
            set
            {
                if (value >= 0)
                {
                    materialCost = value;
                }
                else
                {
                    throw new ArgumentException("Material Cost cannot be negative");
                }
            }
        }

        public float InvoicePrice
        {
            get { return invoicePrice; }
            set
            {
                if (value >= 0)
                {
                    invoicePrice = value;
                }
                else
                {
                    throw new ArgumentException("Invoice Price cannot be negative");
                }
            }
        }

        public string Details
        {
            get { return details; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    details = value;
                }
                else
                {
                    throw new ArgumentException("Details cannot be empty");
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
