using Avalonia;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Views;

namespace RestaurantManagementSystem;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            DatabaseConnection.InitializeDatabase();
            SeedData.InsertDefaultData();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
