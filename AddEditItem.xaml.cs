using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WEGutters.ConstructorClasses;

namespace WEGutters
{
    /// <summary>
    /// Interaction logic for AddEditItem.xaml
    /// </summary>
    public partial class AddEditItem : Window
    {
        private bool isNew = true;
        private string createdDate;
        public ObservableCollection<Category> CategoryCollection { get; set; } = new ObservableCollection<Category>();
        public ObservableCollection<SKU> SKUCollection { get; set; } = new ObservableCollection<SKU>();
        public ObservableCollection<BaseItem> BaseItemCollection { get; set; } = new ObservableCollection<BaseItem>();
        public InventoryItem ReturnItem { get; set; }


        public AddEditItem(bool isNew, InventoryItem currentItem = null)
        {
            InitializeComponent();

            UpdateCategoryComboBox(StockDBAccess.GetCategories());
            UpdateSKUComboBox(StockDBAccess.GetSKUs());
            UpdateBaseItemComboBox(StockDBAccess.GetBaseItems());

            this.isNew = isNew;
            ReturnItem = currentItem;
            DataContext = this;
            if (!isNew)
            {
                SetEditValues();
            }
        }

        // Sets the input fields' data
        private void SetEditValues()
        {
            if (ReturnItem == null || ReturnItem.Item == null) return;

            // Try to find a matching BaseItem already in the ItemsSource (by reference or by name+SKU code)
            var baseMatch = BaseItemCollection.FirstOrDefault(b =>
                object.ReferenceEquals(b, ReturnItem.Item) ||
                (!string.IsNullOrEmpty(b.ItemName) && !string.IsNullOrEmpty(ReturnItem.Item.ItemName)
                    && b.ItemName == ReturnItem.Item.ItemName
                    && ((b.SKUProperty?.SKUCode ?? string.Empty) == (ReturnItem.Item.SKUProperty?.SKUCode ?? string.Empty)))
            );

            if (baseMatch == null)
            {
                // ensure the ItemsSource contains the instance we want to select
                BaseItemCollection.Add(ReturnItem.Item);
                baseMatch = ReturnItem.Item;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Select the BaseItem that belongs to the ItemsSource
                ItemNameComboBox.SelectedItem = baseMatch;

                // Ensure CategoryCollection has matching Category instance
                var catName = ReturnItem.Item.Category?.CategoryName ?? string.Empty;
                var catMatch = CategoryCollection.FirstOrDefault(c => c.CategoryName == catName);
                if (catMatch == null && ReturnItem.Item.Category != null)
                {
                    CategoryCollection.Add(ReturnItem.Item.Category);
                    catMatch = ReturnItem.Item.Category;
                }
                CategoryComboBox.SelectedItem = catMatch ?? CategoryCollection.FirstOrDefault();

                // Ensure SKUCollection has matching SKU instance
                var skuCode = ReturnItem.Item.SKUProperty?.SKUCode ?? string.Empty;
                var skuMatch = SKUCollection.FirstOrDefault(s => s.SKUCode == skuCode);
                if (skuMatch == null && ReturnItem.Item.SKUProperty != null)
                {
                    SKUCollection.Add(ReturnItem.Item.SKUProperty);
                    skuMatch = ReturnItem.Item.SKUProperty;
                }
                SKUComboBox.SelectedItem = skuMatch ?? SKUCollection.FirstOrDefault();

                ItemDetailsBox.Text = ReturnItem.ItemDetails;
                UnitBox.Text = ReturnItem.Item.Unit;
                QuantityPerBundleBox.Text = ReturnItem.Item.QuantityPerBundle.ToString();

                QuantityBox.Text = ReturnItem.Quantity.ToString();
                MinimumQuantityBox.Text = ReturnItem.MinQuantity.ToString();
                PurchaseCostBox.Text = ReturnItem.PurchaseCost.ToString();
                SalePriceBox.Text = ReturnItem.SalePrice.ToString();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private void UpdateCategoryComboBox(ObservableCollection<Category> Categories)
        {
            CategoryCollection = Categories;
            CategoryCollection.Insert(0,(new Category("Add New Category")));
        }
        private void UpdateSKUComboBox(ObservableCollection<SKU> SKUs)
        {
            SKUCollection = SKUs;
            SKUCollection.Insert(0,(new SKU("Add New SKU")));
        }
        private void UpdateBaseItemComboBox(ObservableCollection<BaseItem> BaseItems)
        {
            BaseItemCollection = BaseItems;
            BaseItemCollection.Insert(0,(new BaseItem(new SKU("null"), "Add New Item", new Category("null"), "null", 1)));
        }

        // Save button click event
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
                createdDate = ReturnItem.CreatedDate;
            }

            try
            {
                // If editing, update the existing instance in-place so its InventoryId remains intact.
                if (!isNew && ReturnItem != null)
                {
                    ReturnItem.Item = GetSelectedBaseItem();
                    ReturnItem.ItemDetails = ItemDetailsBox.Text;
                    ReturnItem.Quantity = Convert.ToInt32(QuantityBox.Text);
                    ReturnItem.MinQuantity = Convert.ToInt32(MinimumQuantityBox.Text);
                    ReturnItem.PurchaseCost = float.Parse(PurchaseCostBox.Text, CultureInfo.InvariantCulture);
                    ReturnItem.SalePrice = float.Parse(SalePriceBox.Text, CultureInfo.InvariantCulture);
                    ReturnItem.LastModified = DateTime.Now.ToString("yyyy-MM-dd_HH:mm");
                    // CreatedDate remains as-is (createdDate variable preserves it)
                }
                else
                {
                    // New item: create new instance (InventoryId will be assigned by DB on insert)
                    InventoryItem inventoryItem = new InventoryItem(
                        GetSelectedBaseItem(),
                        ItemDetailsBox.Text,
                        Convert.ToInt32(QuantityBox.Text),
                        Convert.ToInt32(MinimumQuantityBox.Text),
                        float.Parse(PurchaseCostBox.Text, CultureInfo.InvariantCulture),
                        float.Parse(SalePriceBox.Text, CultureInfo.InvariantCulture),
                        DateTime.Now.ToString("yyyy-MM-dd_HH:mm"),
                        createdDate);
                    ReturnItem = inventoryItem;
                }

                this.DialogResult = true; // tell caller we saved
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private SKU GetSelectedSKU()
        {
            if (SKUComboBox.SelectedIndex == 0 && !(StockDBAccess.SKUExists(SKUComboBox.Text)))
            {
                // create new SKU
                SKU sku = new SKU(SKUComboBox.Text);
                sku.SKUID = StockDBAccess.AddSKU(SKUComboBox.Text);
                SKUCollection.Add(sku);
                SKUComboBox.SelectedItem = sku;
                return sku;
            }
            else if (SKUComboBox.SelectedIndex == 0 && (StockDBAccess.SKUExists(SKUComboBox.Text)))
            {
                // get existing SKU
                var sku = SKUCollection.FirstOrDefault(s => s.SKUCode == SKUComboBox.Text);
                return sku;
            }
            else
            {
                return SKUComboBox.SelectedItem as SKU;
            }
        }
        private Category GetSelectedCategory()
        {
            if (CategoryComboBox.SelectedIndex == 0 && !(StockDBAccess.CategoryExists(CategoryComboBox.Text)))
            {
                // create new SKU
                Category categ = new Category(CategoryComboBox.Text);
                categ.CategoryID = StockDBAccess.AddCategory(CategoryComboBox.Text);
                CategoryCollection.Add(categ);
                CategoryComboBox.SelectedItem = categ;
                return categ;
            }
            else if (CategoryComboBox.SelectedIndex == 0 && (StockDBAccess.CategoryExists(CategoryComboBox.Text)))
            {
                // get existing category
                var categ = CategoryCollection.FirstOrDefault(c => c.CategoryName == CategoryComboBox.Text);
                return categ;
            }
            else
            {
                return CategoryComboBox.SelectedItem as Category;
            }
        }
        private BaseItem GetSelectedBaseItem()
        {
            if (ItemNameComboBox.SelectedIndex == 0 && (StockDBAccess.BaseItemExists(ItemNameComboBox.Text)))
            {
                throw new InvalidOperationException(
                    $"Item with the name '{ItemNameComboBox.Text}' already exists."
                );
            }
            else if (ItemNameComboBox.SelectedIndex == 0 && !(StockDBAccess.BaseItemExists(ItemNameComboBox.Text)))
            { 
                SKU sku = GetSelectedSKU();
                Category category = GetSelectedCategory();
                BaseItem baseItem = new BaseItem(
                    sku,
                    ItemNameComboBox.Text,
                    category,
                    UnitBox.Text,
                    Convert.ToInt32(QuantityPerBundleBox.Text));
                baseItem.ItemID = StockDBAccess.AddBaseItem(
                        sku,
                        ItemNameComboBox.Text,
                        category,
                        UnitBox.Text,
                        Convert.ToInt32(QuantityPerBundleBox.Text));
                return baseItem;
            }
            else
            {
                return ItemNameComboBox.SelectedItem as BaseItem;
            }
        }

        // Cancel button click event
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window.
            this.DialogResult = false; // This tells the MainWindow that we cancelled.
            this.Close();
        }

        // Validation
        private bool ValidateInputs(out string validationError)
        {
            var errors = new StringBuilder();

            // Item name
            string itemName;
            if (ItemNameComboBox.SelectedIndex == 0)
            {
                itemName = (ItemNameComboBox.Text ?? string.Empty).Trim();
                ItemNameComboBox.Text = itemName; // normalize
            }
            else
            {
                itemName = (ItemNameComboBox.SelectedItem as BaseItem)?.ItemName?.Trim() ?? string.Empty;
            }
            if (string.IsNullOrEmpty(itemName))
            {
                errors.AppendLine("- Item Name cannot be blank.");
            }
            else if (string.Equals(itemName, "Add New Item", StringComparison.OrdinalIgnoreCase))
            {
                errors.AppendLine("- Item Name cannot be \"Add New Item\".");
            }

            // Item details
            var itemDetails = (ItemDetailsBox.Text ?? string.Empty).Trim();
            ItemDetailsBox.Text = itemDetails;
            if (string.IsNullOrEmpty(itemDetails))
            {
                errors.AppendLine("- Item Details cannot be blank.");
            }

            // Category
            string categoryName;
            if (CategoryComboBox.SelectedIndex == 0)
            {
                categoryName = (CategoryComboBox.Text ?? string.Empty).Trim();
                CategoryComboBox.Text = categoryName;
            }
            else
            {
                categoryName = (CategoryComboBox.SelectedItem as Category)?.CategoryName?.Trim() ?? string.Empty;
            }
            if (string.IsNullOrEmpty(categoryName))
            {
                errors.AppendLine("- Category cannot be blank.");
            }
            else if (string.Equals(categoryName, "Add New Category", StringComparison.OrdinalIgnoreCase))
            {
                errors.AppendLine("- Category cannot be \"Add New Category\".");
            }

            // SKU
            string skuText;
            if (SKUComboBox.SelectedIndex == 0)
            {
                skuText = (SKUComboBox.Text ?? string.Empty).Trim();
                SKUComboBox.Text = skuText;
            }
            else
            {
                skuText = (SKUComboBox.SelectedItem as SKU)?.SKUCode?.Trim() ?? string.Empty;
            }
            if (string.IsNullOrEmpty(skuText))
            {
                errors.AppendLine("- SKU cannot be blank.");
            }
            else if (string.Equals(skuText, "Add New SKU", StringComparison.OrdinalIgnoreCase))
            {
                errors.AppendLine("- SKU cannot be \"Add New SKU\".");
            }

            // Quantity Per Bundle
            var qtyPerBundleText = (QuantityPerBundleBox.Text ?? string.Empty).Trim();
            QuantityPerBundleBox.Text = qtyPerBundleText;
            if (!int.TryParse(qtyPerBundleText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int qtyPerBundle) || qtyPerBundle <= 0)
            {
                errors.AppendLine("- Qty Per Bundle cannot be 0 or less than 0.");
            }

            // Units
            var unitText = (UnitBox.Text ?? string.Empty).Trim();
            UnitBox.Text = unitText;
            if (string.IsNullOrEmpty(unitText))
            {
                errors.AppendLine("- Units cannot be blank.");
            }

            // Quantity
            var quantityText = (QuantityBox.Text ?? string.Empty).Trim();
            QuantityBox.Text = quantityText;
            if (!int.TryParse(quantityText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                errors.AppendLine("- Quantity must be an number and cannot be blank.");
            }

            // Minimum Quantity
            var minQuantityText = (MinimumQuantityBox.Text ?? string.Empty).Trim();
            MinimumQuantityBox.Text = minQuantityText;
            if (!int.TryParse(minQuantityText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                errors.AppendLine("- Minimum Quantity must be an number and cannot be blank.");
            }

            // Purchase cost
            var purchaseCostText = (PurchaseCostBox.Text ?? string.Empty).Trim();
            PurchaseCostBox.Text = purchaseCostText;
            if (!float.TryParse(purchaseCostText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out _))
            {
                errors.AppendLine("- Purchase Cost must be a number.");
            }

            // Sale price
            var salePriceText = (SalePriceBox.Text ?? string.Empty).Trim();
            SalePriceBox.Text = salePriceText;
            if (!float.TryParse(salePriceText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out _))
            {
                errors.AppendLine("- Sale Price must be a number.");
            }

            validationError = errors.ToString().TrimEnd();
            return validationError.Length == 0;
        }

        // ComboBox SelectionChanged events
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex == 0)
            {
                comboBox.IsEditable = true;
            }
            else
            {
                comboBox.IsEditable = false;
            }
        }

        private void ItemNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemNameComboBox.SelectedIndex == 0)
            {
                ItemNameComboBox.IsEditable = true;

                CategoryComboBox.SelectedIndex = 0;
                CategoryComboBox.IsEnabled = true;
                CategoryLabel.Opacity = 1;

                SKUComboBox.SelectedIndex = 0;
                SKUComboBox.IsEnabled = true;
                SKULabel.Opacity = 1;

                QuantityPerBundleBox.Text = "";
                QuantityPerBundleBox.IsEnabled = true;
                QuantityPerBundleLabel.Opacity = 1;

                UnitBox.Text = "";
                UnitBox.IsEnabled = true;
                UnitLabel.Opacity = 1;
            }
            else
            {
                ItemNameComboBox.IsEditable = false;

                // gets the matching object by ID
                var matchingCategory = CategoryCollection.FirstOrDefault(c => c.CategoryID == (ItemNameComboBox.SelectedItem as BaseItem).Category.CategoryID);
                CategoryComboBox.SelectedItem = matchingCategory;
                CategoryComboBox.IsEnabled = false;
                CategoryLabel.Opacity = 0.25;

                // gets the matching object by ID
                var matchingSKU = SKUCollection.FirstOrDefault(s => s.SKUID == (ItemNameComboBox.SelectedItem as BaseItem).SKUProperty.SKUID);
                SKUComboBox.SelectedItem = matchingSKU;
                SKUComboBox.IsEnabled = false;
                SKULabel.Opacity = 0.25;

                QuantityPerBundleBox.Text = (ItemNameComboBox.SelectedItem as BaseItem).QuantityPerBundle.ToString();
                QuantityPerBundleBox.IsEnabled = false;
                QuantityPerBundleLabel.Opacity = 0.25;

                UnitBox.Text = (ItemNameComboBox.SelectedItem as BaseItem).Unit;
                UnitBox.IsEnabled = false;
                UnitLabel.Opacity = 0.25;
            }
        }


    }
}
