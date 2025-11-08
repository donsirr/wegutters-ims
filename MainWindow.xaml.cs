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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WEGutters
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<InventoryItemDisplay> InventoryList { get; set; }
        public MainWindow()
        {            
            InitializeComponent();
            InventoryList = new ObservableCollection<InventoryItemDisplay>();
            InventoryList.Add(testItem());
            this.DataContext = this;
        }
        #region Stock Button Functionality

        private InventoryItemDisplay testItem()
        {
            SKU sKU = new SKU(1, "GUT-AL-5", "Aluminum Gutter SKU", 1);
            BaseItem baseItem = new BaseItem(1, sKU,"Aluminum Gutter", "Amazing", "Gutter","Pc", 5.00f, 10.00f);
            InventoryItem inventoryItem = new InventoryItem(1, baseItem, DateTime.Now.ToString("yyyy-MM-dd"), 100, 10);
            InventoryItemDisplay inventoryItemDisplay = inventoryItem.ToDisplay();
            return inventoryItemDisplay;
        }

        public void AddTestRow()
        {
            // use unique id based on current count to avoid duplicates
            int id = InventoryList.Count + 1;
            SKU sKU = new SKU(id, $"TEST-SKU-{id}", "Test SKU", 1);
            BaseItem baseItem = new BaseItem(id, sKU, $"Test Item {id}", "Default test item", "TestCategory", "Pc", 1.00f, 2.50f);
            InventoryItem inventoryItem = new InventoryItem(id, baseItem, DateTime.Now.ToString("yyyy-MM-dd"), 25, 5);
            InventoryList.Add(inventoryItem.ToDisplay());
        }
        public void testAdd()
        {
            SKU sKU = new SKU(1, "GUT-AL-5", "Aluminum Gutter SKU", 1);
            BaseItem baseItem = new BaseItem(1, sKU, "Aluminum Gutter", "Amazing", "Gutter", "Pc", 5.00f, 10.00f);
            InventoryItem inventoryItem = new InventoryItem(1, baseItem, DateTime.Now.ToString("yyyy-MM-dd"), 100, 10);
            InventoryItemDisplay inventoryItemDisplay = inventoryItem.ToDisplay();
            InventoryList.Add(inventoryItemDisplay);
        }

        private void Stock_NewItem_Click(object sender, RoutedEventArgs e)
        {
            AddTestRow();
            // TODO: Add logic here to:
            // 1. Add Item.
            // 2. Update the Stock DataGrid's ItemsSource.
            AddEditServiceWindow addServiceWindow = new AddEditServiceWindow();
            addServiceWindow.Owner = this; // Set this window as the owner
            addServiceWindow.Title = "Add New Service";

            // ShowDialog() opens the window and pauses code here until the user closes it
            bool? result = addServiceWindow.ShowDialog();

            // Check if the user clicked "Save"
            if (result == true)
            {
                // If they saved, add item to the data grid
               
            }
            MessageBox.Show("Add Item... (functionality to be added)");
        }
        private void Stock_EditItem_Click(object sender, RoutedEventArgs e)
        {
            testAdd();
            // TODO: Add logic here to:
            // 1. Edit Item.
            // 2. Update the Stock DataGrid's ItemsSource.

            MessageBox.Show("Edit Item... (functionality to be added)");
        }
        private void Stock_DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic here to:
            // 1. Remove Item.
            // 2. Update the Stock DataGrid's ItemsSource.

            MessageBox.Show("Delete Item... (functionality to be added)");
        }

        // This method will be called to refresh the data in the Stock DataGrid
        private void Stock_Refresh_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic here to:
            // 1. Re-load the inventory data from your database.
            // 2. Update the Stock DataGrid's ItemsSource.

            MessageBox.Show("Refresh Stock Data... (functionality to be added)");
        }

        // This method will handle printing the DataGrid
        private void Stock_Print_Click(object sender, RoutedEventArgs e)
        {
            // Printing is an advanced feature.
            // You will typically use the System.Windows.Controls.PrintDialog class.

            MessageBox.Show("Print Stock Report... (functionality to be added)");
        }

        // This method will save the grid as a PDF
        private void Stock_SaveAsPdf_Click(object sender, RoutedEventArgs e)
        {
            // PDF generation requires a third-party library (a "NuGet package")
            // such as "PdfSharp" or "iTextSharp".

            MessageBox.Show("Save as PDF... (requires PDF library)");
        }

        // This method will save the grid as an Excel file
        private void Stock_Excel_Click(object sender, RoutedEventArgs e)
        {
            // Excel generation requires a third-party library (a "NuGet package")
            // such as "EPPlus".

            MessageBox.Show("Export to Excel... (requires Excel library)");
        }

        // This method will open the specific "Inventory Count Report"
        private void Stock_InventoryReport_Click(object sender, RoutedEventArgs e)
        {
            // This will likely open a new window or switch to the Reporting tab
            // with a specific report selected.

            MessageBox.Show("Open Inventory Count Report... (functionality to be added)");
        }

        #endregion

        #region Services Button Functionality

        private void Services_Refresh_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic to refresh the Services DataGrid
            MessageBox.Show("Refresh Services Data... (functionality to be added)");
        }

        private void Services_NewService_Click(object sender, RoutedEventArgs e)
        {
            AddEditServiceWindow addServiceWindow = new AddEditServiceWindow();
            addServiceWindow.Owner = this; // Set this window as the owner
            addServiceWindow.Title = "Add New Service";

            // ShowDialog() opens the window and pauses code here until the user closes it
            bool? result = addServiceWindow.ShowDialog();

            // Check if the user clicked "Save"
            if (result == true)
            {
                // If they saved, refresh the data grid
                Services_Refresh_Click(null, null);
            }
        }

        private void Services_EditService_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            // 1. Get the selected item from the Services DataGrid
            //    (You'll need to give your DataGrid a name in the template)
            // 2. If nothing is selected, show a message and return.

            // Example:
            // if (myServicesDataGrid.SelectedItem == null)
            // {
            //     MessageBox.Show("Please select a service to edit.");
            //     return;
            // }
            // var selectedService = (YourServiceClass)myServicesDataGrid.SelectedItem;


            AddEditServiceWindow editServiceWindow = new AddEditServiceWindow();
            editServiceWindow.Owner = this;
            editServiceWindow.Title = "Edit Service";

            // TODO:
            // 3. Pre-load the window's textboxes with the selected item's data
            //    editServiceWindow.ItemNameBox.Text = selectedService.Name;
            //    editServiceWindow.CategoryBox.Text = selectedService.Category;
            //    ...etc.

            bool? result = editServiceWindow.ShowDialog();

            if (result == true)
            {
                // If they saved, refresh the data grid
                Services_Refresh_Click(null, null);
            }
        }

        private void Services_DeleteService_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            // 1. Get the selected item from the Services DataGrid
            // 2. If nothing is selected, return.

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this service/product?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // TODO:
                // 3. Delete the item from your database
                // 4. Refresh the grid
                Services_Refresh_Click(null, null);
            }
        }

        private void Services_Print_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print... (functionality to be added)");
        }

        private void Services_SaveAsPdf_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Save as PDF... (functionality to be added)");
        }

        private void Services_PriceTags_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Price Tags... (functionality to be added)");
        }

        private void Services_Sorting_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sorting... (functionality to be added)");
        }

        #endregion
    }
}