using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters
{
    public class InventoryItemDisplay
    {
        public int InventoryId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemDetails { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public float PurchaseCost { get; set; }
        public float SalePrice { get; set; }
        public int Quantity { get; set; }
        public int QtyPerBundle { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int MinCount { get; set; }
        public string LastModified { get; set; } = string.Empty;
    }
    internal class InventoryItem
    {
        private int inventoryID;
        private BaseItem item;
        private string lastModified;
        private int quantity;
        private int minQuantity;

        public InventoryItem(int inventoryID, BaseItem item, string lastModified, int quantity, int minQuantity)
        {
            this.inventoryID = inventoryID;
            this.item = item;
            this.lastModified = lastModified;
            this.quantity = quantity;
            this.minQuantity = minQuantity;
        }

        public InventoryItemDisplay ToDisplay()
        {
            return new InventoryItemDisplay
            {
                InventoryId = this.InventoryId,
                ItemID = this.ItemID,
                ItemName = item.ItemName,
                ItemDetails = item.ItemDetails,
                Category = item.Category,
                SKU = item.SKUProperty.SKUCode,
                PurchaseCost = item.PurchaseCost,
                SalePrice = item.SalePrice,
                Quantity = this.Quantity,
                QtyPerBundle = item.SKUProperty.QuantityPerBundle,
                Unit = item.Unit,
                MinCount = this.MinQuantity,
                LastModified = this.LastModified
            };
        }

        public int InventoryId
        {   get { return inventoryID; }
            set
            {
                if (value >= 1)
                {
                    inventoryID = value;
                }
                else
                {
                    throw new ArgumentException("Inventory ID cannot be 0 or lower");
                }
            }
        }
        public BaseItem Item
        {
            get { return item; }
            set
            {
                if (value != null)
                {
                    item = value;
                }
                else
                {
                    throw new ArgumentException("Item cannot be null");
                }
            }
        }
        public int ItemID
        {
            get { return item.ItemID; }
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
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value >= 0)
                {
                    quantity = value;
                }
                else
                {
                    throw new ArgumentException("Quantity cannot be negative");
                }
            }
        }
        public int MinQuantity
        {
            get { return minQuantity; }
            set
            {
                if (value >= 0)
                {
                    minQuantity = value;
                }
                else
                {
                    throw new ArgumentException("Minimum Quantity cannot be negative");
                }
            }
        }

        public void editInventoryItem(BaseItem newItem, string newLastModified, int newQuantity, int newMinQuantity)
        {
            Item = newItem;
            LastModified = newLastModified;
            Quantity = newQuantity;
            MinQuantity = newMinQuantity;
        }

        public float calcProjectedSale()
        {
            return item.SalePrice * quantity;
        }

        public InventoryItem GetInventoryItem()
        {
            return this;
        }
    }
}
