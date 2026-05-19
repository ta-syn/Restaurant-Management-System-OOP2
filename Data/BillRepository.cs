using Microsoft.Data.Sqlite;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data;

public class BillRepository
{
    public static Bill GenerateBill(int orderId)
    {
        var items = OrderRepository.GetOrderItems(orderId);
        decimal subtotal = items.Sum(i => i.Subtotal);

        using var connection = DatabaseConnection.GetConnection();
        connection.Open();

        // Check if bill already exists
        var check = connection.CreateCommand();
        check.CommandText = "SELECT Id FROM Bills WHERE OrderId = @id";
        check.Parameters.AddWithValue("@id", orderId);
        var existing = check.ExecuteScalar();
        if (existing != null)
        {
            return GetBillByOrderId(orderId)!;
        }

        double currentTaxRate = SettingsRepository.GetTaxRate();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Bills (OrderId, Subtotal, TaxRate, IsPaid, BillTime) VALUES (@oid, @sub, @tax, 0, @time); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@oid", orderId);
        cmd.Parameters.AddWithValue("@sub", (double)subtotal);
        cmd.Parameters.AddWithValue("@tax", currentTaxRate);
        cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("o"));
        var newId = Convert.ToInt32(cmd.ExecuteScalar());

        return new Bill { Id = newId, OrderId = orderId, Subtotal = subtotal, TaxRate = currentTaxRate, IsPaid = false, BillTime = DateTime.Now };
    }

    public static Bill? GetBillByOrderId(int orderId)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, OrderId, Subtotal, TaxRate, IsPaid, BillTime, CompletedBy FROM Bills WHERE OrderId = @id";
        cmd.Parameters.AddWithValue("@id", orderId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Bill
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                Subtotal = (decimal)reader.GetDouble(2),
                TaxRate = reader.GetDouble(3),
                IsPaid = reader.GetInt32(4) == 1,
                BillTime = DateTime.Parse(reader.GetString(5)),
                CompletedBy = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }
        return null;
    }

    public static List<Bill> GetAllBills()
    {
        var list = new List<Bill>();
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, OrderId, Subtotal, TaxRate, IsPaid, BillTime, CompletedBy FROM Bills ORDER BY Id DESC";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Bill
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                Subtotal = (decimal)reader.GetDouble(2),
                TaxRate = reader.GetDouble(3),
                IsPaid = reader.GetInt32(4) == 1,
                BillTime = DateTime.Parse(reader.GetString(5)),
                CompletedBy = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        return list;
    }

    public static void MarkAsPaid(int billId, string? waiterName = null)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        if (waiterName != null)
        {
            cmd.CommandText = "UPDATE Bills SET IsPaid=1, BillTime=@time, CompletedBy=@w WHERE Id=@id";
            cmd.Parameters.AddWithValue("@w", waiterName);
        }
        else
        {
            cmd.CommandText = "UPDATE Bills SET IsPaid=1, BillTime=@time WHERE Id=@id";
        }
        cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("o"));
        cmd.Parameters.AddWithValue("@id", billId);
        cmd.ExecuteNonQuery();
    }

    public static void SetCompletedBy(int billId, string waiterName)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Bills SET CompletedBy=@w WHERE Id=@id";
        cmd.Parameters.AddWithValue("@w", waiterName);
        cmd.Parameters.AddWithValue("@id", billId);
        cmd.ExecuteNonQuery();
    }

    public static decimal GetTotalRevenue()
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COALESCE(SUM(Subtotal * (1 + TaxRate/100.0)), 0) FROM Bills WHERE IsPaid=1";
        var result = cmd.ExecuteScalar();
        return Convert.ToDecimal(result);
    }
}
