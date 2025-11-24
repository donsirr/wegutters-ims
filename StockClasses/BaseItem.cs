using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters.UserClasses
{
    public class BaseItem
    {
        private int itemID;
        private SKU SKU;
        private string itemName;
        private Category category;
        private string unit;
        private int quantityPerBundle;


        public BaseItem(SKU SKU, string itemName, Category category, string unit, int quantityPerBundle)
        {
            SKUProperty = SKU;
            ItemName = itemName;
            Category = category;
            Unit = unit;
            QuantityPerBundle = quantityPerBundle;
        }
        public int ItemID
        {
            get { return itemID; }
            set
            {
                if (value >= 1)
                {
                    itemID = value;
                }
                else
                {
                    throw new ArgumentException("Item ID cannot be 0 or lower");
                }
            }
        }
        public SKU SKUProperty
        {
            get { return SKU; }
            set
            {
                if (value != null)
                {
                    SKU = value;
                }
                else
                {
                    throw new ArgumentException("SKU cannot be null");
                }
            }
        }
        public int SKUID
        { get { return SKU.SKUID;  } }
        public string SKUCode
        { get { return SKU.SKUCode; } }

        public string ItemName
        {
            get { return itemName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    itemName = value;
                }
                else
                {
                    throw new ArgumentException("Item Name cannot be empty");
                }
            }
        }

        public Category Category
        {
            get { return category; }
            set
            {
                if (value != null)
                {
                    category = value;
                }
                else
                {
                    throw new ArgumentException("Category cannot be empty");
                }
            }
        }
        public int CategoryID
        { get { return Category.CategoryID; } }

        public string CategoryName
        { get { return Category.CategoryName; } }


        public string Unit
        {
            get { return unit; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    unit = value;
                }
                else
                {
                    throw new ArgumentException("Unit cannot be empty");
                }
            }
        }
        public int QuantityPerBundle
        {
            get { return quantityPerBundle; }
            set
            {
                if (value >= 0)
                {
                    quantityPerBundle = value;
                }
                else
                {
                    throw new ArgumentException("Quantity per bundle cannot be negative");
                }
            }
        }
        public BaseItem GetItem()
        {
            return this;
        }

        public void editItem(SKU SKU, string itemName, Category category, string unit)
        {
            SKUProperty = SKU;
            ItemName = itemName;
            Category = category;
            Unit = unit;
        }
    }
}