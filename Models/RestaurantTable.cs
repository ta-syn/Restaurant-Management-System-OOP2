namespace RestaurantManagementSystem.Models;

public class RestaurantTable
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = "Free";

    public override string ToString() => $"Table {TableNumber} | Capacity: {Capacity} | Status: {Status}";
}
