using Microsoft.Data.Sqlite;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data;

public class TableRepository
{
    public static List<RestaurantTable> GetAllTables()
    {
        var list = new List<RestaurantTable>();
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, TableNumber, Capacity, Status FROM RestaurantTables";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new RestaurantTable
            {
                Id = reader.GetInt32(0),
                TableNumber = reader.GetInt32(1),
                Capacity = reader.GetInt32(2),
                Status = reader.GetString(3)
            });
        }
        return list;
    }

    public static void AddTable(int tableNumber, int capacity)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO RestaurantTables (TableNumber, Capacity, Status) VALUES (@t, @c, 'Free')";
        cmd.Parameters.AddWithValue("@t", tableNumber);
        cmd.Parameters.AddWithValue("@c", capacity);
        cmd.ExecuteNonQuery();
    }

    public static void UpdateTableStatus(int id, string status)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE RestaurantTables SET Status=@s WHERE Id=@id";
        cmd.Parameters.AddWithValue("@s", status);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public static void DeleteTable(int id)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM RestaurantTables WHERE Id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public static void UpdateTableOccupancyStatus(int tableId)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        
        int capacity = 0;
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = "SELECT Capacity FROM RestaurantTables WHERE Id = @id";
        tableCmd.Parameters.AddWithValue("@id", tableId);
        var capResult = tableCmd.ExecuteScalar();
        if (capResult != null && capResult != DBNull.Value)
        {
            capacity = Convert.ToInt32(capResult);
        }

        int activeOrdersCount = 0;
        var orderCmd = connection.CreateCommand();
        orderCmd.CommandText = "SELECT COUNT(*) FROM Orders WHERE TableId = @id AND Status IN ('Pending', 'Confirmed', 'Preparing')";
        orderCmd.Parameters.AddWithValue("@id", tableId);
        var orderResult = orderCmd.ExecuteScalar();
        if (orderResult != null && orderResult != DBNull.Value)
        {
            activeOrdersCount = Convert.ToInt32(orderResult);
        }

        string newStatus = (activeOrdersCount >= capacity) ? "Occupied" : "Free";
        
        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = "UPDATE RestaurantTables SET Status = @status WHERE Id = @id";
        updateCmd.Parameters.AddWithValue("@status", newStatus);
        updateCmd.Parameters.AddWithValue("@id", tableId);
        updateCmd.ExecuteNonQuery();
    }
}
