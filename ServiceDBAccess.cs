using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEGutters.ConstructorClasses;

namespace WEGutters
{
    internal class ServiceDBAccess
    {
        public static ObservableCollection<ServiceCategory> GetServiceCategories()
        {
            var serviceCategoriesCollection = new ObservableCollection<ServiceCategory>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * From ServiceCategories;", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var serviceCategory = new ServiceCategory(
                                reader.GetString(1) // CategoryName
                            )
                            { ServiceCategoryID = reader.GetInt32(0) }; // CategoryID since it's not in the constructor

                            serviceCategoriesCollection.Add(serviceCategory);
                        }
                    }
                }
                return serviceCategoriesCollection;
            }
        }

        public static ObservableCollection<Service> GetServices()
        {
            var serviceCollection = new ObservableCollection<Service>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {

                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                    SELECT
                                                        s.ServiceID, 

                                                        c.CustomerID, 
                                                        c.CustomerName,
                                                        c.CustomerAddress,
                                                        c.CustomerNumber,
                                                        c.CustomerEmail,
                                                        c.CustomerComments,
                                                        c.LastModified,
                                                        c.CreatedDate,
                                                        
                                                        s.ServiceDetails,
                                                        
                                                        sc.ServiceCategoryID, 
                                                        sc.ServiceCategoryName,
                                                        
                                                        s.MaterialCost,
                                                        s.InvoicePrice,
                                                        s.Details,
                                                        s.LastModified,
                                                        s.CreatedDate
                                                    FROM Services s
                                                    JOIN ServiceCategories sc ON s.ServiceCategoryID = sc.ServiceCategoryID
                                                    JOIN Customers c ON s.CustomerID = c.CustomerID;
                                                    """, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customer = new Customer(reader.GetString(2),
                                                        reader.GetString(3),
                                                        reader.GetString(4),
                                                        reader.GetString(5),
                                                        reader.GetString(6),
                                                        reader.GetString(7),
                                                        reader.GetString(8))
                            { CustomerID = reader.GetInt32(1) }; // CustomerID since it's not in the constructor

                            var serviceCategory = new ServiceCategory(reader.GetString(11)) // CategoryName
                            { ServiceCategoryID = reader.GetInt32(10) }; // ServiceCategoryID since it's not in the constructor

                            var service = new Service(
                                customer, // customer
                                reader.GetString(9), // serviceDetails
                                serviceCategory, // servicecategory
                                reader.GetFloat(12), // material cost
                                reader.GetFloat(13), // invoicePrice
                                reader.GetString(14), // details
                                reader.GetString(15), // lastModified
                                reader.GetString(16)  //created date
                            )
                            { ServiceID = reader.GetInt32(0) }; // ItemID since it's not in the constructor

                            serviceCollection.Add(service);
                        }
                    }
                }
                return serviceCollection;
            }
        }


        public static int AddServiceCategory(string serviceCategoryName)
        {

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO ServiceCategories (ServiceCategoryName) VALUES (@ServiceCategoryName);", conn))
                {
                    cmd.Parameters.AddWithValue("@ServiceCategoryName", serviceCategoryName);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }
        public static int AddService(Customer customer, string serviceDetails, ServiceCategory serviceCategory, float materialCost, float invoicePrice, string details, string lastModified, string createdDate)
        {

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT INTO Services (CustomerID, ServiceDetails, ServiceCategoryID, MaterialCost, InvoicePrice, Details, LastModified, CreatedDate) VALUES (@CustomerID, @ServiceDetails, @ServiceCategoryID, @MaterialCost, @InvoicePrice, @Details, @LastModified, @CreatedDate);", conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", CustomerDBAccess.GetCustomerID(customer));
                    cmd.Parameters.AddWithValue("@ServiceDetails", serviceDetails);
                    cmd.Parameters.AddWithValue("@ServiceCategoryID", GetServiceCategoryID(serviceCategory));
                    cmd.Parameters.AddWithValue("@MaterialCost", materialCost);
                    cmd.Parameters.AddWithValue("@InvoicePrice", invoicePrice);
                    cmd.Parameters.AddWithValue("@Details", details);
                    cmd.Parameters.AddWithValue("@LastModified", lastModified);
                    cmd.Parameters.AddWithValue("@CreatedDate", createdDate);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }


        public static void EditService(Service service, Customer customer, string serviceDetails, ServiceCategory serviceCategory, float materialCost, float invoicePrice, string details, string lastModified)
        {
            int id = service.ServiceID;
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("""
                    UPDATE Services SET 
                    CustomerID = @newCustomerID, 
                    ServiceDetails = @newServiceDetails,
                    ServiceCategoryID = @newServiceCategoryID, 
                    MaterialCost = @newMaterialCost,
                    InvoicePrice = @newInvoicePrice, 
                    Details = @newDetails,
                    LastModified = @newLastModified
                    WHERE ServiceID = @id;
                    """, conn))
                {
                    cmd.Parameters.AddWithValue("@newCustomerID", CustomerDBAccess.GetCustomerID(customer));
                    cmd.Parameters.AddWithValue("@newServiceDetails", serviceDetails);
                    cmd.Parameters.AddWithValue("@newServiceCategoryID", GetServiceCategoryID(serviceCategory));
                    cmd.Parameters.AddWithValue("@newMaterialCost", materialCost);
                    cmd.Parameters.AddWithValue("@newInvoicePrice", invoicePrice);
                    cmd.Parameters.AddWithValue("@newDetails", details);
                    cmd.Parameters.AddWithValue("@newLastModified", lastModified);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteService(Service service)
        {
            int id = service.ServiceID;
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Services WHERE ServiceID= @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public static int GetServiceCategoryID(ServiceCategory serviceCategory)
        {
            if (serviceCategory.ServiceCategoryID != 0) { return serviceCategory.ServiceCategoryID; }

            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT ServiceCategoryID FROM ServiceCategories WHERE ServiceCategoryName = @ServiceCategoryName;", conn))
                {
                    cmd.Parameters.AddWithValue("@ServiceCategoryName", serviceCategory.ServiceCategoryName);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }



        public static bool ServiceCategoryExists(string serviceCategoryName)
        {
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                  SELECT COUNT(*) FROM ServiceCategories 
                                                  WHERE ServiceCategoryName = @ServiceCategoryName
                                                  """, conn))
                {
                    cmd.Parameters.AddWithValue("@ServiceCategoryName", serviceCategoryName);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static ObservableCollection<Service> SearchServices(string search)
        {
            var serviceCollection = new ObservableCollection<Service>();
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {

                conn.Open();
                using (var cmd = new SQLiteCommand("""
                                                    SELECT
                                                        s.ServiceID, 

                                                        c.CustomerID, 
                                                        c.CustomerName,
                                                        c.CustomerAddress,
                                                        c.CustomerNumber,
                                                        c.CustomerEmail,
                                                        c.CustomerComments,
                                                        c.LastModified,
                                                        c.CreatedDate,
                                                        
                                                        s.ServiceDetails,
                                                        
                                                        sc.ServiceCategoryID, 
                                                        sc.ServiceCategoryName,
                                                        
                                                        s.MaterialCost,
                                                        s.InvoicePrice,
                                                        s.Details,
                                                        s.LastModified,
                                                        s.CreatedDate
                                                    FROM Services s
                                                    JOIN ServiceCategories sc ON s.ServiceCategoryID = sc.ServiceCategoryID
                                                    JOIN Customers c ON s.CustomerID = c.CustomerID
                                                    WHERE c.CustomerName LIKE @Search OR sc.ServiceCategoryName LIKE @Search OR s.CustomerAddress LIKE @Search;
                                                    """, conn))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customer = new Customer(reader.GetString(2),
                                                        reader.GetString(3),
                                                        reader.GetString(4),
                                                        reader.GetString(5),
                                                        reader.GetString(6),
                                                        reader.GetString(7),
                                                        reader.GetString(8))
                            { CustomerID = reader.GetInt32(1) }; // CustomerID since it's not in the constructor

                            var serviceCategory = new ServiceCategory(reader.GetString(11)) // CategoryName
                            { ServiceCategoryID = reader.GetInt32(10) }; // ServiceCategoryID since it's not in the constructor

                            var service = new Service(
                                customer, // customer
                                reader.GetString(9), // serviceDetails
                                serviceCategory, // servicecategory
                                reader.GetFloat(12), // material cost
                                reader.GetFloat(13), // invoicePrice
                                reader.GetString(14), // details
                                reader.GetString(15), // lastModified
                                reader.GetString(16)  //created date
                            )
                            { ServiceID = reader.GetInt32(0) }; // ItemID since it's not in the constructor

                            serviceCollection.Add(service);

                        }
                    }
                }
                return serviceCollection;
            }
        }





    }
}
