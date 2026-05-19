using Microsoft.Data.Sqlite;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data;

public class OrderRepository
{
    public static int CreateOrder(int tableId, int userId)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Orders (TableId, UserId, Status, OrderTime) VALUES (@t, @u, 'Pending', @time); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@t", tableId);
        cmd.Parameters.AddWithValue("@u", userId);
        cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("o"));
        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    public static void AddOrderItem(int orderId, int menuItemId, string menuItemName, int quantity, decimal unitPrice)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO OrderItems (OrderId, MenuItemId, MenuItemName, Quantity, UnitPrice) VALUES (@oid, @mid, @name, @qty, @price)";
        cmd.Parameters.AddWithValue("@oid", orderId);
        cmd.Parameters.AddWithValue("@mid", menuItemId);
        cmd.Parameters.AddWithValue("@name", menuItemName);
        cmd.Parameters.AddWithValue("@qty", quantity);
        cmd.Parameters.AddWithValue("@price", (double)unitPrice);
        cmd.ExecuteNonQuery();
    }

    public static List<Order> GetAllOrders()
    {
        var list = new List<Order>();
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT o.Id, o.TableId, o.UserId, o.Status, o.OrderTime, o.CancelReason, u.Name, o.ConfirmedBy FROM Orders o LEFT JOIN Users u ON o.UserId = u.Id ORDER BY o.Id DESC";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Order
            {
                Id = reader.GetInt32(0),
                TableId = reader.GetInt32(1),
                UserId = reader.GetInt32(2),
                Status = reader.GetString(3),
                OrderTime = DateTime.Parse(reader.GetString(4)),
                CancelReason = reader.IsDBNull(5) ? null : reader.GetString(5),
                WaiterName = reader.IsDBNull(6) ? null : reader.GetString(6),
                ConfirmedBy = reader.IsDBNull(7) ? null : reader.GetString(7)
            });
        }
        return list;
    }

    public static Order? GetOrderById(int orderId)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT o.Id, o.TableId, o.UserId, o.Status, o.OrderTime, o.CancelReason, u.Name, o.ConfirmedBy FROM Orders o LEFT JOIN Users u ON o.UserId = u.Id WHERE o.Id = @id";
        cmd.Parameters.AddWithValue("@id", orderId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Order
            {
                Id = reader.GetInt32(0),
                TableId = reader.GetInt32(1),
                UserId = reader.GetInt32(2),
                Status = reader.GetString(3),
                OrderTime = DateTime.Parse(reader.GetString(4)),
                CancelReason = reader.IsDBNull(5) ? null : reader.GetString(5),
                WaiterName = reader.IsDBNull(6) ? null : reader.GetString(6),
                ConfirmedBy = reader.IsDBNull(7) ? null : reader.GetString(7)
            };
        }
        return null;
    }

    public static void SetOrderConfirmedBy(int orderId, string waiterName)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Orders SET ConfirmedBy = @w WHERE Id = @id";
        cmd.Parameters.AddWithValue("@w", waiterName);
        cmd.Parameters.AddWithValue("@id", orderId);
        cmd.ExecuteNonQuery();
    }

    public static List<OrderItem> GetOrderItems(int orderId)
    {
        var list = new List<OrderItem>();
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, OrderId, MenuItemId, MenuItemName, Quantity, UnitPrice FROM OrderItems WHERE OrderId = @id";
        cmd.Parameters.AddWithValue("@id", orderId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new OrderItem
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                MenuItemId = reader.GetInt32(2),
                MenuItemName = reader.GetString(3),
                Quantity = reader.GetInt32(4),
                UnitPrice = (decimal)reader.GetDouble(5)
            });
        }
        return list;
    }

    public static void UpdateOrderStatus(int orderId, string status)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Orders SET Status=@s WHERE Id=@id";
        cmd.Parameters.AddWithValue("@s", status);
        cmd.Parameters.AddWithValue("@id", orderId);
        cmd.ExecuteNonQuery();
    }

    public static void CancelOrder(int orderId, string reason)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Orders SET Status='Canceled', CancelReason=@r WHERE Id=@id";
        cmd.Parameters.AddWithValue("@r", reason);
        cmd.Parameters.AddWithValue("@id", orderId);
        cmd.ExecuteNonQuery();
    }
}
