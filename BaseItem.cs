using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters
{
    internal class BaseItem
    {
        private int itemID;
        private SKU SKU;
        private string SKU_Code;
        private string itemName;
        private string itemDetails;
        private string category;
        private string unit;
        private float purchaseCost;
        private float salePrice;

        public BaseItem(int itemID, SKU SKU, string itemName, string itemDetails, string category, string unit, float purchaseCost, float salePrice)
        {
            this.itemID = itemID;
            this.SKU = SKU;
            this.itemName = itemName;
            this.itemDetails = itemDetails;
            this.category = category;
            this.unit = unit;
            this.purchaseCost = purchaseCost;
            this.salePrice = salePrice;
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
                    SKU_Code = value.SKUCode;
                }
                else
                {
                    throw new ArgumentException("SKU cannot be null");
                }
            }
        }
        public string SKUCode
        {
            get { return SKU.SKUCode; }
        }
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
        public string ItemDetails
        {
            get { return itemDetails; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    itemDetails = value;
                }
                else
                {
                    throw new ArgumentException("Item Details cannot be empty");
                }
            }
        }
        public string Category
        {
            get { return category; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    category = value;
                }
                else
                {
                    throw new ArgumentException("Category cannot be empty");
                }
            }
        }
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
        public float PurchaseCost
        {
            get { return purchaseCost; }
            set
            {
                if (value >= 0)
                {
                    purchaseCost = value;
                }
                else
                {
                    throw new ArgumentException("Purchase Cost cannot be negative");
                }
            }
        }
        public float SalePrice
        {
            get { return salePrice; }
            set
            {
                if (value >= 0)
                {
                    salePrice = value;
                }
                else
                {
                    throw new ArgumentException("Sale Price cannot be negative");
                }
            }
        }

        public BaseItem GetItem()
        {
            return this;
        }

        public void editItem(SKU SKU, string itemName, string itemDetails, string category, string unit, float purchaseCost, float salePrice)
        {
            SKUProperty = SKU;
            ItemName = itemName;
            ItemDetails = itemDetails;
            Category = category;
            Unit = unit;
            PurchaseCost = purchaseCost;
            SalePrice = salePrice;
        }
    }
}