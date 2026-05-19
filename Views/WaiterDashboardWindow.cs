using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Views;

public class WaiterDashboardWindow : Window
{
    private readonly User _currentUser;
    private Panel _contentArea = new Panel();
    private Border? _activeBorder;

    public WaiterDashboardWindow(User user)
    {
        _currentUser = user;
        Title = "Waiter Dashboard — Restaurant Management System";
        Width = 1100;
        Height = 700;
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

        var profileStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 10
        };
        DockPanel.SetDock(profileStack, Dock.Right);

        var userInfo = new TextBlock
        {
            Text = $"{_currentUser.Name} (Waiter)",
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

        var avatar = new Border
        {
            Width = 36,
            Height = 36,
            CornerRadius = new CornerRadius(18),
            Background = UIHelper.BlueBrush, // blue for waiters
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
            Text = "WAITER PANEL",
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.GrayBrush,
            Margin = new Thickness(20, 0, 0, 16),
            LetterSpacing = 1.0
        };
        sideStack.Children.Add(sideTitle);

        string[] panels = { "🆕 New Order", "📋 My Orders", "🔍 Search Menu" };
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

        if (sideStack.Children[1] is Border firstBorder)
        {
            SetActiveBorder(firstBorder);
            LoadPanel("🆕 New Order");
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
            "🆕 New Order" => BuildNewOrderPanel(),
            "📋 My Orders" => BuildMyOrdersPanel(),
            "🔍 Search Menu" => BuildSearchPanel(),
            _ => new TextBlock { Text = "Not found" }
        };
        _contentArea.Children.Add(panel);
    }

