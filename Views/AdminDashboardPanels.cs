using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Linq;
using System.Collections.Generic;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Views;

public partial class AdminDashboardWindow
{
    private Control BuildMenuPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };

        // Header Section
        var headerPanel = new DockPanel { LastChildFill = false };
        var title = new TextBlock { Text = "Menu Items Catalog", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, VerticalAlignment = VerticalAlignment.Center };
        
        var addBtn = new Button
        {
            Content = "➕ Add Menu Item",
            Background = UIHelper.GreenBrush,
            Foreground = UIHelper.WhiteBrush,
            Padding = new Thickness(16, 10),
            CornerRadius = new CornerRadius(8),
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0)
        };
        DockPanel.SetDock(addBtn, Dock.Right);
        headerPanel.Children.Add(addBtn);
        headerPanel.Children.Add(title);
        topStack.Children.Add(headerPanel);

        // Filter / Search Toolbar
        var toolbarPanel = new DockPanel { LastChildFill = false };
        var searchBox = new TextBox
        {
            Watermark = "🔍 Search menu items...",
            Width = 320,
            Padding = new Thickness(12, 10),
            Background = UIHelper.WhiteBrush,
            BorderBrush = UIHelper.BorderBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8)
        };
        DockPanel.SetDock(searchBox, Dock.Left);
        toolbarPanel.Children.Add(searchBox);
        topStack.Children.Add(toolbarPanel);

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        // Cards grid
        var grid = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Left };
        var scroll = new ScrollViewer { Content = grid };
        mainDock.Children.Add(scroll);

        void RefreshMenu(string filter = "")
        {
            grid.Children.Clear();
            var items = MenuRepository.GetAllMenuItems();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(filter) && 
                    !item.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) && 
                    !item.Category.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Card container
                var card = new Border
                {
                    Width = 240,
                    Height = 330,
                    Background = UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(14),
                    Margin = new Thickness(0, 0, 16, 16),
                    BoxShadow = UIHelper.CardShadow
                };

                var cardStack = new StackPanel { Spacing = 8 };

                // Image container inside card
                var imageContainer = new Border
                {
                    Height = 110,
                    CornerRadius = new CornerRadius(8),
                    ClipToBounds = true,
                    Background = UIHelper.LightGrayBrush,
                    Margin = new Thickness(0, 0, 0, 4)
                };

                var bitmap = UIHelper.LoadBitmap(item.ImagePath);
                if (bitmap != null)
                {
                    imageContainer.Child = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill
                    };
                }
                else
                {
                    imageContainer.Child = new TextBlock
                    {
                        Text = UIHelper.GetCategoryEmoji(item.Category),
                        FontSize = 38,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }

                // Name
                var nameText = new TextBlock
                {
                    Text = item.Name,
                    FontSize = 14,
                    FontWeight = FontWeight.Bold,
                    Foreground = UIHelper.DarkBrush,
                    TextWrapping = TextWrapping.Wrap,
                    Height = 36
                };

                // Category & Availability row
                var badgeRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
                var catBadge = new Border
                {
                    Background = UIHelper.GetCategoryBrush(item.Category),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 4),
                    Child = new TextBlock { Text = item.Category, FontSize = 10, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                };
                var statusBadge = new Border
                {
                    Background = item.IsAvailable ? UIHelper.GreenBrush : UIHelper.DangerBrush,
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 4),
                    Child = new TextBlock { Text = item.IsAvailable ? "Available" : "Unavailable", FontSize = 10, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                };
                badgeRow.Children.Add(catBadge);
                badgeRow.Children.Add(statusBadge);

                // Price
                var priceText = new TextBlock
                {
                    Text = $"৳{item.Price:F2}",
                    FontSize = 16,
                    FontWeight = FontWeight.Bold,
                    Foreground = UIHelper.AccentBrush
                };

                // Actions row
                var actionRow = new UniformGrid { Columns = 2 };
                var editBtn = new Button
                {
                    Content = "✏️ Edit",
                    Background = UIHelper.BlueBrush,
                    Foreground = UIHelper.WhiteBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(0, 6),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0, 0, 4, 0)
                };
                var deleteBtn = new Button
                {
                    Content = "🗑️ Delete",
                    Background = UIHelper.DangerBrush,
                    Foreground = UIHelper.WhiteBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(0, 6),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(4, 0, 0, 0)
                };

                var capItem = item;
                editBtn.Click += async (_, _) =>
                {
                    var dialog = new MenuItemDialog(capItem);
                    await dialog.ShowDialog(this);
                    RefreshMenu(searchBox.Text ?? "");
                };
                deleteBtn.Click += (_, _) =>
                {
                    MenuRepository.DeleteMenuItem(capItem.Id);
                    RefreshMenu(searchBox.Text ?? "");
                };

                actionRow.Children.Add(editBtn);
                actionRow.Children.Add(deleteBtn);

                cardStack.Children.Add(imageContainer);
                cardStack.Children.Add(nameText);
                cardStack.Children.Add(badgeRow);
                cardStack.Children.Add(priceText);
                cardStack.Children.Add(actionRow);

                card.Child = cardStack;
                grid.Children.Add(card);
            }

            if (grid.Children.Count == 0)
            {
                grid.Children.Add(new TextBlock
                {
                    Text = "No items found matching the search.",
                    Foreground = UIHelper.GrayBrush,
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0)
                });
            }
        }

        RefreshMenu();
        searchBox.TextChanged += (_, _) => RefreshMenu(searchBox.Text ?? "");

        addBtn.Click += async (_, _) =>
        {
            var dialog = new MenuItemDialog();
            await dialog.ShowDialog(this);
            RefreshMenu(searchBox.Text ?? "");
        };

        return mainDock;
    }

    private Control BuildStaffPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };

        // Header Section
        var headerPanel = new DockPanel { LastChildFill = false };
        var title = new TextBlock { Text = "Users & Staff", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, VerticalAlignment = VerticalAlignment.Center };
        
        var addBtn = new Button
        {
            Content = "➕ Add New User",
            Background = UIHelper.GreenBrush,
            Foreground = UIHelper.WhiteBrush,
            Padding = new Thickness(16, 10),
            CornerRadius = new CornerRadius(8),
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0)
        };
        DockPanel.SetDock(addBtn, Dock.Right);
        headerPanel.Children.Add(addBtn);
        headerPanel.Children.Add(title);
        topStack.Children.Add(headerPanel);

        // Search Box
        var toolbarPanel = new DockPanel { LastChildFill = false };
        var searchBox = new TextBox
        {
            Watermark = "🔍 Search users and staff...",
            Width = 320,
            Padding = new Thickness(12, 10),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            Background = UIHelper.WhiteBrush,
            BorderBrush = UIHelper.BorderBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8)
        };
        DockPanel.SetDock(searchBox, Dock.Left);
        toolbarPanel.Children.Add(searchBox);
        topStack.Children.Add(toolbarPanel);

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        // Staff List Container
        var listStack = new StackPanel { Spacing = 12 };
        var scroll = new ScrollViewer { Content = listStack };
        mainDock.Children.Add(scroll);

        void RefreshStaff(string filter = "")
        {
            listStack.Children.Clear();
            var users = UserRepository.GetAllUsers();
            foreach (var u in users)
            {
                if (!string.IsNullOrEmpty(filter) && 
                    !u.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) && 
                    !u.Role.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Row Border Container
                var rowBorder = new Border
                {
                    Background = UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(16, 12),
                    BoxShadow = UIHelper.CardShadow
                };

                var rowContent = new DockPanel();

                // Left Section: Avatar and Info
                var leftPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, VerticalAlignment = VerticalAlignment.Center };
                
                // Color by role
                var roleColor = u.Role.ToLower() switch
                {
                    "admin" => UIHelper.AccentBrush,
                    "waiter" => UIHelper.BlueBrush,
                    _ => UIHelper.GreenBrush
                };

                var userAvatar = new Border
                {
                    Width = 40,
                    Height = 40,
                    CornerRadius = new CornerRadius(20),
                    Background = roleColor,
                    Child = new TextBlock
                    {
                        Text = u.Name[0].ToString().ToUpper(),
                        Foreground = UIHelper.WhiteBrush,
                        FontSize = 15,
                        FontWeight = FontWeight.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                var textDetails = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Spacing = 2 };
                var nameText = new TextBlock { Text = u.Name, FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
                var roleText = new TextBlock { Text = u.Role, FontSize = 12, Foreground = UIHelper.GrayBrush };
                textDetails.Children.Add(nameText);
                textDetails.Children.Add(roleText);

                leftPanel.Children.Add(userAvatar);
                leftPanel.Children.Add(textDetails);

                // Right Section: Delete Button
                var deleteBtn = new Button
                {
                    Content = "🗑️ Remove",
                    Background = UIHelper.DangerBrush,
                    Foreground = UIHelper.WhiteBrush,
                    Padding = new Thickness(12, 8),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 12,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                DockPanel.SetDock(deleteBtn, Dock.Right);

                var capUser = u;
                deleteBtn.Click += (_, _) =>
                {
                    UserRepository.DeleteUser(capUser.Id);
                    RefreshStaff(searchBox.Text ?? "");
                };

                // Add right-docked elements FIRST to prevent clipping/overlapping!
                rowContent.Children.Add(deleteBtn);
                rowContent.Children.Add(leftPanel);
                rowBorder.Child = rowContent;
                listStack.Children.Add(rowBorder);
            }

            if (listStack.Children.Count == 0)
            {
                listStack.Children.Add(new TextBlock
                {
                    Text = "No staff members found.",
                    Foreground = UIHelper.GrayBrush,
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0)
                });
            }
        }

        RefreshStaff();
        searchBox.TextChanged += (_, _) => RefreshStaff(searchBox.Text ?? "");

        addBtn.Click += async (_, _) =>
        {
            var dialog = new AddUserDialog();
            await dialog.ShowDialog(this);
            RefreshStaff(searchBox.Text ?? "");
        };

        return mainDock;
    }

    private Control BuildTablesPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };

        // Header Section
        var headerPanel = new DockPanel { LastChildFill = false };
        var title = new TextBlock { Text = "Table Management", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, VerticalAlignment = VerticalAlignment.Center };
        
        var addBtn = new Button
        {
            Content = "➕ Add Table",
            Background = UIHelper.GreenBrush,
            Foreground = UIHelper.WhiteBrush,
            Padding = new Thickness(16, 10),
            CornerRadius = new CornerRadius(8),
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0)
        };
        DockPanel.SetDock(addBtn, Dock.Right);
        headerPanel.Children.Add(addBtn);
        headerPanel.Children.Add(title);
        topStack.Children.Add(headerPanel);

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        // Grid container
        var grid = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Left };
        var scroll = new ScrollViewer { Content = grid };
        mainDock.Children.Add(scroll);

        void RefreshTables()
        {
            grid.Children.Clear();
            var tables = TableRepository.GetAllTables();
            foreach (var t in tables)
            {
                // Card Border
                var card = new Border
                {
                    Width = 190,
                    Height = 170,
                    Background = UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(16),
                    Margin = new Thickness(0, 0, 16, 16),
                    BoxShadow = UIHelper.CardShadow
                };

                var cardStack = new StackPanel { Spacing = 10 };

                // Title row
                var titleText = new TextBlock
                {
                    Text = $"Table {t.TableNumber}",
                    FontSize = 18,
                    FontWeight = FontWeight.Bold,
                    Foreground = UIHelper.DarkBrush
                };

                // Capacity row
                var capText = new TextBlock
                {
                    Text = $"👥 Capacity: {t.Capacity} seats",
                    FontSize = 12,
                    Foreground = UIHelper.GrayBrush
                };

                // Status Badge
                var statusBadge = new Border
                {
                    Background = t.Status == "Free" ? UIHelper.GreenBrush : UIHelper.DangerBrush,
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 4),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Child = new TextBlock { Text = t.Status, FontSize = 10, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                };

                // Buttons row
                var actionRow = new UniformGrid { Columns = 2 };
                
                // Toggle status btn
                var toggleBtn = new Button
                {
                    Content = t.Status == "Free" ? "Occupy" : "Free Up",
                    Background = t.Status == "Free" ? UIHelper.DangerBrush : UIHelper.GreenBrush,
                    Foreground = UIHelper.WhiteBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(0, 6),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0, 0, 4, 0)
                };

                var deleteBtn = new Button
                {
                    Content = "🗑️",
                    Background = UIHelper.LightGrayBrush,
                    Foreground = UIHelper.DangerBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(0, 6),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(4, 0, 0, 0)
                };

                var capTable = t;
                toggleBtn.Click += (_, _) =>
                {
                    string newStatus = capTable.Status == "Free" ? "Occupied" : "Free";
                    TableRepository.UpdateTableStatus(capTable.Id, newStatus);
                    RefreshTables();
                };

                deleteBtn.Click += (_, _) =>
                {
                    TableRepository.DeleteTable(capTable.Id);
                    RefreshTables();
                };

                actionRow.Children.Add(toggleBtn);
                actionRow.Children.Add(deleteBtn);

                cardStack.Children.Add(titleText);
                cardStack.Children.Add(capText);
                cardStack.Children.Add(statusBadge);
                cardStack.Children.Add(actionRow);

                card.Child = cardStack;
                grid.Children.Add(card);
            }

            if (grid.Children.Count == 0)
            {
                grid.Children.Add(new TextBlock { Text = "No tables configured.", Foreground = UIHelper.GrayBrush, FontSize = 14 });
            }
        }

        RefreshTables();

        addBtn.Click += async (_, _) =>
        {
            var dialog = new AddTableDialog();
            await dialog.ShowDialog(this);
            RefreshTables();
        };

        return mainDock;
    }

    private Control BuildOrdersPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };

        // Header Section
        var headerPanel = new DockPanel { LastChildFill = false };
        var title = new TextBlock { Text = "Customer Orders Log", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, VerticalAlignment = VerticalAlignment.Center };
        
        var refreshBtn = new Button
        {
            Content = "🔄 Refresh Orders",
            Background = UIHelper.BlueBrush,
            Foreground = UIHelper.WhiteBrush,
            Padding = new Thickness(16, 10),
            CornerRadius = new CornerRadius(8),
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0)
        };
        DockPanel.SetDock(refreshBtn, Dock.Right);
        headerPanel.Children.Add(refreshBtn);
        headerPanel.Children.Add(title);
        topStack.Children.Add(headerPanel);

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        // List container
        var listStack = new StackPanel { Spacing = 10 };
        var scroll = new ScrollViewer { Content = listStack };
        mainDock.Children.Add(scroll);

        void RefreshOrders()
        {
            listStack.Children.Clear();
            var orders = OrderRepository.GetAllOrders();
            bool isEven = false;
            foreach (var o in orders)
            {
                var rowBorder = new Border
                {
                    Background = isEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 12),
                    BorderBrush = UIHelper.BorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                isEven = !isEven;

                var row = new DockPanel();

                var leftPanel = new StackPanel { Spacing = 4 };
                var orderTitle = new TextBlock { Text = $"Order #{o.Id}", FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
                string placedBy = o.WaiterName ?? $"User #{o.UserId}";
                string confirmedByText = string.IsNullOrEmpty(o.ConfirmedBy) ? "" : $"  •  Confirmed By: {o.ConfirmedBy}";
                var tableText = new TextBlock { Text = $"🪑 Table ID: {o.TableId}  •  Placed by: {placedBy}{confirmedByText}  •  Status: {o.Status}  •  {o.OrderTime:g}", FontSize = 12, Foreground = UIHelper.GrayBrush };
                leftPanel.Children.Add(orderTitle);
                leftPanel.Children.Add(tableText);
                // Right Button
                var viewBtn = new Button
                {
                    Content = "👁️ View Details",
                    Background = UIHelper.BlueBrush,
                    Foreground = UIHelper.WhiteBrush,
                    Padding = new Thickness(12, 6),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 12,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                DockPanel.SetDock(viewBtn, Dock.Right);

                var capOrder = o;
                viewBtn.Click += (_, _) =>
                {
                    new OrderItemsWindow(capOrder.Id).Show();
                };

                // Add right-docked elements FIRST to prevent clipping/overlapping!
                row.Children.Add(viewBtn);
                row.Children.Add(leftPanel);
                rowBorder.Child = row;
                listStack.Children.Add(rowBorder);
            }

            if (listStack.Children.Count == 0)
            {
                listStack.Children.Add(new TextBlock { Text = "No orders recorded yet.", Foreground = UIHelper.GrayBrush, FontSize = 14 });
            }
        }

        RefreshOrders();
        refreshBtn.Click += (_, _) => RefreshOrders();

        return mainDock;
    }

    private Control BuildBillsPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 14, Margin = new Thickness(0, 0, 0, 16) };

        var title = new TextBlock { Text = "Bills & Financial Log", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
        topStack.Children.Add(title);

        var bills = BillRepository.GetAllBills();
        decimal revenue = BillRepository.GetTotalRevenue();
        int paidCount = bills.Count(b => b.IsPaid);
        int unpaidCount = bills.Count(b => !b.IsPaid);

        // Stats Row at Top
        var statsRow = new UniformGrid { Columns = 3, Margin = new Thickness(0, 0, 0, 10) };

        void AddStatCard(string label, string value, IBrush color, string icon)
        {
            var box = new Border 
            { 
                Background = UIHelper.WhiteBrush, 
                CornerRadius = new CornerRadius(12), 
                Padding = new Thickness(20, 16), 
                BoxShadow = UIHelper.CardShadow,
                Margin = new Thickness(6)
            };
            
            var s = new DockPanel();
            
            var iconText = new TextBlock { Text = icon, FontSize = 28, VerticalAlignment = VerticalAlignment.Center };
            DockPanel.SetDock(iconText, Dock.Left);
            s.Children.Add(iconText);

            var textStack = new StackPanel { Margin = new Thickness(16, 0, 0, 0) };
            textStack.Children.Add(new TextBlock { Text = value, FontSize = 20, FontWeight = FontWeight.Bold, Foreground = color });
            textStack.Children.Add(new TextBlock { Text = label, FontSize = 12, Foreground = UIHelper.GrayBrush });
            s.Children.Add(textStack);

            box.Child = s;
            statsRow.Children.Add(box);
        }

        AddStatCard("Total Revenue", $"৳{revenue:F2}", UIHelper.GreenBrush, "💰");
        AddStatCard("Paid Invoices", paidCount.ToString(), UIHelper.BlueBrush, "✅");
        AddStatCard("Unpaid Invoices", unpaidCount.ToString(), UIHelper.DangerBrush, "⏳");

        topStack.Children.Add(statsRow);

        // Tax Settings Configuration Row
        var taxCard = new Border
        {
            Background = UIHelper.WhiteBrush,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 14),
            BoxShadow = UIHelper.CardShadow,
            Margin = new Thickness(0, 4, 0, 8)
        };
        var taxRow = new DockPanel();
        
        var taxLabel = new TextBlock
        {
            Text = "⚙️ Configure Global System Tax Rate (%):",
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        var updateTaxBtn = new Button
        {
            Content = "Update Rate",
            Background = UIHelper.AccentBrush,
            Foreground = UIHelper.WhiteBrush,
            Padding = new Thickness(14, 8),
            CornerRadius = new CornerRadius(6),
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0),
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        DockPanel.SetDock(updateTaxBtn, Dock.Right);
        
        var taxInput = new TextBox
        {
            Text = SettingsRepository.GetTaxRate().ToString("F1"),
            Width = 90,
            Padding = new Thickness(10, 8),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            Background = UIHelper.LightGrayBrush,
            Foreground = UIHelper.DangerBrush, // Beautiful red text color!
            FontWeight = FontWeight.Bold,      // Make it bold and readable!
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(6),
            Margin = new Thickness(12, 0, 12, 0)
        };
        DockPanel.SetDock(taxInput, Dock.Right);

        updateTaxBtn.Click += (_, _) =>
        {
            if (double.TryParse(taxInput.Text, out double newRate) && newRate >= 0 && newRate <= 100)
            {
                SettingsRepository.SetTaxRate(newRate);
                LoadPanel("💰 Bills"); // Refresh the Admin bills panel to reflect
            }
        };

        taxRow.Children.Add(updateTaxBtn);
        taxRow.Children.Add(taxInput);
        taxRow.Children.Add(taxLabel);
        taxCard.Child = taxRow;
        topStack.Children.Add(taxCard);

        // List title
        topStack.Children.Add(new TextBlock { Text = "Invoices List", FontSize = 16, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        // Bills List
        var listStack = new StackPanel { Spacing = 10 };
        var scroll = new ScrollViewer { Content = listStack }; // dynamic full height!
        mainDock.Children.Add(scroll);

        void RefreshBills()
        {
            listStack.Children.Clear();
            var billList = BillRepository.GetAllBills();
            bool isEven = false;
            foreach (var b in billList)
            {
                var rowBorder = new Border
                {
                    Background = isEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 12),
                    BorderBrush = UIHelper.BorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                isEven = !isEven;

                var row = new DockPanel();

                // Left side: Bill information
                var leftPanel = new StackPanel { Spacing = 4 };
                var billHeader = new TextBlock { Text = $"Invoice #{b.Id}  •  Order #{b.OrderId}", FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
                string completedByText = string.IsNullOrEmpty(b.CompletedBy) ? "" : $"  |  Staff: {b.CompletedBy}";
                var billDetails = new TextBlock { Text = $"Subtotal: ৳{b.Subtotal:F2}  |  Tax ({b.TaxRate:F1}%): ৳{b.Tax:F2}  |  Total: ৳{b.GrandTotal:F2}  |  {b.BillTime:g}{completedByText}", FontSize = 12, Foreground = UIHelper.GrayBrush };
                leftPanel.Children.Add(billHeader);
                leftPanel.Children.Add(billDetails);
                // Right side: Status and actions
                var actionStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
                DockPanel.SetDock(actionStack, Dock.Right);

                var statusText = new Border
                {
                    Background = b.IsPaid ? UIHelper.GreenBrush : UIHelper.DangerBrush,
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 4),
                    VerticalAlignment = VerticalAlignment.Center,
                    Child = new TextBlock { Text = b.IsPaid ? "Paid" : "Unpaid", FontSize = 10, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                };
                actionStack.Children.Add(statusText);

                var detailsBtn = new Button
                {
                    Content = "👁️ Details",
                    Background = UIHelper.BlueBrush,
                    Foreground = UIHelper.WhiteBrush,
                    Padding = new Thickness(10, 6),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0)
                };
                var capBill = b;
                detailsBtn.Click += (_, _) =>
                {
                    new OrderItemsWindow(capBill.OrderId, capBill).Show();
                };
                actionStack.Children.Add(detailsBtn);



                // Add right-docked elements FIRST to prevent clipping/overlapping!
                row.Children.Add(actionStack);
                row.Children.Add(leftPanel);
                rowBorder.Child = row;
                listStack.Children.Add(rowBorder);
            }

            if (listStack.Children.Count == 0)
            {
                listStack.Children.Add(new TextBlock { Text = "No bills generated yet.", Foreground = UIHelper.GrayBrush, FontSize = 14 });
            }
        }

        RefreshBills();

        return mainDock;
    }

    private Control BuildSearchPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };
        var title = new TextBlock { Text = "Global Search Engine", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
        topStack.Children.Add(title);

        var searchBox = new TextBox 
        { 
            Watermark = "🔍 Search menu items, staff username, or table statuses...", 
            Width = 450, 
            HorizontalAlignment = HorizontalAlignment.Left, 
            Padding = new Thickness(12, 10),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            Background = UIHelper.WhiteBrush,
            BorderBrush = UIHelper.BorderBrush,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8)
        };
        topStack.Children.Add(searchBox);

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        var resultsPanel = new StackPanel { Spacing = 14 };
        var scroll = new ScrollViewer { Content = resultsPanel };
        mainDock.Children.Add(scroll);

        searchBox.TextChanged += (_, _) =>
        {
            resultsPanel.Children.Clear();
            string q = searchBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(q)) return;

            var menuItems = MenuRepository.GetAllMenuItems().Where(m => m.Name.Contains(q, StringComparison.OrdinalIgnoreCase) || m.Category.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            var users = UserRepository.GetAllUsers().Where(u => u.Name.Contains(q, StringComparison.OrdinalIgnoreCase) || u.Role.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            var tables = TableRepository.GetAllTables().Where(t => t.Status.Contains(q, StringComparison.OrdinalIgnoreCase) || t.TableNumber.ToString() == q).ToList();

            if (menuItems.Any())
            {
                var menuHeader = new TextBlock { Text = "🍽️ Menu Items", FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush, Margin = new Thickness(0, 8, 0, 4) };
                resultsPanel.Children.Add(menuHeader);

                var itemsStack = new StackPanel { Spacing = 6 };
                foreach (var m in menuItems)
                {
                    var card = new Border { Background = UIHelper.WhiteBrush, CornerRadius = new CornerRadius(8), Padding = new Thickness(12), BoxShadow = UIHelper.CardShadow };
                    card.Child = new TextBlock { Text = $"{m.Name}  |  {m.Category}  |  ৳{m.Price:F2}", FontSize = 13, Foreground = UIHelper.DarkBrush };
                    itemsStack.Children.Add(card);
                }
                resultsPanel.Children.Add(itemsStack);
            }

            if (users.Any())
            {
                var usersHeader = new TextBlock { Text = "👥 Users & Staff", FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.BlueBrush, Margin = new Thickness(0, 12, 0, 4) };
                resultsPanel.Children.Add(usersHeader);

                var usersStack = new StackPanel { Spacing = 6 };
                foreach (var u in users)
                {
                    var card = new Border { Background = UIHelper.WhiteBrush, CornerRadius = new CornerRadius(8), Padding = new Thickness(12), BoxShadow = UIHelper.CardShadow };
                    card.Child = new TextBlock { Text = $"{u.Name}  |  Role: {u.Role}", FontSize = 13, Foreground = UIHelper.DarkBrush };
                    usersStack.Children.Add(card);
                }
                resultsPanel.Children.Add(usersStack);
            }

            if (tables.Any())
            {
                var tablesHeader = new TextBlock { Text = "🪑 Seating Tables", FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.GreenBrush, Margin = new Thickness(0, 12, 0, 4) };
                resultsPanel.Children.Add(tablesHeader);

                var tablesStack = new StackPanel { Spacing = 6 };
                foreach (var t in tables)
                {
                    var card = new Border { Background = UIHelper.WhiteBrush, CornerRadius = new CornerRadius(8), Padding = new Thickness(12), BoxShadow = UIHelper.CardShadow };
                    card.Child = new TextBlock { Text = $"Table {t.TableNumber}  |  Seats: {t.Capacity}  |  Status: {t.Status}", FontSize = 13, Foreground = UIHelper.DarkBrush };
                    tablesStack.Children.Add(card);
                }
                resultsPanel.Children.Add(tablesStack);
            }

            if (!menuItems.Any() && !users.Any() && !tables.Any())
            {
                resultsPanel.Children.Add(new TextBlock { Text = "No matches found.", Foreground = UIHelper.GrayBrush, FontSize = 13, Margin = new Thickness(0, 10, 0, 0) });
            }
        };

        return mainDock;
    }
}
