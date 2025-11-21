using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEGutters
{
    internal class CustomerDBAccess
    {

        public static ObservableCollection<Customer> GetCustomers()
        {
            var customersCollection = new ObservableCollection<Customer>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * From ServiceCategories;", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customer = new Customer(
                                reader.GetString(1), // CustomerName
                                reader.GetString(2), // CustomerAddress
                                reader.GetString(3), // CustomerNumber
                                reader.GetString(4), // CustomerEmail
                                reader.GetString(5), // CustomerComments
                                reader.GetString(6), // LastModified
                                reader.GetString(7)  // CreatedDate
                            )
                            { CustomerID = reader.GetInt32(0) }; // CategoryID since it's not in the constructor

                            customersCollection.Add(customer);
                        }
                    }
                }
                return customersCollection;
            }
        }






        public static int GetCustomerID(Customer customer)
        {
            if (customer.CustomerID != 0) { return customer.CustomerID; }

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT CustomerID FROM Customers WHERE CustomerName = @CustomerName AND CustomerAddress = @CustomerAddress AND CustomerNumber = @CustomerNumber;", conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", customer.Name);
                    cmd.Parameters.AddWithValue("@CustomerAddress", customer.Address);
                    cmd.Parameters.AddWithValue("@CustomerNumber", customer.ContactNumber);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        
    }
}
