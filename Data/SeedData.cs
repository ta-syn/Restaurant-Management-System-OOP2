using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Net.Http;

namespace RestaurantManagementSystem.Data;

public class SeedData
{
    public static void InsertDefaultData()
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();

        // Temporarily disable foreign keys for safe reseeding
        var pragmaOff = connection.CreateCommand();
        pragmaOff.CommandText = "PRAGMA foreign_keys = OFF;";
        pragmaOff.ExecuteNonQuery();

        try
        {
            // 1. Seed Users (If Empty)
            var check = connection.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM Users";
            var count = (long)(check.ExecuteScalar() ?? 0);
            if (count == 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Users (Name, Username, Password, Role) VALUES
                    ('Administrator', 'admin', 'admin123', 'Admin'),
                    ('John Waiter', 'john', 'waiter123', 'Waiter'),
                    ('Rafi Customer', 'rafi', 'customer123', 'Customer');
                ";
                cmd.ExecuteNonQuery();
            }

            // 2. Seed Menu Items (Seed exactly 12 fresh, premium food items with downloaded images if empty)
            var checkMenu = connection.CreateCommand();
            checkMenu.CommandText = "SELECT COUNT(*) FROM MenuItems";
            var menuCount = (long)(checkMenu.ExecuteScalar() ?? 0);
            if (menuCount == 0)
            {
                string targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "restaurant_images");
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                var foods = new[]
                {
                    new { Name = "Chicken Burger", Cat = "Fast Food", Price = 180.00, Url = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400&q=80", File = "chicken_burger.jpg" },
                    new { Name = "Beef Steak", Cat = "Main Course", Price = 470.00, Url = "https://images.unsplash.com/photo-1544025162-d76694265947?w=400&q=80", File = "beef_steak.jpg" },
                    new { Name = "Vegetable Pasta", Cat = "Main Course", Price = 220.00, Url = "https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?w=400&q=80", File = "veg_pasta.jpg" },
                    new { Name = "French Fries", Cat = "Sides", Price = 120.00, Url = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=400&q=80", File = "french_fries.jpg" },
                    new { Name = "Chicken Wings", Cat = "Appetizer", Price = 280.00, Url = "https://images.unsplash.com/photo-1567620832903-9fc6debc209f?w=400&q=80", File = "chicken_wings.jpg" },
                    new { Name = "Mango Juice", Cat = "Drinks", Price = 80.00, Url = "https://images.unsplash.com/photo-1600271886742-f049cd451bba?w=400&q=80", File = "mango_juice_fresh.jpg" },
                    new { Name = "Coca Cola", Cat = "Drinks", Price = 60.00, Url = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=400&q=80", File = "coca_cola.jpg" },
                    new { Name = "Chocolate Cake", Cat = "Dessert", Price = 150.00, Url = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=400&q=80", File = "chocolate_cake.jpg" },
                    new { Name = "Fried Chicken", Cat = "Fast Food", Price = 250.00, Url = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?w=400&q=80", File = "fried_chicken.jpg" },
                    new { Name = "Club Sandwich", Cat = "Fast Food", Price = 190.00, Url = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400&q=80", File = "club_sandwich_fresh.jpg" },
                    new { Name = "Garlic Mushroom", Cat = "Appetizer", Price = 210.00, Url = "https://images.unsplash.com/photo-1534422298391-e4f8c172dddb?w=400&q=80", File = "garlic_mushroom_fresh.jpg" },
                    new { Name = "Vanilla Ice Cream", Cat = "Dessert", Price = 100.00, Url = "https://images.unsplash.com/photo-1570197788417-0e82375c9371?w=400&q=80", File = "vanilla_icecream_fresh.jpg" }
                };

                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                foreach (var food in foods)
                {
                    string localPath = Path.Combine(targetDir, food.File);
                    
                    // If file does not exist or is empty (0 bytes), attempt download
                    bool needsDownload = !File.Exists(localPath);
                    if (!needsDownload)
                    {
                        try
                        {
                            var info = new FileInfo(localPath);
                            if (info.Length == 0)
                            {
                                needsDownload = true;
                                File.Delete(localPath); // Remove 0-byte corrupt files
                            }
                        }
                        catch {}
                    }

                    if (needsDownload)
                    {
                        try
                        {
                            var data = client.GetByteArrayAsync(food.Url).GetAwaiter().GetResult();
                            File.WriteAllBytes(localPath, data);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[SeedData] Failed to download {food.Name}: {ex.Message}");
                            localPath = food.Url; // Save the raw remote URL to the database as fallback
                        }
                    }

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO MenuItems (Name, Category, Price, ImagePath, IsAvailable) VALUES (@n, @c, @p, @img, 1);";
                    cmd.Parameters.AddWithValue("@n", food.Name);
                    cmd.Parameters.AddWithValue("@c", food.Cat);
                    cmd.Parameters.AddWithValue("@p", food.Price);
                    cmd.Parameters.AddWithValue("@img", localPath);
                    cmd.ExecuteNonQuery();
                }
            }

            // 3. Seed Tables (If Empty)
            var checkTables = connection.CreateCommand();
            checkTables.CommandText = "SELECT COUNT(*) FROM RestaurantTables";
            var tableCount = (long)(checkTables.ExecuteScalar() ?? 0);
            if (tableCount == 0)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO RestaurantTables (TableNumber, Capacity, Status) VALUES
                    (1, 2, 'Free'), (2, 4, 'Free'), (3, 4, 'Free'),
                    (4, 6, 'Free'), (5, 6, 'Free'), (6, 8, 'Free'),
                    (7, 2, 'Free'), (8, 4, 'Free');
                ";
                cmd.ExecuteNonQuery();
            }
        }
        finally
        {
            // Re-enable foreign key constraints
            var pragmaOn = connection.CreateCommand();
            pragmaOn.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaOn.ExecuteNonQuery();
        }
    }
}
