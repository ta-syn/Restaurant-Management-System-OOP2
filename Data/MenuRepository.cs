using Microsoft.Data.Sqlite;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data;

public class MenuRepository
{
    public static List<MenuItem> GetAllMenuItems()
    {
        var list = new List<MenuItem>();
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Category, Price, ImagePath, IsAvailable FROM MenuItems";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new MenuItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Category = reader.GetString(2),
                Price = (decimal)reader.GetDouble(3),
                ImagePath = reader.IsDBNull(4) ? "" : reader.GetString(4),
                IsAvailable = reader.GetInt32(5) == 1
            });
        }
        return list;
    }

    public static void AddMenuItem(string name, string category, decimal price, string imagePath = "")
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO MenuItems (Name, Category, Price, ImagePath, IsAvailable) VALUES (@n, @c, @p, @img, 1)";
        cmd.Parameters.AddWithValue("@n", name);
        cmd.Parameters.AddWithValue("@c", category);
        cmd.Parameters.AddWithValue("@p", (double)price);
        cmd.Parameters.AddWithValue("@img", imagePath ?? "");
        cmd.ExecuteNonQuery();
    }

    public static void UpdateMenuItem(int id, string name, string category, decimal price, string imagePath = "")
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE MenuItems SET Name=@n, Category=@c, Price=@p, ImagePath=@img WHERE Id=@id";
        cmd.Parameters.AddWithValue("@n", name);
        cmd.Parameters.AddWithValue("@c", category);
        cmd.Parameters.AddWithValue("@p", (double)price);
        cmd.Parameters.AddWithValue("@img", imagePath ?? "");
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public static void DeleteMenuItem(int id)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM MenuItems WHERE Id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}
