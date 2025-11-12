using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters
{
    internal class DatabaseAccess
    {

        #region get Collections Methods
        public static ObservableCollection<Category> GetCategories()
        {
            var categoriesCollection = new ObservableCollection<Category>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * From Category;", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var category = new Category(
                                reader.GetString(1) // CategoryName
                            )
                            { CategoryID = reader.GetInt32(0) }; // CategoryID since it's not in the constructor

                            categoriesCollection.Add(category);
                        }
                    }
                }
                return categoriesCollection;
            }
        }

        public static ObservableCollection<SKU> GetSKUs()
        {
            var skusCollection = new ObservableCollection<SKU>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * From SKU;", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sku = new SKU(
                                reader.GetString(1) // SKUCode
                            )
                            { SKUID = reader.GetInt32(0) }; // SKUID since it's not in the constructor

                            skusCollection.Add(sku);
                        }
                    }
                }
                return skusCollection;
            }
        }
        public static ObservableCollection<BaseItem> GetBaseItems()
        {
            var baseItemsCollection = new ObservableCollection<BaseItem>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {

                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                    SELECT
                                                        b.ItemID,
                                                        s.SKU_ID, 
                                                        s.SKU_Code,
                                                        b.ItemName,
                                                        b.ItemDetails,
                                                        c.CategoryID, 
                                                        c.CategoryName,
                                                        b.Unit,
                                                        b.QuantityPerBundle
                                                    FROM BaseItems b
                                                    JOIN SKU s ON b.SKU_ID = s.SKU_ID
                                                    JOIN Category c ON b.CategoryID = c.CategoryID;
                                                    """, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sku = new SKU(reader.GetString(2)) // SKUCode
                            { SKUID = reader.GetInt32(1) }; // SKUID since it's not in the constructor

                            var category = new Category(reader.GetString(6)) // CategoryName
                            { CategoryID = reader.GetInt32(5) }; // CategoryID since it's not in the constructor

                            var baseItem = new BaseItem(
                                sku, // SKUCode
                                reader.GetString(3), // ItemName
                                reader.GetString(4), // ItemDetails
                                category, // CategoryName
                                reader.GetString(7), // Unit
                                reader.GetInt32(8)  // QuantityPerBundle
                            )
                            { ItemID = reader.GetInt32(0) }; // ItemID since it's not in the constructor

                            baseItemsCollection.Add(baseItem);
                        }
                    }
                }
                return baseItemsCollection;
            }
        }
        public static ObservableCollection<InventoryItem> GetInventoryItems()
        {
            var inventoryItemsCollection = new ObservableCollection<InventoryItem>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {

                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                    SELECT

                                                        i.InventoryID,
                                                        i.Quantity,
                                                        i.MinimumQuantity,
                                                        i.PurchaseCost,
                                                        i.SalePrice,
                                                        i.LastModified,
                                                        i.CreatedDate,

                                                        b.ItemID,
                                                        b.ItemName,
                                                        b.ItemDetails,
                                                        b.Unit,
                                                        b.QuantityPerBundle,

                                                        s.SKU_ID,
                                                        s.SKU_Code,

                                                        c.CategoryID,
                                                        c.CategoryName

                                                    FROM Inventory i
                                                    JOIN BaseItems b ON i.ItemID = b.ItemID
                                                    JOIN SKU s ON b.SKU_ID = s.SKU_ID
                                                    JOIN Category c ON b.CategoryID = c.CategoryID;
                                                    """, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sku = new SKU(reader.GetString(13)) // SKUCode
                            { SKUID = reader.GetInt32(12) }; // SKUID since it's not in the constructor

                            var category = new Category(reader.GetString(15)) // CategoryName
                            { CategoryID = reader.GetInt32(14) }; // CategoryID since it's not in the constructor

                            var baseItem = new BaseItem(
                                sku, // SKUCode
                                reader.GetString(8), // ItemName
                                reader.GetString(9), // ItemDetails
                                category, // CategoryName
                                reader.GetString(10), // Unit
                                reader.GetInt32(11)  // QuantityPerBundle
                            )
                            { ItemID = reader.GetInt32(7) }; // ItemID since it's not in the constructor

                            var inventoryItem = new InventoryItem(
                                baseItem,
                                reader.GetInt32(1), // Quantity
                                reader.GetInt32(2), // MinimumQuantity
                                reader.GetFloat(3), // PurchaseCost
                                reader.GetFloat(4), // SalePrice
                                reader.GetString(5), // LastModified
                                reader.GetString(6)  // CreatedDate
                            )
                            { InventoryId = reader.GetInt32(0) }; // InventoryID since it's not in the constructor

                            inventoryItemsCollection.Add(inventoryItem);
                        }
                    }
                }
                return inventoryItemsCollection;
            }
        }

        public static ObservableCollection<InventoryItemDisplay> GetInventoryItemDisplays()
        {
            var inventoryItemsDisplayCollection = new ObservableCollection<InventoryItemDisplay>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {

                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                    SELECT

                                                        i.InventoryID,
                                                        i.Quantity,
                                                        i.MinimumQuantity,
                                                        i.PurchaseCost,
                                                        i.SalePrice,
                                                        i.LastModified,
                                                        i.CreatedDate,

                                                        b.ItemID,
                                                        b.ItemName,
                                                        b.ItemDetails,
                                                        b.Unit,
                                                        b.QuantityPerBundle,

                                                        s.SKU_ID,
                                                        s.SKU_Code,

                                                        c.CategoryID,
                                                        c.CategoryName

                                                    FROM Inventory i
                                                    JOIN BaseItems b ON i.ItemID = b.ItemID
                                                    JOIN SKU s ON b.SKU_ID = s.SKU_ID
                                                    JOIN Category c ON b.CategoryID = c.CategoryID;
                                                    """, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sku = new SKU(reader.GetString(13)) // SKUCode
                            { SKUID = reader.GetInt32(12) }; // SKUID since it's not in the constructor

                            var category = new Category(reader.GetString(15)) // CategoryName
                            { CategoryID = reader.GetInt32(14) }; // CategoryID since it's not in the constructor

                            var baseItem = new BaseItem(
                                sku, // SKUCode
                                reader.GetString(8), // ItemName
                                reader.GetString(9), // ItemDetails
                                category, // CategoryName
                                reader.GetString(10), // Unit
                                reader.GetInt32(11)  // QuantityPerBundle
                            )
                            { ItemID = reader.GetInt32(7) }; // ItemID since it's not in the constructor

                            var inventoryItem = new InventoryItem(
                                baseItem,
                                reader.GetInt32(1), // Quantity
                                reader.GetInt32(2), // MinimumQuantity
                                reader.GetFloat(3), // PurchaseCost
                                reader.GetFloat(4), // SalePrice
                                reader.GetString(5), // LastModified
                                reader.GetString(6)  // CreatedDate
                            )
                            { InventoryId = reader.GetInt32(0) }; // InventoryID since it's not in the constructor

                            inventoryItemsDisplayCollection.Add(inventoryItem.ToDisplay());
                        }
                    }
                }
                return inventoryItemsDisplayCollection;
            }
        }

        #endregion

        #region add Methods
        //returns an int so when we add an item, we don't need to reload to get the ID
        public static int AddCategory(string categoryName)
        {

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO Category (CategoryName) VALUES (@CategoryName);", conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }

        public static int AddSKU(string skuCode)
        {

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO SKU (SKU_Code) VALUES (@SKUCode);", conn))
                {
                    cmd.Parameters.AddWithValue("@SKUCode", skuCode);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }

        public static int AddBaseItem(SKU sku, string itemName, string itemDetails, Category category, string unit, int quantityPerBundle)
        {

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO BaseItems (SKU_ID, ItemName, ItemDetails, CategoryID, Unit, QuantityPerBundle) VALUES (@SKU_ID, @ItemName, @ItemDetails, @CategoryID, @Unit, @QuantityPerBundle);", conn))
                {
                    cmd.Parameters.AddWithValue("@SKU_ID", GetSKUID(sku));
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@ItemDetails", itemDetails);
                    cmd.Parameters.AddWithValue("@CategoryID", GetCategoryID(category));
                    cmd.Parameters.AddWithValue("@Unit", unit);
                    cmd.Parameters.AddWithValue("@QuantityPerBundle", quantityPerBundle);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }

        public static int AddInventoryItem(BaseItem item, int quantity, int minQuantity, float purchaseCost, float salePrice, string lastModified, string createdDate)
        {
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO Inventory (ItemID, Quantity, MinimumQuantity, PurchaseCost, SalePrice, LastModified, CreatedDate) VALUES (@ItemID, @Quantity, @MinimumQuantity, @PurchaseCost, @SalePrice, @LastModified, @CreatedDate);", conn))
                {
                    cmd.Parameters.AddWithValue("@ItemID", GetBaseItemID(item));
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@MinimumQuantity", minQuantity);
                    cmd.Parameters.AddWithValue("@PurchaseCost", purchaseCost);
                    cmd.Parameters.AddWithValue("@SalePrice", salePrice);
                    cmd.Parameters.AddWithValue("@LastModified", lastModified);
                    cmd.Parameters.AddWithValue("@CreatedDate", createdDate);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }
        #endregion

        #region get ID Methods
        public static int GetCategoryID(Category category)
        {
            if (category.CategoryID != 0) { return category.CategoryID; }

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT CategoryID FROM Category WHERE CategoryName = @CategoryName;", conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static int GetSKUID(SKU sku)
        {
            if (sku.SKUID != 0) { return sku.SKUID; }

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT SKU_ID FROM SKU WHERE SKU_Code = @SKUCode;", conn))
                {
                    cmd.Parameters.AddWithValue("@SKUCode", sku.SKUCode);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static int GetBaseItemID(BaseItem item)
        {
            if (item.ItemID != 0) { return item.ItemID; }

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT ItemID FROM BaseItems WHERE ItemName = @ItemName;", conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion

        #region edit Methods
        public static void EditCategory(string categoryName, Category category)
        {
            int id = GetCategoryID(category);

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE Category SET (CategoryName) = @newCategoryName WHERE CategoryID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@newCategoryName", categoryName);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void EditSKU(string skuCode, SKU sku)
        {
            int id = GetSKUID(sku);
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE SKU SET (SKU_Code) = @newSKUCode WHERE SKU_ID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@newSKUCode", skuCode);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void EditBaseItem(BaseItem item, string itemName, string itemDetails, Category category, string unit, int quantityPerBundle)
        {
            int id = GetBaseItemID(item);
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE BaseItems SET (ItemName, ItemDetails, CategoryID, Unit, QuantityPerBundle) = (@newItemName, @newItemDetails, @newCategoryID, @newUnit, @newQuantityPerBundle) WHERE ItemID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@newItemName", itemName);
                    cmd.Parameters.AddWithValue("@newItemDetails", itemDetails);
                    cmd.Parameters.AddWithValue("@newCategoryID", GetCategoryID(category));
                    cmd.Parameters.AddWithValue("@newUnit", unit);
                    cmd.Parameters.AddWithValue("@newQuantityPerBundle", quantityPerBundle);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void EditInventoryItem(InventoryItem inventoryItem, int quantity, int minQuantity, float purchaseCost, float salePrice, string lastModified)
        {
            int id = inventoryItem.InventoryId; // the original item reference to get the ID we edit
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE Inventory SET (Quantity, MinimumQuantity, PurchaseCost, SalePrice, LastModified) = (@newQuantity, @newMinQuantity, @newPurchaseCost, @newSalePrice, @newLastModified) WHERE InventoryID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@newQuantity", quantity);
                    cmd.Parameters.AddWithValue("@newMinQuantity", minQuantity);
                    cmd.Parameters.AddWithValue("@newPurchaseCost", purchaseCost);
                    cmd.Parameters.AddWithValue("@newSalePrice", salePrice);
                    cmd.Parameters.AddWithValue("@newLastModified", lastModified);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        #endregion

        #region delete Methods
        public static void DeleteCategory(Category category)
        {
            int id = GetCategoryID(category);

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Category WHERE CategoryID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteSKU(SKU sku)
        {
            int id = GetSKUID(sku);
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM SKU WHERE SKU_ID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteBaseItem(BaseItem item)
        {
            int id = GetBaseItemID(item);
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM BaseItems WHERE ItemID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteInventoryItem(InventoryItem inventoryItem)
        {
            int id = inventoryItem.InventoryId;
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Inventory WHERE InventoryID = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        #endregion

        #region existence Check Methods

        public static bool CategoryExists(string categoryName)
        {
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Category WHERE CategoryName = @CategoryName;", conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static bool SKUExists(string skuCode)
        {
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM SKU WHERE SKU_Code = @SKUCode;", conn))
                {
                    cmd.Parameters.AddWithValue("@SKUCode", skuCode);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static bool BaseItemExists(SKU sku, string itemName, string itemDetails, Category category, string unit, int qtyPerBundle)
        {
            int skuID = GetSKUID(sku);
            int categoryID = GetCategoryID(category);
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                  SELECT COUNT(*) FROM BaseItems 
                                                  WHERE SKU_ID = @SKUID
                                                  AND ItemName = @ItemName
                                                  AND ItemDetails = @ItemDetails
                                                  AND CategoryID = @CategoryID
                                                  AND Unit = @Unit
                                                  AND QuantityPerBundle = @QtyPerBundle;
                                                  """, conn))
                {
                    cmd.Parameters.AddWithValue("@SKUID", skuID);
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@ItemDetails", itemDetails);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@Unit", unit);
                    cmd.Parameters.AddWithValue("@QtyPerBundle", qtyPerBundle);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        } 

        #endregion
        public static ObservableCollection<InventoryItem> SearchInventory(string search)
        {
            var inventoryItemsCollection = new ObservableCollection<InventoryItem>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {

                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                    SELECT

                                                        i.InventoryID,
                                                        i.Quantity,
                                                        i.MinimumQuantity,
                                                        i.PurchaseCost,
                                                        i.SalePrice,
                                                        i.LastModified,
                                                        i.CreatedDate,

                                                        b.ItemID,
                                                        b.ItemName,
                                                        b.ItemDetails,
                                                        b.Unit,
                                                        b.QuantityPerBundle,

                                                        s.SKU_ID,
                                                        s.SKU_Code,

                                                        c.CategoryID,
                                                        c.CategoryName

                                                    FROM Inventory i
                                                    JOIN BaseItems b ON i.ItemID = b.ItemID
                                                    JOIN SKU s ON b.SKU_ID = s.SKU_ID
                                                    JOIN Category c ON b.CategoryID = c.CategoryID
                                                    WHERE b.ItemName LIKE @Search OR s.SKUCode LIKE @Search OR c.CategoryName LIKE @Search;
                                                    """, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sku = new SKU(reader.GetString(13)) // SKUCode
                            { SKUID = reader.GetInt32(12) }; // SKUID since it's not in the constructor

                            var category = new Category(reader.GetString(15)) // CategoryName
                            { CategoryID = reader.GetInt32(14) }; // CategoryID since it's not in the constructor

                            var baseItem = new BaseItem(
                                sku, // SKUCode
                                reader.GetString(8), // ItemName
                                reader.GetString(9), // ItemDetails
                                category, // CategoryName
                                reader.GetString(10), // Unit
                                reader.GetInt32(11)  // QuantityPerBundle
                            )
                            { ItemID = reader.GetInt32(7) }; // ItemID since it's not in the constructor

                            var inventoryItem = new InventoryItem(
                                baseItem,
                                reader.GetInt32(1), // Quantity
                                reader.GetInt32(2), // MinimumQuantity
                                reader.GetFloat(3), // PurchaseCost
                                reader.GetFloat(4), // SalePrice
                                reader.GetString(5), // LastModified
                                reader.GetString(6)  // CreatedDate
                            )
                            { InventoryId = reader.GetInt32(0) }; // InventoryID since it's not in the constructor
                        }
                    }
                }
                return inventoryItemsCollection;
            }
        }


    }
}