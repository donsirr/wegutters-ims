using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters
{
    public class ServiceCategory
    {
        private int serviceCategoryID;
        private string serviceCategoryName;

        public ServiceCategory(string serviceName)
        {
            ServiceCategoryName = serviceName;
        }

        public int ServiceCategoryID
        {
            get { return serviceCategoryID; }
            set
            {
                if (value >= 1)
                {
                    serviceCategoryID = value;
                }
                else
                {
                    throw new ArgumentException("Service Category ID cannot be 0 or lower");
                }
            }
        }

        public string ServiceCategoryName
        {
            get { return serviceCategoryName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    serviceCategoryName = value;
                }
                else
                {
                    throw new ArgumentException("Service Name cannot be empty");
                }
            }
        }

    }
}
