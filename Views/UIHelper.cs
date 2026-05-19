using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace RestaurantManagementSystem.Views;

public static class UIHelper
{
    // Overhauled Theme Colors
    public static IBrush LightBackgroundBrush => new SolidColorBrush(Color.Parse("#F8F9FA"));
    public static IBrush SidebarBrush => new SolidColorBrush(Color.Parse("#1A1A2E"));
    public static IBrush SidebarAccentBrush => new SolidColorBrush(Color.Parse("#16213E"));
    public static IBrush AccentBrush => new SolidColorBrush(Color.Parse("#E94560"));
    public static IBrush WhiteBrush => new SolidColorBrush(Colors.White);
    public static IBrush GreenBrush => new SolidColorBrush(Color.Parse("#2ECC71"));
    public static IBrush BlueBrush => new SolidColorBrush(Color.Parse("#3498DB"));
    public static IBrush DarkBrush => new SolidColorBrush(Color.Parse("#2C3E50"));
    public static IBrush GrayBrush => new SolidColorBrush(Color.Parse("#7F8C8D"));
    public static IBrush DangerBrush => new SolidColorBrush(Color.Parse("#E74C3C"));
    public static IBrush LightGrayBrush => new SolidColorBrush(Color.Parse("#F1F3F5"));
    public static IBrush BorderBrush => new SolidColorBrush(Color.Parse("#E9ECEF"));

    // Subtle premium shadows
    public static BoxShadows CardShadow => BoxShadows.Parse("0 4 12 0 #0D000000");
    public static BoxShadows GlowShadow => BoxShadows.Parse("0 8 32 0 #33000000");
    public static BoxShadows TopBarShadow => BoxShadows.Parse("0 2 10 0 #08000000");

    // Dynamic color picker for categories
    public static IBrush GetCategoryBrush(string category)
    {
        return category.ToLower() switch
        {
            "fast food" => new SolidColorBrush(Color.Parse("#E67E22")), // orange
            "main course" => new SolidColorBrush(Color.Parse("#9B59B6")), // purple
            "drinks" => new SolidColorBrush(Color.Parse("#3498DB")), // blue
            "dessert" => new SolidColorBrush(Color.Parse("#E91E63")), // pink
            "appetizer" => new SolidColorBrush(Color.Parse("#1ABC9C")), // turquoise
            "sides" => new SolidColorBrush(Color.Parse("#F1C40F")), // yellow
            _ => new SolidColorBrush(Color.Parse("#95A5A6")) // gray
        };
    }

    
    private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

    static UIHelper()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
    }

    public static Avalonia.Media.Imaging.Bitmap? LoadBitmap(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        try
        {
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                string targetDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "restaurant_images");
                if (!System.IO.Directory.Exists(targetDir))
                {
                    System.IO.Directory.CreateDirectory(targetDir);
                }

                string ext = System.IO.Path.GetExtension(path);
                if (string.IsNullOrEmpty(ext) || ext.Length > 5 || ext.Contains("?") || ext.Contains("&")) ext = ".png";

                using var sha = System.Security.Cryptography.SHA256.Create();
                byte[] hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(path));
                string hexName = Convert.ToHexString(hashBytes).ToLower() + ext;
                string cachedPath = System.IO.Path.Combine(targetDir, hexName);

                if (System.IO.File.Exists(cachedPath))
                {
                    try
                    {
                        return new Avalonia.Media.Imaging.Bitmap(cachedPath);
                    }
                    catch
                    {
                        try { System.IO.File.Delete(cachedPath); } catch {}
                    }
                }

                // Download in background
                System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        var responseBytes = await _httpClient.GetByteArrayAsync(path);
                        await System.IO.File.WriteAllBytesAsync(cachedPath, responseBytes);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[UIHelper] Background download failed: {ex.Message}");
                    }
                });

                return null;
            }
            else if (System.IO.File.Exists(path))
            {
                return new Avalonia.Media.Imaging.Bitmap(path);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UIHelper] Failed to load image: {ex.Message}");
        }

        return null;
    }

    public static string GetCategoryEmoji(string category)
    {
        return category.ToLower() switch
        {
            "fast food" => "🍔",
            "main course" => "🍛",
            "drinks" => "🍹",
            "dessert" => "🍰",
            "appetizer" => "🥗",
            "sides" => "🍟",
            _ => "🍽️"
        };
    }
}
