namespace RestaurantManagementSystem.Models;

public class Waiter : User
{
    public string ShiftTime { get; set; }

    public Waiter(int id, string name, string password, string shiftTime = "Morning")
        : base(id, name, password, "Waiter")
    {
        ShiftTime = shiftTime;
    }

    public override void ShowDashboard()
    {
        Console.WriteLine($"[Waiter Dashboard] Welcome, {Name}. Shift: {ShiftTime}");
    }
}
