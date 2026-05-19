namespace RestaurantManagementSystem.Models;

public class Admin : User
{
    public string AdminCode { get; private set; }

    public Admin(int id, string name, string password, string adminCode = "ADMIN001")
        : base(id, name, password, "Admin")
    {
        AdminCode = adminCode;
    }

    public override void ShowDashboard()
    {
        Console.WriteLine($"[Admin Dashboard] Welcome, {Name}. Code: {AdminCode}");
    }
}
