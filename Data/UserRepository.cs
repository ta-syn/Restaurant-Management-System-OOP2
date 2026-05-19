using Microsoft.Data.Sqlite;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data;

public class UserRepository
{
    public static User? GetUserByCredentials(string username, string password)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Username, Password, Role FROM Users WHERE Username = @u AND Password = @p";
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string pass = reader.GetString(3);
            string role = reader.GetString(4);
            return role switch
            {
                "Admin" => new Admin(id, name, pass),
                "Waiter" => new Waiter(id, name, pass),
                "Customer" => new Customer(id, name, pass),
                _ => null
            };
        }
        return null;
    }

    public static List<Models.User> GetAllUsers()
    {
        var list = new List<Models.User>();
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Username, Password, Role FROM Users";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string pass = reader.GetString(3);
            string role = reader.GetString(4);
            Models.User? user = role switch
            {
                "Admin" => new Admin(id, name, pass),
                "Waiter" => new Waiter(id, name, pass),
                "Customer" => new Customer(id, name, pass),
                _ => null
            };
            if (user != null) list.Add(user);
        }
        return list;
    }

    public static void AddUser(string name, string username, string password, string role)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO Users (Name, Username, Password, Role) VALUES (@n, @u, @p, @r)";
        cmd.Parameters.AddWithValue("@n", name);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        cmd.Parameters.AddWithValue("@r", role);
        cmd.ExecuteNonQuery();
    }

    public static void DeleteUser(int id)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Users WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}
