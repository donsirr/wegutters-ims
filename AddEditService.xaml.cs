using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using WEGutters.UserClasses;
using WEGutters.ServiceClasses;
using WEGutters.CustomerClasses;
using WEGutters.DatabaseAccess;

namespace WEGutters
{
    public partial class AddEditServiceWindow : Window
    {
        private bool isNew = true;
        private string createdDate;
        public ObservableCollection<Customer> CustomerCollection { get; set; } = new ObservableCollection<Customer>();
        public ObservableCollection<ServiceCategory> ServiceCategoryCollection { get; set; } = new ObservableCollection<ServiceCategory>();

        public Service ReturnService { get; set; }

        public AddEditServiceWindow(bool isNew, Service currentService = null)
        {
            InitializeComponent();

            ContactPhoneBox.IsEnabled = false;
            ContactPhoneLabel.Opacity = 0.25;
            AddressBox.IsEnabled = false;
            AddressLabel.Opacity = 0.25;

            UpdateCustomerComboBox(CustomerDBAccess.GetCustomers());
            UpdateServiceCategoryComboBox(ServiceDBAccess.GetServiceCategories());

            this.isNew = isNew;
            ReturnService = currentService;
            DataContext = this;
            if (!isNew)
            {
                SetEditValues();
            }
        }

