using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RestaurantManagementSystem.Models;
using System;

namespace RestaurantManagementSystem.Views;

public partial class AdminDashboardWindow : Window
{
    private readonly User _currentUser;
    private Panel _contentArea = new StackPanel();
    private Border? _activeBorder;

    public AdminDashboardWindow(User user)
    {
        _currentUser = user;
        Title = "Admin Dashboard — Restaurant Management System";
        Width = 1150;
        Height = 720;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = UIHelper.LightBackgroundBrush;
        BuildUI();
    }

    private void BuildUI()
    {
        var root = new DockPanel();

        // 1. Top bar (60px height, white background, shadow)
        var topBar = new Border
        {
            Height = 60,
            Background = UIHelper.WhiteBrush,
            Padding = new Thickness(24, 0),
            BoxShadow = UIHelper.TopBarShadow,
            ZIndex = 10
        };
        DockPanel.SetDock(topBar, Dock.Top);

        var topContent = new DockPanel();
        
        var appName = new TextBlock
        {
            Text = "🍽️ RESTAURANT MS",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.AccentBrush,
            VerticalAlignment = VerticalAlignment.Center,
            LetterSpacing = 1.0
        };

        // User profile section on the right side
        var profileStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 10
        };
        DockPanel.SetDock(profileStack, Dock.Right);

        var userInfo = new TextBlock
        {
            Text = $"{_currentUser.Name} ({_currentUser.Role})",
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            VerticalAlignment = VerticalAlignment.Center
        };

        var dateText = new TextBlock
        {
            Text = $"|  {DateTime.Now:ddd, MMM dd}",
            FontSize = 12,
            Foreground = UIHelper.GrayBrush,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Custom avatar circle with name initial
        var avatar = new Border
        {
            Width = 36,
            Height = 36,
            CornerRadius = new CornerRadius(18),
            Background = UIHelper.AccentBrush,
            BoxShadow = BoxShadows.Parse("0 2 6 0 #22000000"),
            Child = new TextBlock
            {
                Text = _currentUser.Name[0].ToString().ToUpper(),
                Foreground = UIHelper.WhiteBrush,
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        profileStack.Children.Add(userInfo);
        profileStack.Children.Add(dateText);
        profileStack.Children.Add(avatar);

        topContent.Children.Add(profileStack);
        topContent.Children.Add(appName);
        topBar.Child = topContent;
        root.Children.Add(topBar);

        // 2. Sidebar (220px wide, dark navy #1A1A2E)
        var sidebar = new Border
        {
            Width = 220,
            Background = UIHelper.SidebarBrush,
            Padding = new Thickness(0, 20)
        };
        DockPanel.SetDock(sidebar, Dock.Left);

        var sideStack = new StackPanel { Spacing = 6 };

        var sideTitle = new TextBlock
        {
            Text = "ADMINISTRATIVE CONSOLE",
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.GrayBrush,
            Margin = new Thickness(20, 0, 0, 16),
            LetterSpacing = 1.0
        };
        sideStack.Children.Add(sideTitle);

        string[] panels = { "📋 Menu Items", "👥 Users", "🪑 Tables", "📦 Orders", "💰 Bills", "🔍 Search" };
        foreach (var label in panels)
        {
            var itemBorder = new Border
            {
                BorderThickness = new Thickness(4, 0, 0, 0),
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                Height = 44,
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };

            var labelText = new TextBlock
            {
                Text = label,
                Foreground = UIHelper.WhiteBrush,
                FontSize = 13,
                FontWeight = FontWeight.Medium,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(16, 0)
            };
            itemBorder.Child = labelText;

            var capturedLabel = label;
            itemBorder.PointerPressed += (_, _) =>
            {
                SetActiveBorder(itemBorder);
                LoadPanel(capturedLabel);
            };

            // Custom hover effects
            itemBorder.PointerEntered += (s, e) =>
            {
                if (_activeBorder != itemBorder)
                {
                    itemBorder.Background = new SolidColorBrush(Color.Parse("#16213E"));
                }
            };
            itemBorder.PointerExited += (s, e) =>
            {
                if (_activeBorder != itemBorder)
                {
                    itemBorder.Background = Brushes.Transparent;
                }
            };

            sideStack.Children.Add(itemBorder);
        }

        // Bottom container for Website Link and Logout
        var bottomStack = new StackPanel { Spacing = 6 };
        DockPanel.SetDock(bottomStack, Dock.Bottom);

        // Website Link Button
        var webBorder = new Border
        {
            BorderThickness = new Thickness(4, 0, 0, 0),
            BorderBrush = Brushes.Transparent,
            Background = Brushes.Transparent,
            Height = 44,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };
        var webText = new TextBlock
        {
            Text = "🌐 Go to Website",
            Foreground = UIHelper.BlueBrush,
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(16, 0)
        };
        webBorder.PointerPressed += (_, _) => { new CustomerDashboardWindow(_currentUser).Show(); };
        webBorder.PointerEntered += (s, e) => { webBorder.Background = new SolidColorBrush(Color.Parse("#16213E")); };
        webBorder.PointerExited += (s, e) => { webBorder.Background = Brushes.Transparent; };
        webBorder.Child = webText;
        bottomStack.Children.Add(webBorder);

        // Logout Button
        var logoutBorder = new Border
        {
            BorderThickness = new Thickness(4, 0, 0, 0),
            BorderBrush = Brushes.Transparent,
            Background = Brushes.Transparent,
            Height = 44,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };
        var logoutText = new TextBlock
        {
            Text = "🚪 Logout",
            Foreground = UIHelper.DangerBrush,
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(16, 0)
        };
        logoutBorder.PointerPressed += (_, _) => { new LoginWindow().Show(); Close(); };
        logoutBorder.PointerEntered += (s, e) => { logoutBorder.Background = new SolidColorBrush(Color.Parse("#16213E")); };
        logoutBorder.PointerExited += (s, e) => { logoutBorder.Background = Brushes.Transparent; };
        logoutBorder.Child = logoutText;
        bottomStack.Children.Add(logoutBorder);

        var sidebarDock = new DockPanel();
        sidebarDock.Children.Add(bottomStack);
        sidebarDock.Children.Add(sideStack);

        sidebar.Child = sidebarDock;
        root.Children.Add(sidebar);

        // 3. Content Area (#F8F9FA background)
        _contentArea = new Panel
        {
            Background = UIHelper.LightBackgroundBrush
        };
        root.Children.Add(_contentArea);

        Content = root;

        // Load default panel
        if (sideStack.Children[1] is Border firstBorder)
        {
            SetActiveBorder(firstBorder);
            LoadPanel("📋 Menu Items");
        }
    }

    private void SetActiveBorder(Border newBorder)
    {
        if (_activeBorder != null)
        {
            _activeBorder.BorderBrush = Brushes.Transparent;
            _activeBorder.Background = Brushes.Transparent;
        }
        _activeBorder = newBorder;
        _activeBorder.BorderBrush = UIHelper.AccentBrush;
        _activeBorder.Background = UIHelper.SidebarAccentBrush;
    }

    private void LoadPanel(string label)
    {
        _contentArea.Children.Clear();
        Control panel = label switch
        {
            "📋 Menu Items" => BuildMenuPanel(),
            "👥 Users" => BuildStaffPanel(),
            "🪑 Tables" => BuildTablesPanel(),
            "📦 Orders" => BuildOrdersPanel(),
            "💰 Bills" => BuildBillsPanel(),
            "🔍 Search" => BuildSearchPanel(),
            _ => new TextBlock { Text = "Panel not found" }
        };
        _contentArea.Children.Add(panel);
    }
}
