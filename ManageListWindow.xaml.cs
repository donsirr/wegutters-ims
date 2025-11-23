using System;
using System.Collections.Generic;
using System.Windows;
using WEGutters.ConstructorClasses;

namespace WEGutters
{
    public partial class ManageListWindow : Window
    {
        public enum ManageType { Category, SKU, ServiceCategory, BaseItem }
        private ManageType _type;

        // Simple wrapper class for display
        public class ListItem { public int Id { get; set; } public string Name { get; set; } }

        public ManageListWindow(ManageType type)
        {
            InitializeComponent();
            _type = type;
            this.Title = "Manage " + type.ToString();
            LoadData();
        }

        private void LoadData()
        {
            var list = new List<ListItem>();
            if (_type == ManageType.Category)
            {
                foreach (var c in StockDBAccess.GetCategories()) list.Add(new ListItem { Id = c.CategoryID, Name = c.CategoryName });
            }
            else if (_type == ManageType.SKU)
            {
                foreach (var s in StockDBAccess.GetSKUs()) list.Add(new ListItem { Id = s.SKUID, Name = s.SKUCode });
            }
            else if (_type == ManageType.ServiceCategory)
            {
                foreach (var sc in ServiceDBAccess.GetServiceCategories()) list.Add(new ListItem { Id = sc.ServiceCategoryID, Name = sc.ServiceCategoryName });
            }
            ItemListBox.ItemsSource = list;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string txt = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(txt)) return;

            if (_type == ManageType.Category) StockDBAccess.AddCategory(txt);
            else if (_type == ManageType.SKU) StockDBAccess.AddSKU(txt);
            else if (_type == ManageType.ServiceCategory) ServiceDBAccess.AddServiceCategory(txt);

            InputBox.Text = "";
            LoadData();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ItemListBox.SelectedItem is ListItem selected)
            {
                if (MessageBox.Show($"Delete {selected.Name}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Note: You need to implement generic delete by ID in your DBAccess classes
                    // For now we assume access via ID
                    if (_type == ManageType.Category) StockDBAccess.DeleteCategory(new Category(selected.Name) { CategoryID = selected.Id });
                    else if (_type == ManageType.SKU) StockDBAccess.DeleteSKU(new SKU(selected.Name) { SKUID = selected.Id });
                    else if (_type == ManageType.ServiceCategory) { /* Implement Delete in ServiceDBAccess */ }

                    LoadData();
                }
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}