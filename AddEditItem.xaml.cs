using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            //updateCategoryComboBox();
            //updateSKUComboBox();
            CategoryCollection.Add(new Category("Add New Category"));
            CategoryCollection.Add(new Category("123"));
            CategoryCollection.Add(new Category("456"));
            SKUCollection.Add(new SKU("Add New SKU"));
            SKUCollection.Add(new SKU("Otherrrr"));
            SKU skutest = new SKU("TestSKU");
            Category categtest = new Category("TestCategory");
            CategoryCollection.Add(categtest);
            SKUCollection.Add(skutest);
            BaseItemCollection.Add(new BaseItem(null, "Add New Item", null, null, null, 0));
            BaseItemCollection.Add(new BaseItem(skutest, "Item1", "test", categtest  , "tes", 0));
            this.isNew = isNew;
            ReturnItem = currentItem;
            DataContext = this;
            if (!isNew)
            {
                setEditValues();
            }
        }
        private void setEditValues()
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

                ItemDetailsBox.Text = ReturnItem.Item.ItemDetails;
                UnitBox.Text = ReturnItem.Item.Unit;
                QuantityPerBundleBox.Text = ReturnItem.Item.QuantityPerBundle.ToString();

                QuantityBox.Text = ReturnItem.Quantity.ToString();
                MinimumQuantityBox.Text = ReturnItem.MinQuantity.ToString();
                PurchaseCostBox.Text = ReturnItem.PurchaseCost.ToString();
                SalePriceBox.Text = ReturnItem.SalePrice.ToString();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private void updateCategoryComboBox(ObservableCollection<Category> Categories)
        {
            CategoryCollection = Categories;
        }
        private void updateSKUComboBox(ObservableCollection<SKU> SKUs)
        {
            SKUCollection = SKUs;
        }
        private void MakeBaseItemButton_Click(object sender, RoutedEventArgs e)
        {
            // open DialogueBox and set Combobox and add to BaseItem database/collection
            //updateItemNameComboBox();
        }

        private void MakeSKUButton_Click(object sender, RoutedEventArgs e)
        {
            // open DialogueBox and set Combobox and add to SKU database/collection
            //updateSKUComboBox();
        }

        private void MakeCategoryButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                //set date to now
                createdDate = DateTime.Now.ToString("yyyy-MM-dd_HH:mm");
            }
            else
            {
                //keep original created date
                createdDate = ReturnItem.CreatedDate;
            }
            InventoryItem inventoryItem = new InventoryItem(
                getSelectedBaseItem(),
                Convert.ToInt32(QuantityBox.Text),
                Convert.ToInt32(MinimumQuantityBox.Text),
                float.Parse(PurchaseCostBox.Text),
                float.Parse(SalePriceBox.Text),
                DateTime.Now.ToString("yyyy-MM-dd_HH:mm"),
                createdDate);
            ReturnItem = inventoryItem;
            this.DialogResult = true; // This tells the MainWindow that we saved.
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window.
            this.DialogResult = false; // This tells the MainWindow that we cancelled.
            this.Close();
        }

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
                    ItemDetailsBox.Text = "";
                    CategoryComboBox.SelectedIndex = 0;
                    SKUComboBox.SelectedIndex = 0;
                    UnitBox.Text = "";
                    QuantityPerBundleBox.Text = "";
            }
                else
                {
                    ItemNameComboBox.IsEditable = false;
                    ItemDetailsBox.Text = (ItemNameComboBox.SelectedItem as BaseItem).ItemDetails;
                    CategoryComboBox.SelectedItem = (ItemNameComboBox.SelectedItem as BaseItem).Category;
                    SKUComboBox.SelectedItem = (ItemNameComboBox.SelectedItem as BaseItem).SKUProperty;
                    UnitBox.Text = (ItemNameComboBox.SelectedItem as BaseItem).Unit;
                    QuantityPerBundleBox.Text = (ItemNameComboBox.SelectedItem as BaseItem).QuantityPerBundle.ToString();
            }
        }


        private SKU getSelectedSKU()
        {
            if (SKUComboBox.SelectedIndex == 0)
            {
                // create new SKU
                SKU sku = new SKU(SKUComboBox.Text);
                SKUCollection.Add(sku);
                SKUComboBox.SelectedItem = sku;
                return sku;

            }
            else
            {
                return SKUComboBox.SelectedItem as SKU;
            }
        }

        private Category getSelectedCategory()
        {
            if (CategoryComboBox.SelectedIndex == 0)
            {
                // create new SKU
                Category categ = new Category(CategoryComboBox.Text);
                CategoryCollection.Add(categ);
                CategoryComboBox.SelectedItem = categ;
                return categ;
            }
            else
            {
                return CategoryComboBox.SelectedItem as Category;
            }
        }

        private BaseItem getSelectedBaseItem()
        {
            if (ItemNameComboBox.SelectedIndex == 0)
            {
                // create new BaseItem
                SKU sku = getSelectedSKU();
                Category category = getSelectedCategory();
                BaseItem baseItem = new BaseItem(
                    sku,
                    ItemNameComboBox.Text,
                    ItemDetailsBox.Text,
                    category,
                    UnitBox.Text,
                    Convert.ToInt32(QuantityPerBundleBox.Text));
                MessageBox.Show(category.CategoryName + sku.SKUCode + baseItem.ItemName);
                return baseItem;
            }
            else
            {
                return ItemNameComboBox.SelectedItem as BaseItem;
            }
        }
    }
}
