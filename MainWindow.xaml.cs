using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
using WEGutters.ConstructorClasses;

namespace WEGutters
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region observable collection for customers
        private ObservableCollection<Customer> _customerList;

        public ObservableCollection<Customer> CustomerList
        {
            get => _customerList;
            set
            {
                // commented out as we dont use customers for the dashboard currently
                //if (_customerList != null)
                //    _customerList.CollectionChange -= CustomerList_CollectionChanged;

                //_customerList = value;

                //is (_customerList != null)
                //_customerList.CollectionChanged += CustomerList_CollectionChanged;

                //OnPropertyChanged(nameof(CustomerList));
                //UpdateDashboard();
            }
        }

        //private void ServiceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    UpdateDashboard();
        //}
        #endregion


        #region observable collection for services
        private ObservableCollection<Service> _serviceList;

        public ObservableCollection<Service> ServiceList
        {
            get => _serviceList;
            set
            {
                // commented out as we dont use services for the dashboard currently
                //if (_serviceList != null)
                //    _serviceList.CollectionChange -= ServiceList_CollectionChanged;

                //_serviceList = value;

                //is (_serviceList != null)
                //_serviceList.CollectionChanged += ServiceList_CollectionChanged;

                //OnPropertyChanged(nameof(ServiceList));
                //UpdateDashboard();
            }
        }

        //private void ServiceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    UpdateDashboard();
        //}
        #endregion 


        // observable collection for stock datagrid
        #region stock datagrid collection
        private ObservableCollection<InventoryItemDisplay> _inventoryList;

        public ObservableCollection<InventoryItemDisplay> InventoryList
        {
            get => _inventoryList;
            set
            {
                // remove old collectionchange event subscriptions so when we = it to a new instance it wont duplicate
                if (_inventoryList != null)
                    _inventoryList.CollectionChanged -= InventoryList_CollectionChanged;

                _inventoryList = value;

                // subscribe  to new collection events, allows it to change on add, remove, edit, not just on =
                if (_inventoryList != null)
                    _inventoryList.CollectionChanged += InventoryList_CollectionChanged;

                OnPropertyChanged(nameof(InventoryList));
                UpdateDashboard();
            }

        }

        private void InventoryList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateDashboard();
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set
            {
                _totalItems = value;
                OnPropertyChanged(nameof(TotalItems));
            }
        }

        private int _ItemsAtMinimumAtMinimum;
        public int ItemsAtMinimum
        {
            get => _ItemsAtMinimumAtMinimum;
            set
            {
                _ItemsAtMinimumAtMinimum = value;
                OnPropertyChanged(nameof(ItemsAtMinimum));
            }
        }

        private int _ItemsBelowMinimum;
        public int ItemsBelowMinimum
        {
            get => _ItemsBelowMinimum;
            set
            {
                _ItemsBelowMinimum = value;
                OnPropertyChanged(nameof(ItemsBelowMinimum));
            }
        }

        private string _totalInventoryValue;
        public string TotalInventoryValue
        {
            get => _totalInventoryValue;
            set
            {
                _totalInventoryValue = value;
                OnPropertyChanged(nameof(TotalInventoryValue));
            }
        }

        private string _totalProjectedSales;
        public string TotalProjectedSales
        {
            get => _totalProjectedSales;
            set
            {
                _totalProjectedSales = value;
                OnPropertyChanged(nameof(TotalProjectedSales));
            }
        }

        #endregion

        // notifies when property changes so ObservableCollection = ... works.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {            
            InitializeComponent();

            //parang di na need toh? since may public na sa taas
            //InventoryList = new ObservableCollection<InventoryItemDisplay>();

            InventoryList = StockDBAccess.GetInventoryItemDisplays();
            CustomerList = CustomerDBAccess.GetCustomers();
            ServiceList = ServiceDBAccess.GetServices();

            this.DataContext = this;
        }

        // Source - https://stackoverflow.com/a
        // Posted by Aleksey
        // Retrieved 2025-11-09, License - CC BY-SA 3.0

        // used to get the DataGrid from the container
        private T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(sChildName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, sChildName);

                if (childElement != null)
                    break;
            }
            return childElement;
        }

        private void UpdateDashboard() 
        { 
            TotalItems = InventoryList.Count;
            ItemsAtMinimum = InventoryList.Count(item => item.Quantity == item.MinQuantity);
            ItemsBelowMinimum = InventoryList.Count(item => item.Quantity < item.MinQuantity);
            TotalInventoryValue = InventoryList.Sum(item => item.PurchaseCost * item.Quantity).ToString("$0.00");
            TotalProjectedSales = InventoryList.Sum(item => item.SalePrice * item.Quantity).ToString("$0.00");
        }

        #region Stock Button Functionality

        private void Stock_NewItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic here to:
            // 1. Edit Item.
            // 2. Update the Stock DataGrid's ItemsSource. !!
            AddEditItem addItemWindow = new AddEditItem(true);
            addItemWindow.Owner = this; // Set this window as the owner
            addItemWindow.Title = "Add New Item";

            // ShowDialog() opens the window and pauses code here until the user closes it
            bool? result = addItemWindow.ShowDialog();

            // Check if the user clicked "Save"
            if (result == true)
            {
                // If they saved, add item to the data grid
                InventoryList.Add(addItemWindow.ReturnItem.ToDisplay());
                addItemWindow.ReturnItem.InventoryId = StockDBAccess.AddInventoryItem(addItemWindow.ReturnItem.Item,
                                                                                       addItemWindow.ReturnItem.ItemDetails,
                                                                                       addItemWindow.ReturnItem.Quantity,
                                                                                       addItemWindow.ReturnItem.MinQuantity, 
                                                                                       addItemWindow.ReturnItem.PurchaseCost,
                                                                                       addItemWindow.ReturnItem.SalePrice,
                                                                                       addItemWindow.ReturnItem.LastModified,
                                                                                       addItemWindow.ReturnItem.CreatedDate);
            }
        }

        private void Stock_EditItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic here to:
            // 1. Edit Item.
            // 2. Update the Stock DataGrid's ItemsSource.
            var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "StockDataGrid");
            if (dataGrid?.SelectedItem is InventoryItemDisplay selectedItem)
            {
                AddEditItem addItemWindow = new AddEditItem(false, selectedItem.itemInstance);
                addItemWindow.Owner = this; // Set this window as the owner
                addItemWindow.Title = "Edit Item";

                // ShowDialog() opens the window and pauses code here until the user closes it
                bool? result = addItemWindow.ShowDialog();

                // Check if the user clicked "Save"
                if (result == true)
                {
                    // If they saved, update the data grid entry with the edited item
                    int selectedIndex = dataGrid.SelectedIndex;
                    InventoryList[selectedIndex] = addItemWindow.ReturnItem.ToDisplay();

                    int newQuantity = addItemWindow.ReturnItem.Quantity;
                    int newMinQuantity = addItemWindow.ReturnItem.MinQuantity;
                    float newPurchaseCost = addItemWindow.ReturnItem.PurchaseCost;
                    float newSalePrice = addItemWindow.ReturnItem.SalePrice;
                    string newLastModified = addItemWindow.ReturnItem.LastModified;

                    InventoryList[selectedIndex].Quantity = newQuantity;
                    
                    SKU sku = addItemWindow.ReturnItem.Item.SKUProperty;
                    string itemName = addItemWindow.ReturnItem.Item.ItemName;
                    string itemDetails = addItemWindow.ReturnItem.ItemDetails; // now on InventoryItem
                    Category category = addItemWindow.ReturnItem.Item.Category;
                    string unit = addItemWindow.ReturnItem.Item.Unit;
                    int qtyPerBundle = addItemWindow.ReturnItem.Item.QuantityPerBundle;


                    // Persist inventory changes using the edited InventoryItem returned by the dialog
                    StockDBAccess.EditInventoryItem(addItemWindow.ReturnItem, itemDetails, newQuantity, newMinQuantity, newPurchaseCost, newSalePrice, newLastModified);
                }
            }

        }
        private void Stock_DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this item?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "StockDataGrid");
                if (dataGrid?.SelectedItem is InventoryItemDisplay selectedItem)
                {
                    InventoryList.Remove(selectedItem);
                    StockDBAccess.DeleteInventoryItem(selectedItem.itemInstance);
                }
            }
        }

        // This method will be called to refresh the data in the Stock DataGrid
        private void Stock_Refresh_Click(object sender, RoutedEventArgs e)
        {
            InventoryList = StockDBAccess.GetInventoryItemDisplays();
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
            try
            {
                // Call the ExportToPDF to generate PDF
                bool exported = ExportToPDF.ExportWithSaveDialog(this, InventoryList);

                if (!exported)
                {
                    // Distinguish empty list vs cancelled by user
                    if (InventoryList == null || InventoryList.Count == 0)
                        MessageBox.Show("Nothing to export.", "Export to PDF", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Export cancelled.", "Export to PDF", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // ExportWithSaveDialog already wrote file; optionally inform user
                    MessageBox.Show("PDF exported.", "Export complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to export PDF:\n" + ex.Message, "Export error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // This method will save the grid as an Excel file
        private void Stock_Excel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool exported = ExportToExcel.ExportWithSaveDialog(this, InventoryList);

                if (!exported)
                {
                    // Distinguish empty list vs cancelled by user
                    if (InventoryList == null || InventoryList.Count == 0)
                        MessageBox.Show("Nothing to export.", "Export to Excel", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Export cancelled.", "Export to Excel", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Excel exported.", "Export complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to export Excel:\n" + ex.Message, "Export error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // This method will open the specific "Inventory Count Report"
        private void Stock_InventoryReport_Click(object sender, RoutedEventArgs e)
        {
            // This will likely open a new window or switch to the Reporting tab
            // with a specific report selected.

            MessageBox.Show("Open Inventory Count Report... (functionality to be added)");
        }
        private void StockSearchBox_KeyUp(object sender, KeyEventArgs e)
                {
                    var textBox = sender as TextBox;
                    string searchText = textBox.Text;
                    if (e.Key == Key.Enter)
                    {
                        if (searchText.IsNullOrEmpty())
                        {
                            InventoryList = StockDBAccess.GetInventoryItemDisplays();
                            textBox.Text = "Search Stock";
                            return;
                        }

                        InventoryList = StockDBAccess.SearchInventory(searchText);
                    }
                }
        #endregion

        #region Services Button Functionality

        private void Services_Refresh_Click(object sender, RoutedEventArgs e)
        {
            ServiceList = ServiceDBAccess.GetServices();
        }

        private void Services_NewService_Click(object sender, RoutedEventArgs e)
        {
            AddEditServiceWindow addServiceWindow = new AddEditServiceWindow(true);
            addServiceWindow.Owner = this; // Set this window as the owner
            addServiceWindow.Title = "Add New Service";

            // ShowDialog() opens the window and pauses code here until the user closes it
            bool? result = addServiceWindow.ShowDialog();

            // Check if the user clicked "Save"
            if (result == true)
            {
                // If they saved, add item to the data grid
                ServiceList.Add(addServiceWindow.ReturnService);
                addServiceWindow.ReturnService.ServiceID = ServiceDBAccess.AddService(addServiceWindow.ReturnService.Customer,
                                                                                       addServiceWindow.ReturnService.ServiceDetails,
                                                                                       addServiceWindow.ReturnService.ServiceCategory,
                                                                                       addServiceWindow.ReturnService.MaterialCost,
                                                                                       addServiceWindow.ReturnService.InvoicePrice,
                                                                                       addServiceWindow.ReturnService.Details,
                                                                                       addServiceWindow.ReturnService.LastModified,
                                                                                       addServiceWindow.ReturnService.CreatedDate);
            }
        }

        private void Services_EditService_Click(object sender, RoutedEventArgs e)
        {
            var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "ServiceDataGrid");
            if (dataGrid?.SelectedItem is Service selectedService)
            {
                AddEditServiceWindow addServiceWindow = new AddEditServiceWindow(false, selectedService);
                addServiceWindow.Owner = this; // Set this window as the owner
                addServiceWindow.Title = "Edit Service";

                // ShowDialog() opens the window and pauses code here until the user closes it
                bool? result = addServiceWindow.ShowDialog();

                // Check if the user clicked "Save"
                if (result == true)
                {
                    // If they saved, update the data grid entry with the edited item
                    int selectedIndex = dataGrid.SelectedIndex;
                    ServiceList[selectedIndex] = addServiceWindow.ReturnService;

                    Customer newCustomer = addServiceWindow.ReturnService.Customer;
                    string newServiceDetails = addServiceWindow.ReturnService.ServiceDetails;
                    ServiceCategory newServiceCategory = addServiceWindow.ReturnService.ServiceCategory;
                    float newMaterialCost = addServiceWindow.ReturnService.MaterialCost;
                    float newInvoicePrice = addServiceWindow.ReturnService.InvoicePrice;
                    string newDetails = addServiceWindow.ReturnService.Details;
                    string newLastModified = addServiceWindow.ReturnService.LastModified;

                    // Persist inventory changes using the edited InventoryItem returned by the dialog
                    ServiceDBAccess.EditService(addServiceWindow.ReturnService, newCustomer, newServiceDetails, newServiceCategory, newMaterialCost, newInvoicePrice, newDetails, newLastModified);
                }
            }
        }

        private void Services_DeleteService_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this item?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "ServiceDataGrid");
                if (dataGrid?.SelectedItem is Service selectedService)
                {
                    ServiceList.Remove(selectedService);
                    ServiceDBAccess.DeleteService(selectedService);
                }
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

        private void ServiceSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            string searchText = textBox.Text;
            if (e.Key == Key.Enter)
            {
                if (searchText.IsNullOrEmpty())
                {
                    ServiceList = ServiceDBAccess.GetServices();
                    textBox.Text = "Search Services";
                    return;
                }

                ServiceList = ServiceDBAccess.SearchServices(searchText);
            }
        }
        #endregion

    }
}