using Microsoft.Data.Sqlite;

namespace RestaurantManagementSystem.Data;

public class DatabaseConnection
{
    private static readonly string DbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "restaurant_management.db");

    public static string ConnectionString => $"Data Source={DbPath}";

    public static SqliteConnection GetConnection()
    {
        return new SqliteConnection(ConnectionString);
    }

    public static void InitializeDatabase()
    {
        using var connection = GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Role TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS MenuItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Category TEXT NOT NULL,
                Price REAL NOT NULL,
                ImagePath TEXT,
                IsAvailable INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS RestaurantTables (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TableNumber INTEGER NOT NULL UNIQUE,
                Capacity INTEGER NOT NULL,
                Status TEXT NOT NULL DEFAULT 'Free'
            );

            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TableId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Status TEXT NOT NULL DEFAULT 'Pending',
                OrderTime TEXT NOT NULL,
                CancelReason TEXT,
                FOREIGN KEY (TableId) REFERENCES RestaurantTables(Id),
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            );

            CREATE TABLE IF NOT EXISTS OrderItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                MenuItemId INTEGER NOT NULL,
                MenuItemName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL,
                FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id)
            );

            CREATE TABLE IF NOT EXISTS Bills (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL UNIQUE,
                Subtotal REAL NOT NULL,
                TaxRate REAL NOT NULL DEFAULT 10.0,
                IsPaid INTEGER NOT NULL DEFAULT 0,
                BillTime TEXT NOT NULL,
                FOREIGN KEY (OrderId) REFERENCES Orders(Id)
            );

            CREATE TABLE IF NOT EXISTS SystemSettings (
                SettingKey TEXT PRIMARY KEY,
                SettingValue TEXT NOT NULL
            );
        ";
        cmd.ExecuteNonQuery();

        // Seed default settings if not exists
        var seedSettingsCmd = connection.CreateCommand();
        seedSettingsCmd.CommandText = @"
            INSERT OR IGNORE INTO SystemSettings (SettingKey, SettingValue)
            VALUES ('TaxRate', '10.0');
        ";
        seedSettingsCmd.ExecuteNonQuery();

        // Proactive migration for existing databases
        try
        {
            var migrateCmd = connection.CreateCommand();
            migrateCmd.CommandText = "ALTER TABLE Orders ADD COLUMN CancelReason TEXT;";
            migrateCmd.ExecuteNonQuery();
        }
        catch (SqliteException) {}

        try
        {
            var migrateCmd2 = connection.CreateCommand();
            migrateCmd2.CommandText = "ALTER TABLE Bills ADD COLUMN TaxRate REAL NOT NULL DEFAULT 10.0;";
            migrateCmd2.ExecuteNonQuery();
        }
        catch (SqliteException) {}

        try
        {
            var migrateCmd3 = connection.CreateCommand();
            migrateCmd3.CommandText = "ALTER TABLE MenuItems ADD COLUMN ImagePath TEXT;";
            migrateCmd3.ExecuteNonQuery();
        }
        catch (SqliteException) {}

        try
        {
            var migrateCmd4 = connection.CreateCommand();
            migrateCmd4.CommandText = "ALTER TABLE Bills ADD COLUMN CompletedBy TEXT;";
            migrateCmd4.ExecuteNonQuery();
        }
        catch (SqliteException) {}

        try
        {
            var migrateCmd5 = connection.CreateCommand();
            migrateCmd5.CommandText = "ALTER TABLE Orders ADD COLUMN ConfirmedBy TEXT;";
            migrateCmd5.ExecuteNonQuery();
        }
        catch (SqliteException) {}
    }
}
