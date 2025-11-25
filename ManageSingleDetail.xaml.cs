using System;
using System.Collections.Generic;
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
using WEGutters.ServiceClasses;
using WEGutters.UserClasses;

namespace WEGutters
{
    /// <summary>
    /// Interaction logic for ManageSingleDetail.xaml
    /// </summary>
    public partial class ManageSingleDetail : Window
    {
        private bool isNew = true;
        public Object ReturnBase { get; set; }
        private string labelText;

        public ManageSingleDetail(bool isNew, Object currentBase = null, string labelText = "Base Label")
        {
            InitializeComponent();

            this.isNew = isNew;
            ReturnBase = currentBase;
            this.labelText = labelText;
            DataContext = this;

            ManagedDetailLabel.Text = labelText + ":";

            if (!isNew)
            {
                SetEditValues();
            }
        }

        private void SetEditValues()
        {
            if (ReturnBase == null) return;

            ManagedDetailBox.Text = ReturnBase switch
            {
                Category c => c.CategoryName,
                SKU s => s.SKUCode,
                ServiceCategory sc => sc.ServiceCategoryName,
                _ => string.Empty
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isNew)
                {
                    ReturnBase = ReturnBase switch
                    {
                        null => labelText switch
                        {
                            "Category Name" => new Category(ManagedDetailBox.Text),
                            "SKU Name" => new SKU(ManagedDetailBox.Text),
                            "Service Category Name" => new ServiceCategory(ManagedDetailBox.Text),
                            _ => null
                        },
                        _ => ReturnBase
                    };
                }
                else
                {
                    // Editing existing object
                    switch (ReturnBase)
                    {
                        case Category category:
                            category.CategoryName = ManagedDetailBox.Text;
                            break;
                        case SKU sku:
                            sku.SKUCode = ManagedDetailBox.Text;
                            break;
                        case ServiceCategory serviceCategory:
                            serviceCategory.ServiceCategoryName = ManagedDetailBox.Text;
                            break;
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // Trim and normalize input
            var itemName = (ManagedDetailBox.Text ?? string.Empty).Trim();
            ManagedDetailBox.Text = itemName;

            if (string.IsNullOrEmpty(itemName))
            {
                errors.AppendLine($"- {labelText} cannot be blank.");
            }
            else
            {
                bool exists = false;

                // Check existence depending on object type or label
                if (ReturnBase != null)
                {
                    exists = ReturnBase switch
                    {
                        Category _ => StockDBAccess.CategoryExists(itemName),
                        SKU _ => StockDBAccess.SKUExists(itemName),
                        ServiceCategory _ => ServiceDBAccess.ServiceCategoryExists(itemName),
                        _ => false
                    };
                }
                else
                {
                    // If adding a new item, guess type by label (or replace with a proper type hint)
                    exists = labelText switch
                    {
                        "Category Name" => StockDBAccess.CategoryExists(itemName),
                        "SKU Name" => StockDBAccess.SKUExists(itemName),
                        "Service Category Name" => ServiceDBAccess.ServiceCategoryExists(itemName),
                        _ => false
                    };
                }

                // If adding and the name exists, show validation error
                if (isNew && exists)
                {
                    errors.AppendLine($"{labelText} with the name '{itemName}' already exists.");
                }
                else
                {
                    // make sure its not "add new..." by guessing which data type it is.
                    int type = labelText switch
                    {
                        "Category Name" => 0,
                        "SKU Name" => 1,
                        "Service Category Name" => 2,
                        _ => 3
                    };

                    if (type == 0 && (string.Equals(itemName, "Add New Category", StringComparison.OrdinalIgnoreCase)))
                    {
                        errors.AppendLine("- Item Name cannot be \"Add New Category\".");
                    }
                    else if (type == 1 && (string.Equals(itemName, "Add New SKU", StringComparison.OrdinalIgnoreCase)))
                    {
                        errors.AppendLine("- Item Name cannot be \"Add New SKU\".");
                    }
                    else if (type == 2 && (string.Equals(itemName, "Add New Service Category", StringComparison.OrdinalIgnoreCase)))
                    {
                        errors.AppendLine("- Item Name cannot be \"Add New Service Category\".");
                    }
                }
            }

            validationError = errors.ToString().TrimEnd();
            return validationError.Length == 0;
        }

    }
}
