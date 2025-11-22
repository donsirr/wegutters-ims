using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
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
                if (_customerList != null)
                    _customerList.CollectionChanged -= CustomerList_CollectionChanged;

                _customerList = value;

                if (_customerList != null)
                    _customerList.CollectionChanged += CustomerList_CollectionChanged;

                OnPropertyChanged(nameof(CustomerList));
                UpdateServiceDashboard();
            }
        }

        private void CustomerList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateServiceDashboard();
        }
        #endregion


        #region observable collection for services
        private ObservableCollection<Service> _serviceList;

        public ObservableCollection<Service> ServiceList
        {
            get => _serviceList;
            set
            {
                if (_serviceList != null)
                        _serviceList.CollectionChanged -= ServiceList_CollectionChanged;

                _serviceList = value;

                if (_serviceList != null)
                _serviceList.CollectionChanged += ServiceList_CollectionChanged;

                OnPropertyChanged(nameof(ServiceList));
                UpdateServiceDashboard();
            }
        }

        private void ServiceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateServiceDashboard();
        }

        private int _totalServices;

        public int TotalServices
        {
            get => _totalServices;
            set
            {
                _totalServices = value;
                OnPropertyChanged(nameof(TotalServices));
            }
        }

        private string _highestMaterialCost;
        public string HighestMaterialCost
        {
            get => _highestMaterialCost;
            set
            {
                _highestMaterialCost = value;
                OnPropertyChanged(nameof(HighestMaterialCost));
            }
        }

        private string _highestMaterialCustomer;

        public string HighestMaterialCustomer
        {
            get => _highestMaterialCustomer;
            set
            {
                _highestMaterialCustomer = value;
                OnPropertyChanged(nameof(HighestMaterialCustomer));
            }
        }

        private string _highestMaterialCategory;

        public string HighestMaterialCategory
        {
            get => _highestMaterialCategory;
            set
            {
                _highestMaterialCategory = value;
                OnPropertyChanged(nameof(HighestMaterialCategory));
            }
        }

        private string _highestInvoice;

        public string HighestInvoice
        {
            get => _highestInvoice;
            set
            {
                _highestInvoice = value;
                OnPropertyChanged(nameof(HighestInvoice));
            }
        }

        private string _highestInvoiceCustomer;
        public string HighestInvoiceCustomer
        {
            get => _highestInvoiceCustomer;
            set
            {
                _highestInvoiceCustomer = value;
                OnPropertyChanged(nameof(HighestInvoiceCustomer));
            }
        }

        private string _highestInvoiceCategory;
        public string HighestInvoiceCategory
        {
            get => _highestInvoiceCategory;
            set
            {
                _highestInvoiceCategory = value;
                OnPropertyChanged(nameof(HighestInvoiceCategory));
            }
        }
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
                UpdateStockDashboard();
            }

        }

        private void InventoryList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateStockDashboard();
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

        private void UpdateStockDashboard() 
        { 
            TotalItems = InventoryList.Count;
            ItemsAtMinimum = InventoryList.Count(item => item.Quantity == item.MinQuantity);
            ItemsBelowMinimum = InventoryList.Count(item => item.Quantity < item.MinQuantity);
            TotalInventoryValue = InventoryList.Sum(item => item.PurchaseCost * item.Quantity).ToString("$0.00");
            TotalProjectedSales = InventoryList.Sum(item => item.SalePrice * item.Quantity).ToString("$0.00");
        }

        private void UpdateServiceDashboard()
        {
            var dashboardPlot = FindElementByName<WpfPlot>(ContentControlPanel, "DashboardPopularServices");
            // update barPlot


            TotalServices = ServiceList?.Count() ?? 0;
            HighestMaterialCost = ServiceList?.MaxBy(s => s.MaterialCost)?.MaterialCost.ToString("$0.00") ?? "$0.00";
            HighestMaterialCustomer = ServiceList?.MaxBy(s => s.MaterialCost)?.Customer.Name ?? "N/A";
            HighestMaterialCategory = ServiceList?.MaxBy(s => s.MaterialCost)?.ServiceCategory.ServiceCategoryName ?? "N/A";

            HighestInvoice = ServiceList?.MaxBy(s => s.InvoicePrice)?.InvoicePrice.ToString("$0.00") ?? "$0.00";
            HighestInvoiceCustomer = ServiceList?.MaxBy(s => s.InvoicePrice)?.Customer.Name ?? "N/A";
            HighestInvoiceCategory = ServiceList?.MaxBy(s => s.InvoicePrice)?.ServiceCategory.ServiceCategoryName ?? "N/A";
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
                AddEditItem editItemWindow = new AddEditItem(false, selectedItem.itemInstance);
                editItemWindow.Owner = this; // Set this window as the owner
                editItemWindow.Title = "Edit Item";

                // ShowDialog() opens the window and pauses code here until the user closes it
                bool? result = editItemWindow.ShowDialog();

                // Check if the user clicked "Save"
                if (result == true)
                {
                    // If they saved, update the data grid entry with the edited item
                    int selectedIndex = dataGrid.SelectedIndex;
                    InventoryList[selectedIndex] = editItemWindow.ReturnItem.ToDisplay();

                    int newQuantity = editItemWindow.ReturnItem.Quantity;
                    int newMinQuantity = editItemWindow.ReturnItem.MinQuantity;
                    float newPurchaseCost = editItemWindow.ReturnItem.PurchaseCost;
                    float newSalePrice = editItemWindow.ReturnItem.SalePrice;
                    string newLastModified = editItemWindow.ReturnItem.LastModified;

                    InventoryList[selectedIndex].Quantity = newQuantity;
                    
                    SKU sku = editItemWindow.ReturnItem.Item.SKUProperty;
                    string itemName = editItemWindow.ReturnItem.Item.ItemName;
                    string itemDetails = editItemWindow.ReturnItem.ItemDetails; // now on InventoryItem
                    Category category = editItemWindow.ReturnItem.Item.Category;
                    string unit = editItemWindow.ReturnItem.Item.Unit;
                    int qtyPerBundle = editItemWindow.ReturnItem.Item.QuantityPerBundle;


                    // Persist inventory changes using the edited InventoryItem returned by the dialog
                    StockDBAccess.EditInventoryItem(editItemWindow.ReturnItem, itemDetails, newQuantity, newMinQuantity, newPurchaseCost, newSalePrice, newLastModified);
                }
            }
            else 
            {
                if (InventoryList.Count > 0)
                {
                    MessageBox.Show("Please select an item to edit.", "Edit Item", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Currently no items to edit.", "Edit Item", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private void Stock_DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "StockDataGrid");
            if (dataGrid?.SelectedItem is InventoryItemDisplay selectedItem)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this item?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    InventoryList.Remove(selectedItem);
                    StockDBAccess.DeleteInventoryItem(selectedItem.itemInstance);
                }
            }
            else
            {
                if (InventoryList.Count > 0)
                {
                    MessageBox.Show("Please select an item to delete.", "Delete Item", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Currently no items to delete.", "Delete Item", MessageBoxButton.OK, MessageBoxImage.Error);
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
                AddEditServiceWindow editServiceWindow = new AddEditServiceWindow(false, selectedService);
                editServiceWindow.Owner = this; // Set this window as the owner
                editServiceWindow.Title = "Edit Service";

                // ShowDialog() opens the window and pauses code here until the user closes it
                bool? result = editServiceWindow.ShowDialog();

                // Check if the user clicked "Save"
                if (result == true)
                {
                    // If they saved, update the data grid entry with the edited item
                    int selectedIndex = dataGrid.SelectedIndex;
                    ServiceList[selectedIndex] = editServiceWindow.ReturnService;

                    Customer newCustomer = editServiceWindow.ReturnService.Customer;
                    string newServiceDetails = editServiceWindow.ReturnService.ServiceDetails;
                    ServiceCategory newServiceCategory = editServiceWindow.ReturnService.ServiceCategory;
                    float newMaterialCost = editServiceWindow.ReturnService.MaterialCost;
                    float newInvoicePrice = editServiceWindow.ReturnService.InvoicePrice;
                    string newDetails = editServiceWindow.ReturnService.Details;
                    string newLastModified = editServiceWindow.ReturnService.LastModified;

                    // Persist inventory changes using the edited InventoryItem returned by the dialog
                    ServiceDBAccess.EditService(editServiceWindow.ReturnService, newCustomer, newServiceDetails, newServiceCategory, newMaterialCost, newInvoicePrice, newDetails, newLastModified);
                    dataGrid.Items.Refresh();
                    UpdateServiceDashboard();
                }
            }
            else
            {
                if (ServiceList.Count > 0)
                {
                    MessageBox.Show("Please select a service to edit.", "Edit Service", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Currently no services to edit.", "Edit Service", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Services_DeleteService_Click(object sender, RoutedEventArgs e)
        {
            var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "ServiceDataGrid");
            if (dataGrid?.SelectedItem is Service selectedService)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this service?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    ServiceList.Remove(selectedService);
                    ServiceDBAccess.DeleteService(selectedService);
                }
            }
            else
            {
                if (ServiceList.Count > 0)
                {
                    MessageBox.Show("Please select a service to delete.", "Delete Service", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Currently no services to delete.", "Delete Service", MessageBoxButton.OK, MessageBoxImage.Error);
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

        #region Customers Button Functionality

        //Customer_NewCustomer_Click
        //Customer_EditCustomer_Click
        //Customer_DeleteCustomer_Click
        //Customer_Refresh_Click
        //Customer_SaveAsPdf_Click
        //Customer_Excel_Click

        private void Customer_NewCustomer_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic here to:
            // 1. Edit Item.
            // 2. Update the Stock DataGrid's ItemsSource. !!
            AddEditCustomer addCustomer = new AddEditCustomer(true);
            addCustomer.Owner = this; // Set this window as the owner
            addCustomer.Title = "Add New Customer";

            // ShowDialog() opens the window and pauses code here until the user closes it
            bool? result = addCustomer.ShowDialog();

            // Check if the user clicked "Save"
            if (result == true)
            {
                // If they saved, add item to the data grid
                CustomerList.Add(addCustomer.ReturnCustomer);
                addCustomer.ReturnCustomer.CustomerID = CustomerDBAccess.AddCustomer(addCustomer.ReturnCustomer.Name,
                                                                                     addCustomer.ReturnCustomer.Address,
                                                                                     addCustomer.ReturnCustomer.ContactNumber,
                                                                                     addCustomer.ReturnCustomer.Email,
                                                                                     addCustomer.ReturnCustomer.Comments,
                                                                                     addCustomer.ReturnCustomer.LastModified,
                                                                                     addCustomer.ReturnCustomer.CreatedDate);
            }
        }

        private void Customer_EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic here to:
            // 1. Edit Item.
            // 2. Update the Stock DataGrid's ItemsSource.
            var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "CustomerDataGrid");

            if (dataGrid?.SelectedItem is Customer selectedCustomer)
            {
                AddEditCustomer editCustomerWindow = new AddEditCustomer(false, selectedCustomer);
                editCustomerWindow.Owner = this; // Set this window as the owner
                editCustomerWindow.Title = "Edit Customer";

                // ShowDialog() opens the window and pauses code here until the user closes it
                bool? result = editCustomerWindow.ShowDialog();

                // Check if the user clicked "Save"
                if (result == true)
                {
                    // If they saved, update the data grid entry with the edited customer
                    int selectedIndex = dataGrid.SelectedIndex;
                    CustomerList[selectedIndex] = editCustomerWindow.ReturnCustomer;

                    string newName = editCustomerWindow.ReturnCustomer.Name;
                    string newAddress = editCustomerWindow.ReturnCustomer.Address;
                    string newContactNumber = editCustomerWindow.ReturnCustomer.ContactNumber;
                    string newEmail = editCustomerWindow.ReturnCustomer.Email;
                    string newComments = editCustomerWindow.ReturnCustomer.Comments;
                    string newLastModified = editCustomerWindow.ReturnCustomer.LastModified;

                    // Persist inventory changes using the edited Customer returned by the dialog
                   
                    CustomerDBAccess.EditCustomer(editCustomerWindow.ReturnCustomer, newName, newAddress, newContactNumber, newEmail, newComments, newLastModified);
                    dataGrid.Items.Refresh();
                    ServiceList = ServiceDBAccess.GetServices();
                    UpdateServiceDashboard();
                }
            }
            else 
            {
                if (CustomerList.Count > 0)
                {
                    MessageBox.Show("Please select a customer to edit.", "Edit Customer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Currently no customers to edit.", "Edit Customer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        }
        private void Customer_DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var dataGrid = FindElementByName<DataGrid>(ContentControlPanel, "CustomerDataGrid");
            if (dataGrid?.SelectedItem is Customer selectedCustomer)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this customer?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    CustomerList.Remove(selectedCustomer);
                    CustomerDBAccess.DeleteCustomer(selectedCustomer);
                }
            }
            else
            {
                if (CustomerList.Count > 0)
                {
                    MessageBox.Show("Please select a customer to delete.", "Delete Customer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Currently no customers to delete.", "Delete Customer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        // This method will be called to refresh the data in the Stock DataGrid
        private void Customer_Refresh_Click(object sender, RoutedEventArgs e)
        {
            CustomerList = CustomerDBAccess.GetCustomers();
        }

        // This method will save the grid as a PDF
        private void Customer_SaveAsPdf_Click(object sender, RoutedEventArgs e)
        {
            //make new class for pdf export for customers
            //try
            //{
            //    // Call the ExportToPDF to generate PDF
            //    bool exported = ExportToPDF.ExportWithSaveDialog(this, InventoryList);

            //    if (!exported)
            //    {
            //        // Distinguish empty list vs cancelled by user
            //        if (InventoryList == null || InventoryList.Count == 0)
            //            MessageBox.Show("Nothing to export.", "Export to PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            //        else
            //            MessageBox.Show("Export cancelled.", "Export to PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    else
            //    {
            //        // ExportWithSaveDialog already wrote file; optionally inform user
            //        MessageBox.Show("PDF exported.", "Export complete", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Failed to export PDF:\n" + ex.Message, "Export error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        // This method will save the grid as an Excel file
        private void Customer_Excel_Click(object sender, RoutedEventArgs e)
        {
            //make new class for excel export for customers
            //try
            //{
            //    bool exported = ExportToExcel.ExportWithSaveDialog(this, InventoryList);

            //    if (!exported)
            //    {
            //        // Distinguish empty list vs cancelled by user
            //        if (InventoryList == null || InventoryList.Count == 0)
            //            MessageBox.Show("Nothing to export.", "Export to Excel", MessageBoxButton.OK, MessageBoxImage.Information);
            //        else
            //            MessageBox.Show("Export cancelled.", "Export to Excel", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    else
            //    {
            //        MessageBox.Show("Excel exported.", "Export complete", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Failed to export Excel:\n" + ex.Message, "Export error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        private void CustomerSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            string searchText = textBox.Text;
            if (e.Key == Key.Enter)
            {
                if (searchText.IsNullOrEmpty())
                {
                    CustomerList = CustomerDBAccess.GetCustomers();
                    textBox.Text = "Search Customers";
                    return;
                }

                CustomerList = CustomerDBAccess.SearchCustomers(searchText);
            }
        }

        #endregion

    }
}