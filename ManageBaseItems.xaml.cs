using DocumentFormat.OpenXml.Vml.Office;
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
using WEGutters.DatabaseAccess;
using WEGutters.UserClasses;

namespace WEGutters
{
    /// <summary>
    /// Interaction logic for ManageBaseItems.xaml
    /// </summary>
    public partial class ManageBaseItems : Window
    {

        private bool isNew = true;
        public ObservableCollection<Category> CategoryCollection { get; set; } = new ObservableCollection<Category>();
        public ObservableCollection<SKU> SKUCollection { get; set; } = new ObservableCollection<SKU>();
        public BaseItem ReturnBase { get; set; }

        public ManageBaseItems(bool isNew, BaseItem currentBase = null)
        {
            InitializeComponent();

            UpdateCategoryComboBox(StockDBAccess.GetCategories());
            UpdateSKUComboBox(StockDBAccess.GetSKUs());

            this.isNew = isNew;
            ReturnBase = currentBase;
            DataContext = this;
            if (!isNew)
            {
                SetEditValues();
            }
        }

        private void SetEditValues()
        {
            if (ReturnBase == null) return;

                ItemNameBox.Text = ReturnBase.ItemName;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Ensure CategoryCollection has matching Category instance
                    var catName = ReturnBase.Category?.CategoryName ?? string.Empty;
                    var catMatch = CategoryCollection.FirstOrDefault(c => c.CategoryName == catName);
                    if (catMatch == null && ReturnBase.Category != null)
                    {
                        CategoryCollection.Add(ReturnBase.Category);
                        catMatch = ReturnBase.Category;
                    }
                    CategoryComboBox.SelectedItem = catMatch ?? CategoryCollection.FirstOrDefault();

                    // Ensure SKUCollection has matching SKU instance
                    var skuCode = ReturnBase.SKUProperty?.SKUCode ?? string.Empty;
                    var skuMatch = SKUCollection.FirstOrDefault(s => s.SKUCode == skuCode);
                    if (skuMatch == null && ReturnBase.SKUProperty != null)
                    {
                        SKUCollection.Add(ReturnBase.SKUProperty);
                        skuMatch = ReturnBase.SKUProperty;
                    }
                    SKUComboBox.SelectedItem = skuMatch ?? SKUCollection.FirstOrDefault();
                }), System.Windows.Threading.DispatcherPriority.Loaded);

                UnitBox.Text = ReturnBase.Unit;
                QuantityPerBundleBox.Text = ReturnBase.QuantityPerBundle.ToString();

        }

        private void UpdateCategoryComboBox(ObservableCollection<Category> Categories)
        {
            CategoryCollection = Categories;
            CategoryCollection.Insert(0, (new Category("Add New Category")));
        }
        private void UpdateSKUComboBox(ObservableCollection<SKU> SKUs)
        {
            SKUCollection = SKUs;
            SKUCollection.Insert(0, (new SKU("Add New SKU")));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs before saving
            if (!ValidateInputs(out string validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                ReturnBase = GetBaseItem();

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
        private BaseItem GetBaseItem()
        {
            if (isNew && (StockDBAccess.BaseItemExists(ItemNameBox.Text)))
            {
                throw new InvalidOperationException(
                    $"Item with the name '{ItemNameBox.Text}' already exists."
                );
            }
            else // gets base item information
            {
                SKU sku = GetSelectedSKU();
                Category category = GetSelectedCategory();
                BaseItem baseItem = new BaseItem(
                    sku,
                    ItemNameBox.Text,
                    category,
                    UnitBox.Text,
                    Convert.ToInt32(QuantityPerBundleBox.Text));
                return baseItem;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window.
            this.DialogResult = false; // This tells the MainWindow that we cancelled.
            this.Close();
        }

        private bool ValidateInputs(out string validationError)
        {
            var errors = new StringBuilder();

            // Item name
            var itemName = (ItemNameBox.Text ?? string.Empty).Trim();
            ItemNameBox.Text = itemName;
            if (string.IsNullOrEmpty(itemName))
            {
                errors.AppendLine("- Item Name cannot be blank.");
            }
            else if (string.Equals(itemName, "Add New Base Item", StringComparison.OrdinalIgnoreCase))
            {
                errors.AppendLine("- Item Name cannot be \"Add New Base Item\".");
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
                errors.AppendLine("- Unit cannot be blank.");
            }

            validationError = errors.ToString().TrimEnd();
            return validationError.Length == 0;
        }


        // so they can still add new SKUs and Categories from this window
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
    }
}