    private Control BuildNewOrderPanel()
    {
        var mainGrid = new Grid();
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("3*"))); // Left: Menu
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("2*"))); // Right: Cart

        var stackLeft = new DockPanel { Margin = new Thickness(24) };

        var headerStack = new StackPanel { Spacing = 12, Margin = new Thickness(0, 0, 0, 16) };
        headerStack.Children.Add(new TextBlock { Text = "New Dining Order", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });

        // Waiter Name Info
        var waiterRow = new DockPanel { Margin = new Thickness(0, 4) };
        var waiterLabel = new TextBlock { Text = "Serving Waiter:", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, Width = 150, VerticalAlignment = VerticalAlignment.Center };
        var waiterNameText = new TextBlock { Text = _currentUser.Name, FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.BlueBrush, VerticalAlignment = VerticalAlignment.Center };
        DockPanel.SetDock(waiterLabel, Dock.Left);
        waiterRow.Children.Add(waiterLabel);
        waiterRow.Children.Add(waiterNameText);
        headerStack.Children.Add(waiterRow);

        // Table Selection Row
        var tableRow = new DockPanel { Margin = new Thickness(0, 4) };
        var tableLabel = new TextBlock { Text = "Select Dining Table:", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, Width = 150, VerticalAlignment = VerticalAlignment.Center };
        
        var freeTables = TableRepository.GetAllTables().Where(t => t.Status == "Free").ToList();
        var tableCombo = new ComboBox 
        { 
            Width = 240, 
            Height = 36, 
            PlaceholderText = "Choose a table...",
            Background = UIHelper.WhiteBrush,
            Foreground = UIHelper.DarkBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            CornerRadius = new CornerRadius(6)
        };
        foreach (var t in freeTables) tableCombo.Items.Add($"Table {t.TableNumber} (Seats: {t.Capacity})");

        var noTableMsg = new TextBlock { Text = "⚠️ No free tables available right now.", Foreground = UIHelper.DangerBrush, FontSize = 12, FontWeight = FontWeight.Bold, IsVisible = freeTables.Count == 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 0, 0) };

        DockPanel.SetDock(tableLabel, Dock.Left);
        DockPanel.SetDock(tableCombo, Dock.Left);
        tableRow.Children.Add(tableLabel);
        tableRow.Children.Add(tableCombo);
        tableRow.Children.Add(noTableMsg);
        headerStack.Children.Add(tableRow);

        // Customer Selection Row
        var customerRow = new DockPanel { Margin = new Thickness(0, 4) };
        var customerLabel = new TextBlock { Text = "Select Customer:", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, Width = 150, VerticalAlignment = VerticalAlignment.Center };
        
        var searchLabel = new TextBlock { Text = "🔍 Search ID:", FontSize = 13, Foreground = UIHelper.GrayBrush, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 8, 0) };
        var searchIdBox = new TextBox 
        { 
            Width = 70,
            Height = 36, 
            Padding = new Thickness(8, 6), 
            FontSize = 13, 
            VerticalAlignment = VerticalAlignment.Center, 
            VerticalContentAlignment = VerticalAlignment.Center, 
            CaretBrush = UIHelper.DarkBrush,
            CornerRadius = new CornerRadius(6) 
        };
        
        var customersList = UserRepository.GetAllUsers().Where(u => u.Role == "Customer").ToList();
        var customerCombo = new ComboBox 
        { 
            Width = 240, 
            Height = 36, 
            PlaceholderText = "Choose a customer...", 
            Background = UIHelper.WhiteBrush,
            Foreground = UIHelper.DarkBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            Margin = new Thickness(12, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            CornerRadius = new CornerRadius(6)
        };
        foreach (var c in customersList)
        {
            customerCombo.Items.Add(new CustomerComboItem(c));
        }
        if (customerCombo.Items.Count > 0) customerCombo.SelectedIndex = 0; // Default select first

        searchIdBox.KeyUp += (sender, args) =>
        {
            if (int.TryParse(searchIdBox.Text, out int searchId))
            {
                for (int i = 0; i < customerCombo.Items.Count; i++)
                {
                    var item = (CustomerComboItem)customerCombo.Items[i]!;
                    if (item.Customer.Id == searchId)
                    {
                        customerCombo.SelectedIndex = i;
                        break;
                    }
                }
            }
        };
        
        DockPanel.SetDock(customerLabel, Dock.Left);
        customerRow.Children.Add(customerLabel);
        
        // Add search box & label & combo side-by-side using Dock.Left
        DockPanel.SetDock(searchLabel, Dock.Left);
        DockPanel.SetDock(searchIdBox, Dock.Left);
        DockPanel.SetDock(customerCombo, Dock.Left);
        customerRow.Children.Add(searchLabel);
        customerRow.Children.Add(searchIdBox);
        customerRow.Children.Add(customerCombo);
        headerStack.Children.Add(customerRow);

        // Immediate Payment Row
        var paymentRow = new DockPanel { Margin = new Thickness(0, 4) };
        var paymentLabel = new TextBlock { Text = "Pre-paid Order:", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, Width = 150, VerticalAlignment = VerticalAlignment.Center };
        
        var immediatePayCheck = new CheckBox 
        { 
            IsChecked = true, 
            IsEnabled = false,
            VerticalAlignment = VerticalAlignment.Center 
        };
        var checkText = new TextBlock 
        { 
            Text = "Collect Payment Immediately (Mark as Paid)", 
            Foreground = UIHelper.GreenBrush, 
            FontSize = 12, 
            FontWeight = FontWeight.Bold, 
            VerticalAlignment = VerticalAlignment.Center 
        };
        var checkStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
        checkStack.Children.Add(immediatePayCheck);
        checkStack.Children.Add(checkText);
        
        DockPanel.SetDock(paymentLabel, Dock.Left);
        paymentRow.Children.Add(paymentLabel);
        paymentRow.Children.Add(checkStack);
        headerStack.Children.Add(paymentRow);

        headerStack.Children.Add(new TextBlock { Text = "Available Dishes & Drinks", FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, Margin = new Thickness(0, 12, 0, 0) });

        DockPanel.SetDock(headerStack, Dock.Top);
        stackLeft.Children.Add(headerStack);

        var menuItemsStack = new StackPanel { Spacing = 8 };
        var menuScroll = new ScrollViewer { Content = menuItemsStack }; // NO FIXED HEIGHT! Expands to full screen height
        stackLeft.Children.Add(menuScroll);

        // Cart items tracked inside memory
        var cart = new List<(RestaurantManagementSystem.Models.MenuItem item, int qty)>();
        
        // UI definitions from right panel
        var cartItemsStack = new StackPanel { Spacing = 8 };
        var totalText = new TextBlock { Text = "Total: ৳0.00", FontSize = 18, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush };
        var cartError = new TextBlock { Foreground = UIHelper.DangerBrush, FontSize = 12, IsVisible = false, TextWrapping = TextWrapping.Wrap };

        void RefreshCart()
        {
            cartItemsStack.Children.Clear();
            decimal total = 0;
            bool isEven = false;
            foreach (var cartEntry in cart)
            {
                var entryBorder = new Border
                {
                    Background = isEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                    Padding = new Thickness(12, 8),
                    CornerRadius = new CornerRadius(6),
                    BorderBrush = UIHelper.BorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                isEven = !isEven;

                var entryRow = new DockPanel();
                var labelStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                labelStack.Children.Add(new TextBlock { Text = cartEntry.item.Name, FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });
                labelStack.Children.Add(new TextBlock { Text = $"x{cartEntry.qty} = ৳{cartEntry.item.Price * cartEntry.qty:F2}", FontSize = 12, Foreground = UIHelper.GrayBrush });
                entryRow.Children.Add(labelStack);

                var removeBtn = new Button
                {
                    Content = "❌",
                    Background = Brushes.Transparent,
                    Foreground = UIHelper.DangerBrush,
                    Padding = new Thickness(4),
                    FontSize = 11,
                    BorderThickness = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                DockPanel.SetDock(removeBtn, Dock.Right);

                var capEntry = cartEntry;
                removeBtn.Click += (_, _) =>
                {
                    cart.Remove(capEntry);
                    RefreshCart();
                };

                entryRow.Children.Add(removeBtn);
                entryBorder.Child = entryRow;
                cartItemsStack.Children.Add(entryBorder);
                total += cartEntry.item.Price * cartEntry.qty;
            }

            totalText.Text = $"Total: ৳{total:F2}";
            if (cart.Count == 0)
            {
                cartItemsStack.Children.Add(new TextBlock { Text = "Cart is empty.", Foreground = UIHelper.GrayBrush, FontSize = 13 });
            }
        }

        // Render Dishes inside left panel
        var menuItems = MenuRepository.GetAllMenuItems().Where(m => m.IsAvailable).ToList();
        bool isRowEven = false;
        foreach (var m in menuItems)
        {
            var itemBorder = new Border
            {
                Background = isRowEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 10),
                BorderBrush = UIHelper.BorderBrush,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
            isRowEven = !isRowEven;

            var itemRow = new DockPanel();

            // Image Thumbnail inside list row
            var imgThumbnail = new Border
            {
                Width = 52,
                Height = 52,
                CornerRadius = new CornerRadius(8),
                ClipToBounds = true,
                Background = UIHelper.LightGrayBrush,
                Margin = new Thickness(0, 0, 12, 0)
            };
            DockPanel.SetDock(imgThumbnail, Dock.Left);

            if (!string.IsNullOrEmpty(m.ImagePath) && System.IO.File.Exists(m.ImagePath))
            {
                try
                {
                    var bitmap = new Avalonia.Media.Imaging.Bitmap(m.ImagePath);
                    imgThumbnail.Child = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill
                    };
                }
                catch
                {
                    imgThumbnail.Child = new TextBlock
                    {
                        Text = UIHelper.GetCategoryEmoji(m.Category),
                        FontSize = 24,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
            }
            else
            {
                imgThumbnail.Child = new TextBlock
                {
                    Text = UIHelper.GetCategoryEmoji(m.Category),
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            itemRow.Children.Add(imgThumbnail);
            
            // Middle info (Name, category, price)
            var infoStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Spacing = 4 };
            infoStack.Children.Add(new TextBlock { Text = m.Name, FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });
            
            var badgeRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
            badgeRow.Children.Add(new Border
            {
                Background = UIHelper.GetCategoryBrush(m.Category),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 2),
                Child = new TextBlock { Text = m.Category, FontSize = 9, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
            });
            badgeRow.Children.Add(new TextBlock { Text = $"৳{m.Price:F2}", FontSize = 12, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush });
            infoStack.Children.Add(badgeRow);

            // Right side: quantity input and add button
            var actionsPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };
            DockPanel.SetDock(actionsPanel, Dock.Right);

            var qtyGroup = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 0, VerticalAlignment = VerticalAlignment.Center };

            var minusBtn = new Button
            {
                Content = "➖",
                Width = 30,
                Height = 32,
                Padding = new Thickness(0),
                Background = UIHelper.LightGrayBrush,
                Foreground = UIHelper.DarkBrush,
                BorderThickness = new Thickness(1, 1, 0, 1),
                BorderBrush = UIHelper.BorderBrush,
                CornerRadius = new CornerRadius(6, 0, 0, 6),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 9
            };

            var qtyBox = new TextBox
            {
                Width = 40,
                Height = 32,
                MinHeight = 0,
                Text = "1",
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CaretBrush = UIHelper.DarkBrush,
                Padding = new Thickness(0),
                Background = UIHelper.WhiteBrush,
                Foreground = UIHelper.DarkBrush,
                BorderThickness = new Thickness(1),
                BorderBrush = UIHelper.BorderBrush,
                CornerRadius = new CornerRadius(0)
            };

            var plusBtn = new Button
            {
                Content = "➕",
                Width = 30,
                Height = 32,
                Padding = new Thickness(0),
                Background = UIHelper.LightGrayBrush,
                Foreground = UIHelper.DarkBrush,
                BorderThickness = new Thickness(0, 1, 1, 1),
                BorderBrush = UIHelper.BorderBrush,
                CornerRadius = new CornerRadius(0, 6, 6, 0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 9
            };

            minusBtn.Click += (_, _) =>
            {
                if (int.TryParse(qtyBox.Text, out int qty) && qty > 1)
                {
                    qtyBox.Text = (qty - 1).ToString();
                }
            };

            plusBtn.Click += (_, _) =>
            {
                if (int.TryParse(qtyBox.Text, out int qty))
                {
                    qtyBox.Text = (qty + 1).ToString();
                }
                else
                {
                    qtyBox.Text = "1";
                }
            };

            qtyGroup.Children.Add(minusBtn);
            qtyGroup.Children.Add(qtyBox);
            qtyGroup.Children.Add(plusBtn);
            
            var addBtn = new Button
            {
                Content = "➕ Add",
                Background = UIHelper.BlueBrush,
                Foreground = UIHelper.WhiteBrush,
                Padding = new Thickness(12, 6),
                CornerRadius = new CornerRadius(6),
                FontSize = 12,
                FontWeight = FontWeight.Bold,
                BorderThickness = new Thickness(0)
            };

            var capItem = m;
            addBtn.Click += (_, _) =>
            {
                if (!int.TryParse(qtyBox.Text, out int qty) || qty <= 0) return;
                var existingIndex = cart.FindIndex(c => c.item.Id == capItem.Id);
                if (existingIndex >= 0)
                {
                    var old = cart[existingIndex];
                    cart[existingIndex] = (old.item, old.qty + qty);
                }
                else
                {
                    cart.Add((capItem, qty));
                }
                RefreshCart();
                qtyBox.Text = "1";
            };

            actionsPanel.Children.Add(qtyGroup);
            actionsPanel.Children.Add(addBtn);
            
            // Add right-docked elements FIRST to prevent clipping/overlapping!
            itemRow.Children.Add(actionsPanel);
            itemRow.Children.Add(infoStack);

            itemBorder.Child = itemRow;
            menuItemsStack.Children.Add(itemBorder);
        }

        // 2. Right Side: Cart panel
        var borderRight = new Border
        {
            Background = UIHelper.WhiteBrush,
            BoxShadow = BoxShadows.Parse("-4 0 10 0 #0A000000"),
            Padding = new Thickness(20)
        };
        Grid.SetColumn(borderRight, 1);

        var stackRight = new DockPanel();

        var cartHeader = new TextBlock { Text = "Active Cart", FontSize = 18, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, Margin = new Thickness(0, 0, 0, 16) };
        DockPanel.SetDock(cartHeader, Dock.Top);
        stackRight.Children.Add(cartHeader);

        var placeBtn = new Button
        {
            Content = "✅ Complete & Place Order",
            Background = UIHelper.GreenBrush,
            Foreground = UIHelper.WhiteBrush,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(0, 12),
            CornerRadius = new CornerRadius(8),
            FontWeight = FontWeight.Bold,
            FontSize = 14,
            BorderThickness = new Thickness(0)
        };
        placeBtn.Click += (_, _) =>
        {
            cartError.IsVisible = false;
            if (cart.Count == 0) { cartError.Text = "Please add items to your cart first."; cartError.IsVisible = true; return; }
            if (tableCombo.SelectedIndex < 0) { cartError.Text = "Please select a table."; cartError.IsVisible = true; return; }
            if (customerCombo.SelectedItem == null) { cartError.Text = "Please select a customer."; cartError.IsVisible = true; return; }
            
            var selectedTable = freeTables[tableCombo.SelectedIndex];
            var selectedCustomerItem = (CustomerComboItem)customerCombo.SelectedItem;
            var targetCustomer = selectedCustomerItem.Customer;
            
            // Create the order belonging directly to the chosen customer!
            int orderId = OrderRepository.CreateOrder(selectedTable.Id, targetCustomer.Id);
            OrderRepository.SetOrderConfirmedBy(orderId, _currentUser.Name);
            foreach (var cartEntry in cart)
            {
                OrderRepository.AddOrderItem(orderId, cartEntry.item.Id, cartEntry.item.Name, cartEntry.qty, cartEntry.item.Price);
            }
            TableRepository.UpdateTableOccupancyStatus(selectedTable.Id);
            
            // If pre-paid immediate payment is selected, automatically generate and mark the bill as paid!
            string paymentMsg = "Bill generated (Unpaid).";
            if (immediatePayCheck.IsChecked == true)
            {
                var bill = BillRepository.GenerateBill(orderId);
                BillRepository.MarkAsPaid(bill.Id, _currentUser.Name);
                paymentMsg = "Bill generated & marked as PAID by " + _currentUser.Name + "! 💵";
            }
            
            cart.Clear();
            RefreshCart();
            
            cartError.Foreground = UIHelper.GreenBrush;
            cartError.Text = $"Order #{orderId} placed for {targetCustomer.Name}! {paymentMsg}";
            cartError.IsVisible = true;
            
            // Reload panel to refresh table dropdown
            LoadPanel("🆕 New Order");
        };

        var bottomCartStack = new StackPanel { Spacing = 12, Margin = new Thickness(0, 16, 0, 0) };
        bottomCartStack.Children.Add(new Border { Height = 1, Background = UIHelper.BorderBrush });
        bottomCartStack.Children.Add(totalText);
        bottomCartStack.Children.Add(cartError);
        bottomCartStack.Children.Add(placeBtn);
        DockPanel.SetDock(bottomCartStack, Dock.Bottom);
        stackRight.Children.Add(bottomCartStack);

        var cartScroll = new ScrollViewer { Content = cartItemsStack }; // NO FIXED HEIGHT! Expands dynamically in the middle!
        stackRight.Children.Add(cartScroll);

        borderRight.Child = stackRight;

        RefreshCart(); // trigger empty cart text

        mainGrid.Children.Add(stackLeft);
        mainGrid.Children.Add(borderRight);

        return mainGrid;
    }

    private Control BuildMyOrdersPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };
        topStack.Children.Add(new TextBlock { Text = "🍽️ Restaurant Order Dispatch Center", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        var listStack = new StackPanel { Spacing = 12 };
        var scroll = new ScrollViewer { Content = listStack }; // dynamic full height!
        mainDock.Children.Add(scroll);

        void RefreshOrders()
        {
            listStack.Children.Clear();
            var orders = OrderRepository.GetAllOrders().Where(o => o.Status != "Completed").ToList();
            var tables = TableRepository.GetAllTables();
            bool isEven = false;
            foreach (var o in orders)
            {
                var rowBorder = new Border
                {
                    Background = isEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(16, 12),
                    BorderBrush = UIHelper.BorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BoxShadow = UIHelper.CardShadow,
                    Margin = new Thickness(0, 0, 0, 8)
                };
                isEven = !isEven;

                var row = new DockPanel();

                // Left Info
                var leftPanel = new StackPanel { Spacing = 6 };
                var headerStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
                var orderTitle = new TextBlock { Text = $"Order #{o.Id}", FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
                
                // Status Badge
                IBrush badgeBg = UIHelper.GrayBrush;
                string statusLabel = o.Status;
                if (o.Status == "Pending") { badgeBg = new SolidColorBrush(Color.Parse("#FF9F43")); statusLabel = "Pending Approval"; }
                else if (o.Status == "Confirmed") { badgeBg = UIHelper.BlueBrush; statusLabel = "Confirmed / Preparing"; }
                else if (o.Status == "Canceled") { badgeBg = UIHelper.DangerBrush; statusLabel = "Canceled"; }

                var statusBadge = new Border
                {
                    Background = badgeBg,
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6, 2),
                    Child = new TextBlock { Text = statusLabel, FontSize = 9, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                };

                headerStack.Children.Add(orderTitle);
                headerStack.Children.Add(statusBadge);
                leftPanel.Children.Add(headerStack);

                var tableObj = tables.FirstOrDefault(t => t.Id == o.TableId);
                string placedBy = string.IsNullOrEmpty(o.WaiterName) ? $"User #{o.UserId}" : o.WaiterName;
                string confirmedByText = string.IsNullOrEmpty(o.ConfirmedBy) ? "" : $"  •  Confirmed By: {o.ConfirmedBy}";
                var tableText = new TextBlock { Text = $"🪑 Table: {tableObj?.TableNumber ?? o.TableId}  •  Placed By: {placedBy}{confirmedByText}  •  Placed: {o.OrderTime:g}", FontSize = 12, Foreground = UIHelper.GrayBrush };
                leftPanel.Children.Add(tableText);

                if (o.Status == "Canceled" && !string.IsNullOrEmpty(o.CancelReason))
                {
                    var reasonText = new TextBlock { Text = $"Reason: {o.CancelReason}", FontSize = 11, FontStyle = FontStyle.Italic, Foreground = UIHelper.DangerBrush };
                    leftPanel.Children.Add(reasonText);
                }

                // Right Button actions
                var actionsPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
                DockPanel.SetDock(actionsPanel, Dock.Right);

                var viewItemsBtn = new Button
                {
                    Content = "👁️ Items",
                    Background = UIHelper.BlueBrush,
                    Foreground = UIHelper.WhiteBrush,
                    Padding = new Thickness(10, 6),
                    CornerRadius = new CornerRadius(6),
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    BorderThickness = new Thickness(0)
                };

                var capOrder = o;
                viewItemsBtn.Click += (_, _) =>
                {
                    new OrderItemsWindow(capOrder.Id).Show();
                };
                actionsPanel.Children.Add(viewItemsBtn);

                if (o.Status == "Pending")
                {
                    var confirmBtn = new Button
                    {
                        Content = "✅ Confirm",
                        Background = UIHelper.GreenBrush,
                        Foreground = UIHelper.WhiteBrush,
                        Padding = new Thickness(10, 6),
                        CornerRadius = new CornerRadius(6),
                        FontSize = 11,
                        FontWeight = FontWeight.Bold,
                        BorderThickness = new Thickness(0)
                    };
                    confirmBtn.Click += (_, _) =>
                    {
                        OrderRepository.UpdateOrderStatus(capOrder.Id, "Confirmed");
                        OrderRepository.SetOrderConfirmedBy(capOrder.Id, _currentUser.Name);
                        RefreshOrders();
                    };

                    var cancelBtn = new Button
                    {
                        Content = "❌ Cancel",
                        Background = UIHelper.DangerBrush,
                        Foreground = UIHelper.WhiteBrush,
                        Padding = new Thickness(10, 6),
                        CornerRadius = new CornerRadius(6),
                        FontSize = 11,
                        FontWeight = FontWeight.Bold,
                        BorderThickness = new Thickness(0)
                    };
                    cancelBtn.Click += (_, _) =>
                    {
                        new CancelReasonDialog(reason =>
                        {
                            OrderRepository.CancelOrder(capOrder.Id, reason);
                            TableRepository.UpdateTableOccupancyStatus(capOrder.TableId);
                            RefreshOrders();
                        }).Show();
                    };

                    actionsPanel.Children.Add(confirmBtn);
                    actionsPanel.Children.Add(cancelBtn);
                }
                else if (o.Status == "Confirmed")
                {
                    var bill = BillRepository.GetBillByOrderId(o.Id);
                    if (bill == null)
                    {
                        var billBtn = new Button
                        {
                            Content = "💰 Generate Bill",
                            Background = UIHelper.AccentBrush,
                            Foreground = UIHelper.WhiteBrush,
                            Padding = new Thickness(10, 6),
                            CornerRadius = new CornerRadius(6),
                            FontSize = 11,
                            FontWeight = FontWeight.Bold,
                            BorderThickness = new Thickness(0)
                        };
                        billBtn.Click += (_, _) =>
                        {
                            var newBill = BillRepository.GenerateBill(capOrder.Id);
                            new OrderItemsWindow(capOrder.Id, newBill).Show();
                            RefreshOrders();
                        };
                        actionsPanel.Children.Add(billBtn);

                        var cancelBtn = new Button
                        {
                            Content = "❌ Cancel Order",
                            Background = UIHelper.DangerBrush,
                            Foreground = UIHelper.WhiteBrush,
                            Padding = new Thickness(10, 6),
                            CornerRadius = new CornerRadius(6),
                            FontSize = 11,
                            FontWeight = FontWeight.Bold,
                            BorderThickness = new Thickness(0)
                        };
                        cancelBtn.Click += (_, _) =>
                        {
                            new CancelReasonDialog(reason =>
                            {
                                OrderRepository.CancelOrder(capOrder.Id, reason);
                                TableRepository.UpdateTableOccupancyStatus(capOrder.TableId);
                                RefreshOrders();
                            }).Show();
                        };
                        actionsPanel.Children.Add(cancelBtn);
                    }
                    else
                    {
                        if (bill.IsPaid)
                        {
                            var billPreparedBadge = new Border
                            {
                                Background = UIHelper.SidebarAccentBrush,
                                BorderBrush = UIHelper.GreenBrush,
                                BorderThickness = new Thickness(1),
                                CornerRadius = new CornerRadius(6),
                                Padding = new Thickness(10, 6),
                                Child = new TextBlock { Text = "🧾 Paid Online", FontSize = 11, FontWeight = FontWeight.Bold, Foreground = UIHelper.GreenBrush }
                            };
                            
                            var completeBtn = new Button
                            {
                                Content = "🏁 Complete Order",
                                Background = UIHelper.BlueBrush,
                                Foreground = UIHelper.WhiteBrush,
                                Padding = new Thickness(10, 6),
                                CornerRadius = new CornerRadius(6),
                                FontSize = 11,
                                FontWeight = FontWeight.Bold,
                                BorderThickness = new Thickness(0)
                            };
                            completeBtn.Click += (_, _) =>
                            {
                                OrderRepository.UpdateOrderStatus(capOrder.Id, "Completed");
                                TableRepository.UpdateTableOccupancyStatus(capOrder.TableId);
                                var bill = BillRepository.GetBillByOrderId(capOrder.Id);
                                if (bill != null)
                                {
                                    BillRepository.SetCompletedBy(bill.Id, _currentUser.Name);
                                }
                                RefreshOrders();
                            };
                            
                            actionsPanel.Children.Add(billPreparedBadge);
                            actionsPanel.Children.Add(completeBtn);
                        }
                        else
                        {
                            var billPreparedBadge = new Border
                            {
                                Background = UIHelper.SidebarAccentBrush,
                                BorderBrush = UIHelper.AccentBrush,
                                BorderThickness = new Thickness(1),
                                CornerRadius = new CornerRadius(6),
                                Padding = new Thickness(10, 6),
                                Child = new TextBlock { Text = "🧾 Waiting for Payment", FontSize = 11, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush }
                            };
                            
                            var cancelBtn = new Button
                            {
                                Content = "❌ Cancel Order",
                                Background = UIHelper.DangerBrush,
                                Foreground = UIHelper.WhiteBrush,
                                Padding = new Thickness(10, 6),
                                CornerRadius = new CornerRadius(6),
                                FontSize = 11,
                                FontWeight = FontWeight.Bold,
                                BorderThickness = new Thickness(0)
                            };
                            cancelBtn.Click += (_, _) =>
                            {
                                new CancelReasonDialog(reason =>
                                {
                                    OrderRepository.CancelOrder(capOrder.Id, reason);
                                    TableRepository.UpdateTableOccupancyStatus(capOrder.TableId);
                                    RefreshOrders();
                                }).Show();
                            };
                            
                            actionsPanel.Children.Add(billPreparedBadge);
                            actionsPanel.Children.Add(cancelBtn);
                        }
                    }
                }

                // Add right-docked elements FIRST to prevent clipping/overlapping!
                row.Children.Add(actionsPanel);
                row.Children.Add(leftPanel);
                rowBorder.Child = row;
                listStack.Children.Add(rowBorder);
            }

            if (listStack.Children.Count == 0)
            {
                listStack.Children.Add(new TextBlock { Text = "No active pending or preparing orders currently in the system.", Foreground = UIHelper.GrayBrush, FontSize = 14 });
            }
        }

        RefreshOrders();
        return mainDock;
    }

    private Control BuildSearchPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };
        topStack.Children.Add(new TextBlock { Text = "Search Restaurant Menu", FontSize = 22, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });

        var searchBox = new TextBox 
        { 
            Watermark = "🔍 Type category or item name...", 
            Width = 400, 
            HorizontalAlignment = HorizontalAlignment.Left, 
            Padding = new Thickness(12, 10),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            Background = UIHelper.WhiteBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(8)
        };
        topStack.Children.Add(searchBox);

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        var resultsPanel = new StackPanel { Spacing = 8 };
        var scroll = new ScrollViewer { Content = resultsPanel }; // dynamic full height!
        mainDock.Children.Add(scroll);

        searchBox.TextChanged += (_, _) =>
        {
            resultsPanel.Children.Clear();
            string q = searchBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(q)) return;
            
            var results = MenuRepository.GetAllMenuItems().Where(m => m.Name.Contains(q, StringComparison.OrdinalIgnoreCase) || m.Category.Contains(q, StringComparison.OrdinalIgnoreCase));
            bool isEven = false;
            foreach (var m in results)
            {
                var card = new Border
                {
                    Background = isEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 12),
                    BorderBrush = UIHelper.BorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                isEven = !isEven;

                var row = new DockPanel();
                var textDetails = new StackPanel { Spacing = 4 };
                textDetails.Children.Add(new TextBlock { Text = m.Name, FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });
                
                var badges = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
                badges.Children.Add(new Border
                {
                    Background = UIHelper.GetCategoryBrush(m.Category),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6, 2),
                    Child = new TextBlock { Text = m.Category, FontSize = 9, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                });
                badges.Children.Add(new TextBlock { Text = $"৳{m.Price:F2}", FontSize = 12, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush });
                
                textDetails.Children.Add(badges);
                row.Children.Add(textDetails);

                card.Child = row;
                resultsPanel.Children.Add(card);
            }
            
            if (!results.Any())
            {
                resultsPanel.Children.Add(new TextBlock { Text = "No results found matching search criteria.", Foreground = UIHelper.GrayBrush, FontSize = 13 });
            }
        };

        return mainDock;
    }
}

