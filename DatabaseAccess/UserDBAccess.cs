using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using WEGutters.CustomerClasses;
using WEGutters.UserClasses;

namespace WEGutters.DatabaseAccess
{
    internal class UserDBAccess
    {
        private static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;");
            conn.Open();

            using (var cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn))
                cmd.ExecuteNonQuery();

            return conn;
        }

        public static void InitializeDatabase()
        {
            using (var conn = GetConnection())
            {
                

                // 1. Ensure Table Exists
                string createSql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE,
                        Password TEXT
                    );";

                using (var cmd = new SQLiteCommand(createSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // 2. Schema Migration: Check for missing columns and add them if needed
                var existingColumns = new List<string>();
                using (var cmd = new SQLiteCommand("PRAGMA table_info(Users)", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            existingColumns.Add(reader["name"].ToString());
                        }
                    }
                }

                if (!existingColumns.Contains("FirstName")) ExecuteSql(conn, "ALTER TABLE Users ADD COLUMN FirstName TEXT");
                if (!existingColumns.Contains("LastName")) ExecuteSql(conn, "ALTER TABLE Users ADD COLUMN LastName TEXT");
                if (!existingColumns.Contains("Email")) ExecuteSql(conn, "ALTER TABLE Users ADD COLUMN Email TEXT");
                if (!existingColumns.Contains("AccessLevel")) ExecuteSql(conn, "ALTER TABLE Users ADD COLUMN AccessLevel TEXT");
                if (!existingColumns.Contains("IsActive")) ExecuteSql(conn, "ALTER TABLE Users ADD COLUMN IsActive INTEGER DEFAULT 1");
                if (!existingColumns.Contains("LastModified")) ExecuteSql(conn, "ALTER TABLE Users ADD COLUMN LastModified TEXT");
                if (!existingColumns.Contains("CreatedDate")) ExecuteSql(conn, "ALTER TABLE Users ADD COLUMN CreatedDate TEXT");

                // 3. Insert Default Admin User (Ignore if ID 1 exists)
                string insertSql = @"
                    INSERT OR IGNORE INTO Users (UserID, Username, Password, FirstName, LastName, Email, AccessLevel, IsActive, LastModified, CreatedDate)
                    VALUES (1, 'admin', 'admin', 'System', 'Admin', 'admin@wegutters.com', 'Admin', 1,'admin', 'admin');
                ";
                using (var cmd = new SQLiteCommand(insertSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void ExecuteSql(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static User Login(string username, string password)
        {
            using (var conn = GetConnection())
            {
                
                // Note: In production, compare hashed passwords!
                using (var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Username = @User AND Password = @Pass AND IsActive = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@User", username);
                    cmd.Parameters.AddWithValue("@Pass", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ParseUserFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public static ObservableCollection<User> GetUsers()
        {
            var list = new ObservableCollection<User>();
            using (var conn = GetConnection())
            {
                
                using (var cmd = new SQLiteCommand("SELECT * FROM Users", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(ParseUserFromReader(reader));
                        }
                    }
                }
            }
            return list;
        }

        private static User ParseUserFromReader(SQLiteDataReader reader)
        {
            // Safely handle potential DBNulls or column index shifts by using column names if preferred, 
            // but here we stick to the known schema order assuming migration ran.
            // If strictness is needed, use reader["ColumnName"]
            return new User
            (
                reader["Username"].ToString(),
                reader["Password"].ToString(),
                reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : "",
                reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : "",
                reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                reader["AccessLevel"] != DBNull.Value ? reader["AccessLevel"].ToString() : "Staff",
                reader["IsActive"] != DBNull.Value ? Convert.ToInt32(reader["IsActive"]) : 1,
                reader["LastModified"] != DBNull.Value ? reader["LastModified"].ToString() : DateTime.Now.ToString("yyyy-MM-dd_HH:mm"),
                reader["CreatedDate"] != DBNull.Value ? reader["LastModified"].ToString() : DateTime.Now.ToString("yyyy-MM-dd_HH:mm")

            ) { UserID = Convert.ToInt32(reader["UserID"]) };
        }

        public static int AddUser(User user)
        {
            using (var conn = GetConnection())
            {
                
                string sql = "INSERT INTO Users (Username, Password, FirstName, LastName, Email, AccessLevel, IsActive, LastModified, CreatedDate) VALUES (@U, @P, @F, @L, @E, @A, @I, @LD, @CD)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@U", user.Username);
                    cmd.Parameters.AddWithValue("@P", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@F", user.FirstName);
                    cmd.Parameters.AddWithValue("@L", user.LastName);
                    cmd.Parameters.AddWithValue("@E", user.Email);
                    cmd.Parameters.AddWithValue("@A", user.AccessLevel);
                    cmd.Parameters.AddWithValue("@I", user.IsActive);
                    cmd.Parameters.AddWithValue("@LD", user.LastModified);
                    cmd.Parameters.AddWithValue("@CD", user.CreatedDate);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());

                    return newId;
                }
            }
        }

        public static void EditUser(User user)
        {
            using (var conn = GetConnection())
            {
                
                string sql = "UPDATE Users SET Username=@U, FirstName=@F, LastName=@L, Email=@E, AccessLevel=@A, IsActive=@I, LastModified=@LD WHERE UserID=@ID";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@U", user.Username);
                    cmd.Parameters.AddWithValue("@F", user.FirstName);
                    cmd.Parameters.AddWithValue("@L", user.LastName);
                    cmd.Parameters.AddWithValue("@E", user.Email);
                    cmd.Parameters.AddWithValue("@A", user.AccessLevel);
                    cmd.Parameters.AddWithValue("@I", user.IsActive);
                    cmd.Parameters.AddWithValue("@LD", user.LastModified);
                    cmd.Parameters.AddWithValue("@ID", user.UserID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteUser(User user)
        {
            int id = user.UserID;
            using (var conn = GetConnection())
            {
                
                using (var cmd = new SQLiteCommand("DELETE FROM Users WHERE UserID= @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public static bool UserExists(string username)
        {
            using (var conn = GetConnection())
            {
                
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Username = @User;", conn))
                {
                    cmd.Parameters.AddWithValue("@User", username);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static bool EmailExists(string email)
        {
            using (var conn = GetConnection())
            {
                
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email;", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}