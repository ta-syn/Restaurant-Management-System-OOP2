namespace RestaurantManagementSystem.Models;

public class Customer : User
{
    public int TableNumber { get; set; }

    public Customer(int id, string name, string password, int tableNumber = 0)
        : base(id, name, password, "Customer")
    {
        TableNumber = tableNumber;
    }

    public override void ShowDashboard()
    {
        Console.WriteLine($"[Customer Dashboard] Welcome, {Name}. Table: {TableNumber}");
    }
}
