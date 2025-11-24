using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEGutters.CustomerClasses;

namespace WEGutters.DatabaseAccess
{
    internal class CustomerDBAccess
    {
        private static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;");
            conn.Open();

            using (var cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn))
                cmd.ExecuteNonQuery();

            return conn;
        }

        public static ObservableCollection<Customer> GetCustomers()
        {
            var customersCollection = new ObservableCollection<Customer>();
            using (var conn = GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT * From Customers ORDER BY CustomerID LIMIT -1 OFFSET 1;", conn))
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

            using (var conn = GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT CustomerID FROM Customers WHERE CustomerName = @CustomerName AND CustomerAddress = @CustomerAddress AND CustomerNumber = @CustomerNumber;", conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerName", customer.Name);
                    cmd.Parameters.AddWithValue("@CustomerAddress", customer.Address);
                    cmd.Parameters.AddWithValue("@CustomerNumber", customer.ContactNumber);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }



        public static int AddCustomer(string name, string address, string contactNumber, string email, string comments, string lastModified, string createdDate)
        {

            using (var conn = GetConnection())
            {
                using (var cmd = new SQLiteCommand("INSERT INTO Customers (CustomerName, CustomerAddress, CustomerNumber, CustomerEmail, CustomerComments, LastModified, CreatedDate) VALUES (@Name, @Address, @ContactNumber, @Email, @Comments, @LastModified, @CreatedDate);", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@ContactNumber", contactNumber);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Comments", comments);                
                    cmd.Parameters.AddWithValue("@LastModified", lastModified);
                    cmd.Parameters.AddWithValue("@CreatedDate", createdDate);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }

        public static void EditCustomer(Customer customer, string name, string address, string contactNumber, string email, string comments, string lastModified)
        {
            int id = customer.CustomerID;
            using (var conn = GetConnection())
            {
                using (var cmd = new SQLiteCommand("""
                    UPDATE Customers SET 
                    CustomerName = @newName, 
                    CustomerAddress = @newAddress,
                    CustomerNumber = @newContactNumber, 
                    CustomerEmail = @newEmail,
                    CustomerComments = @newComments,
                    LastModified = @newLastModified
                    WHERE CustomerID = @id;
                    """, conn))
                {
                    cmd.Parameters.AddWithValue("@newName", name);
                    cmd.Parameters.AddWithValue("@newAddress", address);
                    cmd.Parameters.AddWithValue("@newContactNumber", contactNumber);
                    cmd.Parameters.AddWithValue("@newEmail", email);
                    cmd.Parameters.AddWithValue("@newComments", comments);
                    cmd.Parameters.AddWithValue("@newLastModified", lastModified);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteCustomer(Customer customer)
        {
            int id = customer.CustomerID;
            using (var conn = GetConnection())
            {
                using (var cmd = new SQLiteCommand("DELETE FROM Customers WHERE CustomerID= @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static bool CustomerExists(string name)
        {
            using (var conn = GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Customers WHERE CustomerName = @Name;", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }


        public static ObservableCollection<Customer> SearchCustomers(string search)
        {
            var customersCollection = new ObservableCollection<Customer>();
            using (var conn = GetConnection())
            {
                
                using (var cmd = new SQLiteCommand("SELECT * From Customers WHERE CustomerName LIKE @Search OR CustomerAddress LIKE @Search OR CustomerEmail LIKE @Search;", conn))
                {

                    cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

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

    }
}
