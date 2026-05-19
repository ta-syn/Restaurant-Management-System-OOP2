namespace RestaurantManagementSystem.Models;

public class Bill
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Subtotal { get; set; }
    public double TaxRate { get; set; } = 10.0;
    public decimal Tax => Subtotal * (decimal)(TaxRate / 100.0);
    public decimal GrandTotal => Subtotal + Tax;
    public bool IsPaid { get; set; } = false;
    public DateTime BillTime { get; set; } = DateTime.Now;

    public string? CompletedBy { get; set; }

    public override string ToString() => $"Bill #{Id} | Order #{OrderId} | Total: ৳{GrandTotal:F2} | {(IsPaid ? "Paid" : "Unpaid")}";
}
