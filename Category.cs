using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters
{
    public class Category
    {
        private int categoryID;
        private string categoryName;

        public Category(string categoryName)
        {
            //categoryID = categoryID;
            CategoryName = categoryName;
        }
        public int CategoryID
        {
            get { return categoryID; }
            set
            {
                if (value >= 1)
                {
                    categoryID = value;
                }
                else
                {
                    throw new ArgumentException("Category ID cannot be 0 or lower");
                }
            }
        }
        public string CategoryName
        {   
            get { return categoryName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    categoryName = value;
                }
                else
                {
                    throw new ArgumentException("Category Name cannot be empty");
                }
            }
        }
        public Category GetCategory()
        {
            return this;
        }
    }
}
