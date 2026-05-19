namespace RestaurantManagementSystem.Models;

public class Order
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime OrderTime { get; set; } = DateTime.Now;

    public string? CancelReason { get; set; }
    public string? WaiterName { get; set; }
    public string? ConfirmedBy { get; set; }

    public override string ToString() => $"Order #{Id} | Table {TableId} | Placed by: {WaiterName ?? UserId.ToString()} | {Status} | {OrderTime:g}";
}