public class CancelReasonDialog : Window
{
    public CancelReasonDialog(Action<string> onCompleted)
    {
        Title = "Order Cancellation Reason";
        Width = 400; Height = 180;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = UIHelper.LightBackgroundBrush;
        
        var stack = new StackPanel { Margin = new Thickness(20), Spacing = 12 };
        var label = new TextBlock { Text = "Provide Cancellation Reason to Customer:", FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
        var text = new TextBox 
        { 
            Watermark = "e.g., Steak is sold out today", 
            Height = 32, 
            MinHeight = 0,
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            Background = UIHelper.WhiteBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(10, 6)
        };
        
        var btnRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, HorizontalAlignment = HorizontalAlignment.Right };
        var cancelBtn = new Button { Content = "Dismiss", Padding = new Thickness(12, 6) };
        var confirmBtn = new Button { Content = "Cancel Order", Background = UIHelper.DangerBrush, Foreground = UIHelper.WhiteBrush, Padding = new Thickness(12, 6) };
        
        cancelBtn.Click += (_, _) => Close();
        confirmBtn.Click += (_, _) => 
        { 
            string reason = text.Text?.Trim() ?? "Canceled by staff request.";
            onCompleted(reason);
            Close();
        };
        
        btnRow.Children.Add(cancelBtn);
        btnRow.Children.Add(confirmBtn);
        
        stack.Children.Add(label);
        stack.Children.Add(text);
        stack.Children.Add(btnRow);
        
        Content = stack;
    }
}

public class CustomerComboItem
{
    public RestaurantManagementSystem.Models.User Customer { get; }
    public string DisplayText => $"{Customer.Name} (ID: {Customer.Id})";

    public CustomerComboItem(RestaurantManagementSystem.Models.User customer)
    {
        Customer = customer;
    }

    public override string ToString() => DisplayText;
}
