using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WEGutters
{
    public class InventoryItemDisplay
    {
        //public int InventoryId { get; set; }
        public InventoryItem itemInstance { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemDetails { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public float PurchaseCost { get; set; }
        public float ItemValue { get; set; }
        public float SalePrice { get; set; }
        public float ProjectedSale { get; set; }
        public int Quantity { get; set; }
        public int QtyPerBundle { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int MinCount { get; set; }
        public string LastModified { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
    }
    public class InventoryItem
    {
        private int inventoryID;
        private BaseItem item;
        private string itemDetails;
        private int quantity;
        private int minQuantity;
        private float purchaseCost;
        private float salePrice;
        private string lastModified;
        private string createdDate;

        // Note: itemDetails moved to InventoryItem
        public InventoryItem(BaseItem item, string itemDetails, int quantity, int minQuantity, float purchaseCost, float salePrice, string lastModified, string createdDate)
        {
            Item = item ?? throw new ArgumentException("Item cannot be null");
            ItemDetails = itemDetails ?? string.Empty;
            Quantity = quantity;
            MinQuantity = minQuantity;
            PurchaseCost = purchaseCost;
            SalePrice = salePrice;
            LastModified = lastModified;
            CreatedDate = createdDate;
        }

        // calls the InventoryItemDisplay and de-encapsulates the properties from InventoryItem and BaseItem
        public InventoryItemDisplay ToDisplay()
        {
            if (item == null)
                throw new InvalidOperationException("Cannot convert to display: BaseItem is null.");

            return new InventoryItemDisplay
            {
                ItemName = item.ItemName ?? string.Empty,
                ItemDetails = this.itemDetails ?? string.Empty,
                Category = item.Category?.CategoryName ?? string.Empty,
                SKU = item.SKUProperty?.SKUCode ?? string.Empty,
                PurchaseCost = this.PurchaseCost,
                ItemValue = this.calcItemValue(),
                SalePrice = this.SalePrice,
                ProjectedSale = this.calcProjectedSale(),
                Quantity = this.Quantity,
                QtyPerBundle = item.QuantityPerBundle,
                Unit = item.Unit ?? string.Empty,
                MinCount = this.MinQuantity,
                LastModified = this.LastModified ?? string.Empty,
                CreatedDate = this.CreatedDate ?? string.Empty,
                itemInstance = this
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
                    throw new ArgumentException("Created Date cannot be empty");
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

        // New: ItemDetails property on InventoryItem
        public string ItemDetails
        {
            get { return itemDetails; }
            set
            {
                // Allow empty details; validate if you need non-empty
                itemDetails = value ?? string.Empty;
            }
        }

        public void editInventoryItem(BaseItem newItem, string newItemDetails, string newLastModified, int newQuantity, int newMinQuantity, float purchaseCost, float salePrice)
        {
            Item = newItem;
            ItemDetails = newItemDetails;
            LastModified = newLastModified;
            Quantity = newQuantity;
            MinQuantity = newMinQuantity;
            PurchaseCost = purchaseCost;
            SalePrice = salePrice;

        }

        public float calcProjectedSale()
        {
            return salePrice * quantity;
        }

        public float calcItemValue()
        {
            return purchaseCost * quantity;
        }

        public InventoryItem GetInventoryItem()
        {
            return this;
        }
    }
}