        private void SetEditValues()
        {
            if (ReturnService == null || ReturnService.Customer == null) return;

            // Try to find a matching BaseItem already in the ItemsSource (by reference or by name+SKU code)
            var custMatch = CustomerCollection.FirstOrDefault(c =>
                object.ReferenceEquals(c, ReturnService.Customer) ||
                (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(ReturnService.Customer.Name)
                    && c.Name == ReturnService.Customer.Name
                    && ((c?.CustomerID ?? 0) == (ReturnService.Customer?.CustomerID ?? 0)))
            );

            if (custMatch == null)
            {
                // ensure the ItemsSource contains the instance we want to select
                CustomerCollection.Add(ReturnService.Customer);
                custMatch = ReturnService.Customer;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Select the BaseItem that belongs to the ItemsSource
                CustomerComboBox.SelectedItem = custMatch;

                // Ensure CategoryCollection has matching Category instance
                var servCatName = ReturnService.ServiceCategory?.ServiceCategoryName ?? string.Empty;
                var servCatMatch = ServiceCategoryCollection.FirstOrDefault(s => s.ServiceCategoryName == servCatName);
                if (servCatMatch == null && ReturnService.ServiceCategory != null)
                {
                    ServiceCategoryCollection.Add(ReturnService.ServiceCategory);
                    servCatMatch = ReturnService.ServiceCategory;
                }
                ServiceCategoryComboBox.SelectedItem = servCatMatch ?? ServiceCategoryCollection.FirstOrDefault();

                ServiceDetailsBox.Text = ReturnService.ServiceDetails;
                MaterialCostBox.Text = ReturnService.MaterialCost.ToString();
                InvoicePriceBox.Text = ReturnService.InvoicePrice.ToString();
                DetailsBox.Text = ReturnService.Details.ToString();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private void UpdateCustomerComboBox(ObservableCollection<Customer> Customers)
        {
            CustomerCollection = Customers;
        }
        private void UpdateServiceCategoryComboBox(ObservableCollection<ServiceCategory> ServiceCategories)
        {
            ServiceCategoryCollection = ServiceCategories;
            ServiceCategoryCollection.Insert(0, (new ServiceCategory("Add New Service Category")));
        }

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
                createdDate = ReturnService.CreatedDate;
            }

            try
            {
                // If editing, update the existing instance in-place so its InventoryId remains intact.
                if (!isNew && ReturnService != null)
                {
                    ReturnService.Customer = GetSelectedCustomer();
                    ReturnService.ServiceDetails = ServiceDetailsBox.Text;
                    ReturnService.ServiceCategory = GetSelectedServiceCategory();
                    ReturnService.MaterialCost = float.Parse(MaterialCostBox.Text, CultureInfo.InvariantCulture);
                    ReturnService.InvoicePrice = float.Parse(InvoicePriceBox.Text, CultureInfo.InvariantCulture);
                    ReturnService.Details = DetailsBox.Text;
                    ReturnService.LastModified = DateTime.Now.ToString("yyyy-MM-dd_HH:mm");
                    // CreatedDate remains as-is (createdDate variable preserves it)
                }
                else
                {
                    // New item: create new instance (InventoryId will be assigned by DB on insert)
                    Service service = new Service(
                        GetSelectedCustomer(),
                        ServiceDetailsBox.Text,
                        GetSelectedServiceCategory(),
                        float.Parse(MaterialCostBox.Text, CultureInfo.InvariantCulture),
                        float.Parse(InvoicePriceBox.Text, CultureInfo.InvariantCulture),
                        DetailsBox.Text,
                        DateTime.Now.ToString("yyyy-MM-dd_HH:mm"),
                        createdDate);
                    ReturnService = service;
                }

                this.DialogResult = true; // tell caller we saved
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Customer GetSelectedCustomer()
        {
            if (CustomerComboBox.SelectedIndex == 0 && (StockDBAccess.SKUExists(CustomerComboBox.Text)))
            {
                // get existing SKU
                var custo = CustomerCollection.FirstOrDefault(c => c.Name == CustomerComboBox.Text);
                return custo;
            }
            else
            {
                return CustomerComboBox.SelectedItem as Customer;
            }
        }
        private ServiceCategory GetSelectedServiceCategory()
        {
            if (ServiceCategoryComboBox.SelectedIndex == 0 && !(ServiceDBAccess.ServiceCategoryExists(ServiceCategoryComboBox.Text)))
            {
                // create new SKU
                ServiceCategory serviceCateg = new ServiceCategory(ServiceCategoryComboBox.Text);
                serviceCateg.ServiceCategoryID = ServiceDBAccess.AddServiceCategory(ServiceCategoryComboBox.Text);
                ServiceCategoryCollection.Add(serviceCateg);
                ServiceCategoryComboBox.SelectedItem = serviceCateg;
                return serviceCateg;
            }
            else if (ServiceCategoryComboBox.SelectedIndex == 0 && (ServiceDBAccess.ServiceCategoryExists(ServiceCategoryComboBox.Text)))
            {
                // get existing category
                var serviceCateg = ServiceCategoryCollection.FirstOrDefault(c => c.ServiceCategoryName == ServiceCategoryComboBox.Text);
                return serviceCateg;
            }
            else
            {
                return ServiceCategoryComboBox.SelectedItem as ServiceCategory;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window.
            this.DialogResult = false; // This tells the MainWindow that we cancelled.
            this.Close();
        }

        private void ServiceCategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ServiceCategoryComboBox.SelectedIndex == 0)
            {
                ServiceCategoryComboBox.IsEditable = true;
            }
            else
            {
                ServiceCategoryComboBox.IsEditable = false;
            }
        }

        private bool ValidateInputs(out string validationError)
        {
            var errors = new StringBuilder();

            //Customer
            string customerName;
            customerName = (CustomerComboBox.SelectedItem as Customer)?.Name?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(customerName))
            {
                errors.AppendLine("- Currently no customers, please input customers in the \"Customers\" tab");
            }


            // Service details
            var itemDetails = (ServiceDetailsBox.Text ?? string.Empty).Trim();
            ServiceDetailsBox.Text = itemDetails;
            if (string.IsNullOrEmpty(itemDetails))
            {
                errors.AppendLine("- Service Details cannot be blank.");
            }

            // Service Category
            string serviceCategoryName;
            if (ServiceCategoryComboBox.SelectedIndex == 0)
            {
                serviceCategoryName = (ServiceCategoryComboBox.Text ?? string.Empty).Trim();
                ServiceCategoryComboBox.Text = serviceCategoryName;
            }
            else
            {
                serviceCategoryName = (ServiceCategoryComboBox.SelectedItem as ServiceCategory)?.ServiceCategoryName?.Trim() ?? string.Empty;
            }
            if (string.IsNullOrEmpty(serviceCategoryName))
            {
                errors.AppendLine("- Service Category cannot be blank.");
            }
            else if (string.Equals(serviceCategoryName, "Add New Service Category", StringComparison.OrdinalIgnoreCase))
            {
                errors.AppendLine("- Service Category cannot be \"Add New Service Category\".");
            }

            // Material cost
            var materialCostText = (MaterialCostBox.Text ?? string.Empty).Trim();
            MaterialCostBox.Text = materialCostText;
            if (!float.TryParse(materialCostText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out _))
            {
                errors.AppendLine("- Material Cost must be a number.");
            }

            // Invoice price
            var invoicePriceText = (InvoicePriceBox.Text ?? string.Empty).Trim();
            InvoicePriceBox.Text = invoicePriceText;
            if (!float.TryParse(invoicePriceText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out _))
            {
                errors.AppendLine("- Invoice Price must be a number.");
            }

            // Details
            var details = (DetailsBox.Text ?? string.Empty).Trim();
            DetailsBox.Text = details;
            if (string.IsNullOrEmpty(details))
            {
                errors.AppendLine("- Details cannot be blank.");
            }

            validationError = errors.ToString().TrimEnd();
            return validationError.Length == 0;
        }

        private void CustomerComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ContactPhoneBox.Text = (CustomerComboBox.SelectedItem as Customer).ContactNumber;
            AddressBox.Text = (CustomerComboBox.SelectedItem as Customer).Address;
        }
    }
}