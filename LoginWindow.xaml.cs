using System.Windows;
using WEGutters.DatabaseAccess;
using WEGutters.UserClasses;

namespace WEGutters
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            UserDBAccess.InitializeDatabase(); // Ensure table exists
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Password;

            User loggedInUser = UserDBAccess.Login(user, pass);
            if (loggedInUser != null)
            {
                MainWindow main = new MainWindow(loggedInUser.AccessLevel);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Simple prompt to contact admin or open registration logic
            MessageBox.Show("Please contact your System Administrator to register a new account, or use the default admin/admin login to create one in the Users tab.", "Registration");
        }
    }
}