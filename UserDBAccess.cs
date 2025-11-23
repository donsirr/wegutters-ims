using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using WEGutters.ConstructorClasses;

namespace WEGutters
{
    internal class UserDBAccess
    {
        public static void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();

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

                // 3. Insert Default Admin User (Ignore if ID 1 exists)
                string insertSql = @"
                    INSERT OR IGNORE INTO Users (UserID, Username, Password, FirstName, LastName, Email, AccessLevel, IsActive)
                    VALUES (1, 'admin', 'admin', 'System', 'Admin', 'admin@wegutters.com', 'Admin', 1);
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
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
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
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
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
            {
                UserID = Convert.ToInt32(reader["UserID"]),
                Username = reader["Username"].ToString(),
                PasswordHash = reader["Password"].ToString(),
                FirstName = reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : "",
                LastName = reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : "",
                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                AccessLevel = reader["AccessLevel"] != DBNull.Value ? reader["AccessLevel"].ToString() : "Staff",
                IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToInt32(reader["IsActive"]) == 1 : true
            };
        }

        public static void AddUser(User user)
        {
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                string sql = "INSERT INTO Users (Username, Password, FirstName, LastName, Email, AccessLevel, IsActive) VALUES (@U, @P, @F, @L, @E, @A, @I)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@U", user.Username);
                    cmd.Parameters.AddWithValue("@P", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@F", user.FirstName);
                    cmd.Parameters.AddWithValue("@L", user.LastName);
                    cmd.Parameters.AddWithValue("@E", user.Email);
                    cmd.Parameters.AddWithValue("@A", user.AccessLevel);
                    cmd.Parameters.AddWithValue("@I", user.IsActive ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateUser(User user)
        {
            using (var conn = new SQLiteConnection("Data Source=WesternEdgeDB.db;Version=3;"))
            {
                conn.Open();
                string sql = "UPDATE Users SET Username=@U, FirstName=@F, LastName=@L, Email=@E, AccessLevel=@A, IsActive=@I WHERE UserID=@ID";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@U", user.Username);
                    cmd.Parameters.AddWithValue("@F", user.FirstName);
                    cmd.Parameters.AddWithValue("@L", user.LastName);
                    cmd.Parameters.AddWithValue("@E", user.Email);
                    cmd.Parameters.AddWithValue("@A", user.AccessLevel);
                    cmd.Parameters.AddWithValue("@I", user.IsActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("@ID", user.UserID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}