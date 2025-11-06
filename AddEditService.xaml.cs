using System.Windows;

namespace WEGutters
{
    public partial class AddEditServiceWindow : Window
    {
        public AddEditServiceWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add logic here to save the data from the textboxes
            // (e.g., ItemNameBox.Text) to your database.

            // After saving, close the window.
            this.DialogResult = true; // This tells the MainWindow that we saved.
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window.
            this.DialogResult = false; // This tells the MainWindow that we cancelled.
            this.Close();
        }
    }
}