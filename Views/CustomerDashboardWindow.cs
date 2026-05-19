using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Data;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Views;

public class CustomerDashboardWindow : Window
{
    private readonly User _currentUser;
    private Panel _contentArea = new Panel();
    private Border? _activeBorder;

    private readonly List<(RestaurantManagementSystem.Models.MenuItem item, int qty)> _cart = new();
    private Action? _refreshCartCallback;

    private void AddToCart(RestaurantManagementSystem.Models.MenuItem item, int qty)
    {
        var existingIndex = _cart.FindIndex(c => c.item.Id == item.Id);
        if (existingIndex >= 0)
        {
            var old = _cart[existingIndex];
            _cart[existingIndex] = (old.item, old.qty + qty);
        }
        else
        {
            _cart.Add((item, qty));
        }
        _refreshCartCallback?.Invoke();
    }

    public CustomerDashboardWindow(User user)
    {
        _currentUser = user;
        Title = "Customer Dashboard — Restaurant Management System";
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
            Text = $"{_currentUser.Name} (Customer ID: {_currentUser.Id})",
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
            Background = UIHelper.GreenBrush, // green for customers
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
            Text = "CUSTOMER PANEL",
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.GrayBrush,
            Margin = new Thickness(20, 0, 0, 16),
            LetterSpacing = 1.0
        };
        sideStack.Children.Add(sideTitle);

        string[] panels = { "🍽️ Browse Menu", "🛒 Place Order", "📦 My Orders & Bills", "🔍 Search Menu" };
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

        // Logout Button
        var logoutBorder = new Border
        {
            BorderThickness = new Thickness(4, 0, 0, 0),
            BorderBrush = Brushes.Transparent,
            Background = Brushes.Transparent,
            Height = 44,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };
        DockPanel.SetDock(logoutBorder, Dock.Bottom);

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

        var sidebarDock = new DockPanel();
        sidebarDock.Children.Add(logoutBorder);
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
            LoadPanel("🍽️ Browse Menu");
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
            "🍽️ Browse Menu" => BuildBrowseMenuPanel(),
            "🛒 Place Order" => BuildOrderPanel(),
            "📦 My Orders & Bills" => BuildMyOrdersAndBillsPanel(),
            "🔍 Search Menu" => BuildSearchPanel(),
            _ => new TextBlock { Text = "Not found" }
        };
        _contentArea.Children.Add(panel);
    }

    private Control BuildBrowseMenuPanel()
    {
        var root = new DockPanel { Margin = new Thickness(24) };

        var headerText = new TextBlock
        {
            Text = "Browse Dishes & Drinks",
            FontSize = 22,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            Margin = new Thickness(0, 0, 0, 16)
        };
        DockPanel.SetDock(headerText, Dock.Top);
        root.Children.Add(headerText);

        // Category Filter Row
        var filterRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Margin = new Thickness(0, 0, 0, 20) };
        filterRow.Children.Add(new TextBlock { Text = "Filter by Category:", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, VerticalAlignment = VerticalAlignment.Center });

        var categories = new[] { "All", "Fast Food", "Main Course", "Appetizer", "Sides", "Drinks", "Dessert" };
        var catCombo = new ComboBox
        {
            Width = 200,
            Padding = new Thickness(10, 6),
            Background = UIHelper.WhiteBrush,
            Foreground = UIHelper.DarkBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(6)
        };
        foreach (var c in categories) catCombo.Items.Add(c);
        catCombo.SelectedIndex = 0;
        filterRow.Children.Add(catCombo);

        DockPanel.SetDock(filterRow, Dock.Top);
        root.Children.Add(filterRow);

        // Visual Grid of Cards
        var grid = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Left };
        var scroll = new ScrollViewer { Content = grid };
        root.Children.Add(scroll);

        void RefreshMenu()
        {
            grid.Children.Clear();
            var items = MenuRepository.GetAllMenuItems().Where(m => m.IsAvailable);
            if (catCombo.SelectedItem is string cat && cat != "All")
                items = items.Where(m => m.Category == cat);

            foreach (var item in items)
            {
                var card = new Border
                {
                    Width = 240,
                    Height = 260,
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

                var nameText = new TextBlock
                {
                    Text = item.Name,
                    FontSize = 14,
                    FontWeight = FontWeight.Bold,
                    Foreground = UIHelper.DarkBrush,
                    TextWrapping = TextWrapping.Wrap,
                    Height = 36
                };

                var badgeRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
                badgeRow.Children.Add(new Border
                {
                    Background = UIHelper.GetCategoryBrush(item.Category),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 4),
                    Child = new TextBlock { Text = item.Category, FontSize = 10, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                });

                var bottomRow = new DockPanel { LastChildFill = false };

                var priceText = new TextBlock
                {
                    Text = $"৳{item.Price:F2}",
                    FontSize = 15,
                    FontWeight = FontWeight.Bold,
                    Foreground = UIHelper.AccentBrush,
                    VerticalAlignment = VerticalAlignment.Center
                };
                DockPanel.SetDock(priceText, Dock.Left);
                bottomRow.Children.Add(priceText);

                var addBtn = new Button
                {
                    Content = "➕ Add",
                    Background = UIHelper.GreenBrush,
                    Foreground = UIHelper.WhiteBrush,
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                    Padding = new Thickness(10, 4),
                    CornerRadius = new CornerRadius(6),
                    BorderThickness = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                DockPanel.SetDock(addBtn, Dock.Right);

                var capItem = item;
                addBtn.Click += (_, _) =>
                {
                    AddToCart(capItem, 1);

                    addBtn.Content = "Added! ✓";
                    addBtn.Background = UIHelper.DarkBrush;

                    var timer = new Avalonia.Threading.DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(1.5)
                    };
                    timer.Tick += (s, ev) =>
                    {
                        addBtn.Content = "➕ Add";
                        addBtn.Background = UIHelper.GreenBrush;
                        timer.Stop();
                    };
                    timer.Start();
                };
                bottomRow.Children.Add(addBtn);

                cardStack.Children.Add(imageContainer);
                cardStack.Children.Add(nameText);
                cardStack.Children.Add(badgeRow);
                cardStack.Children.Add(bottomRow);
                
                card.Child = cardStack;
                grid.Children.Add(card);
            }

            if (grid.Children.Count == 0)
            {
                grid.Children.Add(new TextBlock { Text = "No items available in this category.", Foreground = UIHelper.GrayBrush, FontSize = 13 });
            }
        }

        RefreshMenu();
        catCombo.SelectionChanged += (_, _) => RefreshMenu();

        return root;
    }

    private Control BuildOrderPanel()
    {
        var mainGrid = new Grid();
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("3*"))); // Left: Menu
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("2*"))); // Right: Cart

        var leftDock = new DockPanel { Margin = new Thickness(24) };
        
        var headerLeft = new TextBlock
        {
            Text = "Tableside Self-Service Order",
            FontSize = 22,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            Margin = new Thickness(0, 0, 0, 16)
        };
        DockPanel.SetDock(headerLeft, Dock.Top);
        leftDock.Children.Add(headerLeft);

        // Table selector drop-down
        var tableRow = new DockPanel { Margin = new Thickness(0, 0, 0, 16) };
        var tableLabel = new TextBlock { Text = "Your Table Number:", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 12, 0) };
        
        var tableCombo = new ComboBox
        {
            Width = 200,
            Padding = new Thickness(10, 6),
            Background = UIHelper.WhiteBrush,
            Foreground = UIHelper.DarkBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(6)
        };

        var allTables = TableRepository.GetAllTables();
        foreach (var t in allTables)
        {
            tableCombo.Items.Add(new TableComboItem(t));
        }

        var tableItemStyle = new Avalonia.Styling.Style(x => x.OfType<ComboBoxItem>());
        tableItemStyle.Setters.Add(new Setter(ComboBoxItem.IsEnabledProperty, new Avalonia.Data.Binding("IsFree")));
        tableCombo.Styles.Add(tableItemStyle);

        var tableError = new TextBlock { Foreground = UIHelper.DangerBrush, FontSize = 12, FontWeight = FontWeight.Bold, IsVisible = false, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0) };

        DockPanel.SetDock(tableLabel, Dock.Left);
        tableRow.Children.Add(tableLabel);
        tableRow.Children.Add(tableCombo);
        tableRow.Children.Add(tableError);
        DockPanel.SetDock(tableRow, Dock.Top);
        leftDock.Children.Add(tableRow);

        // Section title
        var sectionTitle = new TextBlock { Text = "Choose Dishes & Quantities", FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, Margin = new Thickness(0, 0, 0, 10) };
        DockPanel.SetDock(sectionTitle, Dock.Top);
        leftDock.Children.Add(sectionTitle);

        var menuItemsStack = new StackPanel { Spacing = 8 };
        var menuScroll = new ScrollViewer { Content = menuItemsStack };
        leftDock.Children.Add(menuScroll);

        // Right side references
        var cartItemsStack = new StackPanel { Spacing = 8 };
        var totalText = new TextBlock { Text = "Total: ৳0.00", FontSize = 18, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush };
        var cartError = new TextBlock { Foreground = UIHelper.DangerBrush, FontSize = 12, IsVisible = false, TextWrapping = TextWrapping.Wrap };

        // Link class-level cart system to PlaceOrder panel and callback
        _refreshCartCallback = RefreshCart;

        void RefreshCart()
        {
            cartItemsStack.Children.Clear();
            decimal total = 0;
            bool isEven = false;
            foreach (var cartEntry in _cart)
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
                    _cart.Remove(capEntry);
                    RefreshCart();
                };

                entryRow.Children.Add(removeBtn);
                entryBorder.Child = entryRow;
                cartItemsStack.Children.Add(entryBorder);
                total += cartEntry.item.Price * cartEntry.qty;
            }

            totalText.Text = $"Total: ৳{total:F2}";
            if (_cart.Count == 0)
            {
                cartItemsStack.Children.Add(new TextBlock { Text = "Cart is empty.", Foreground = UIHelper.GrayBrush, FontSize = 13 });
            }
        }

        // Render dishes rows inside order screen
        var menuItems = MenuRepository.GetAllMenuItems().Where(m => m.IsAvailable).ToList();
        bool isRowEven = false;
        foreach (var m in menuItems)
        {
            var itemBorder = new Border
            {
                Background = isRowEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12),
                BorderBrush = UIHelper.BorderBrush,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
            isRowEven = !isRowEven;

            var itemRow = new DockPanel();
            
            // Image Thumbnail inside list row
            var imgThumbnail = new Border
            {
                Width = 54,
                Height = 54,
                CornerRadius = new CornerRadius(6),
                ClipToBounds = true,
                Background = UIHelper.LightGrayBrush,
                Margin = new Thickness(0, 0, 12, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var bitmap = UIHelper.LoadBitmap(m.ImagePath);
            if (bitmap != null)
            {
                imgThumbnail.Child = new Image
                {
                    Source = bitmap,
                    Stretch = Stretch.UniformToFill
                };
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
            DockPanel.SetDock(imgThumbnail, Dock.Left);
            itemRow.Children.Add(imgThumbnail);
            
            // Left info
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
            
            itemRow.Children.Add(infoStack);

            // Right side
            var actionsPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, VerticalAlignment = VerticalAlignment.Center };
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
                var existingIndex = _cart.FindIndex(c => c.item.Id == capItem.Id);
                if (existingIndex >= 0)
                {
                    var old = _cart[existingIndex];
                    _cart[existingIndex] = (old.item, old.qty + qty);
                }
                else
                {
                    _cart.Add((capItem, qty));
                }
                RefreshCart();
                qtyBox.Text = "1";
            };

            actionsPanel.Children.Add(qtyGroup);
            actionsPanel.Children.Add(addBtn);
            itemRow.Children.Add(actionsPanel);

            itemBorder.Child = itemRow;
            menuItemsStack.Children.Add(itemBorder);
        }

        // 2. Right Cart Card
        var borderRight = new Border
        {
            Background = UIHelper.WhiteBrush,
            BoxShadow = BoxShadows.Parse("-4 0 10 0 #0A000000"),
            Padding = new Thickness(20)
        };
        Grid.SetColumn(borderRight, 1);

        var rightDock = new DockPanel();

        var cartHeader = new TextBlock
        {
            Text = "My Table Cart",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            Margin = new Thickness(0, 0, 0, 16)
        };
        DockPanel.SetDock(cartHeader, Dock.Top);
        rightDock.Children.Add(cartHeader);

        var placeBtn = new Button
        {
            Content = "✅ Confirm & Submit Order",
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
            tableError.IsVisible = false;
            cartError.IsVisible = false;
            if (tableCombo.SelectedItem == null) { tableError.Text = "Please select a table."; tableError.IsVisible = true; return; }
            if (_cart.Count == 0) { cartError.Text = "Please select dishes first."; cartError.IsVisible = true; return; }

            var selectedItem = (TableComboItem)tableCombo.SelectedItem;
            var table = selectedItem.Table;

            int orderId = OrderRepository.CreateOrder(table.Id, _currentUser.Id);
            foreach (var cartEntry in _cart)
            {
                OrderRepository.AddOrderItem(orderId, cartEntry.item.Id, cartEntry.item.Name, cartEntry.qty, cartEntry.item.Price);
            }
            TableRepository.UpdateTableOccupancyStatus(table.Id);

            _cart.Clear();
            RefreshCart();

            // Refresh table list in combobox
            tableCombo.Items.Clear();
            foreach (var t in TableRepository.GetAllTables())
            {
                tableCombo.Items.Add(new TableComboItem(t));
            }

            cartError.Foreground = UIHelper.GreenBrush;
            cartError.Text = $"Order #{orderId} placed! Waiting for staff approval.";
            cartError.IsVisible = true;
        };

        // Confirm & Submit button at the very bottom
        DockPanel.SetDock(placeBtn, Dock.Bottom);
        rightDock.Children.Add(placeBtn);
        
        // Cart error text right above button
        DockPanel.SetDock(cartError, Dock.Bottom);
        rightDock.Children.Add(cartError);
        
        // Total text above error text
        totalText.Margin = new Thickness(0, 12, 0, 12);
        DockPanel.SetDock(totalText, Dock.Bottom);
        rightDock.Children.Add(totalText);
        
        // Divider above total
        var divider = new Border { Height = 1, Background = UIHelper.BorderBrush, Margin = new Thickness(0, 12, 0, 0) };
        DockPanel.SetDock(divider, Dock.Bottom);
        rightDock.Children.Add(divider);

        // Scroll list of cart items takes all remaining vertical space in the middle!
        var cartScroll = new ScrollViewer { Content = cartItemsStack };
        rightDock.Children.Add(cartScroll);

        borderRight.Child = rightDock;

        RefreshCart(); // trigger empty cart text

        mainGrid.Children.Add(leftDock);
        mainGrid.Children.Add(borderRight);

        return mainGrid;
    }

    private Control BuildSearchPanel()
    {
        var root = new DockPanel { Margin = new Thickness(24) };
        
        var headerText = new TextBlock
        {
            Text = "Search Restaurant Catalog",
            FontSize = 22,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            Margin = new Thickness(0, 0, 0, 16)
        };
        DockPanel.SetDock(headerText, Dock.Top);
        root.Children.Add(headerText);

        var searchBox = new TextBox 
        { 
            Watermark = "🔍 Type item name or category (e.g., Burger, Dessert, Drinks)...", 
            Width = 500, 
            HorizontalAlignment = HorizontalAlignment.Left, 
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            Padding = new Thickness(12, 10),
            Background = UIHelper.WhiteBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(8),
            Margin = new Thickness(0, 0, 0, 20)
        };
        DockPanel.SetDock(searchBox, Dock.Top);
        root.Children.Add(searchBox);

        var resultsPanel = new StackPanel { Spacing = 10 };
        var scroll = new ScrollViewer { Content = resultsPanel };
        root.Children.Add(scroll);

        // Empty state container
        var emptyState = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 16,
            Margin = new Thickness(0, 60, 0, 0)
        };
        
        var emptyIcon = new TextBlock
        {
            Text = "🔍",
            FontSize = 48,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        var emptyTitle = new TextBlock
        {
            Text = "Find Your Next Meal",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        var emptySub = new TextBlock
        {
            Text = "Start typing the name or category of a dish to search the menu.",
            FontSize = 13,
            Foreground = UIHelper.GrayBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
        
        emptyState.Children.Add(emptyIcon);
        emptyState.Children.Add(emptyTitle);
        emptyState.Children.Add(emptySub);
        resultsPanel.Children.Add(emptyState);

        searchBox.TextChanged += (_, _) =>
        {
            resultsPanel.Children.Clear();
            string q = searchBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(q))
            {
                resultsPanel.Children.Add(emptyState);
                return;
            }
            
            var results = MenuRepository.GetAllMenuItems()
                .Where(m => m.IsAvailable && (m.Name.Contains(q, StringComparison.OrdinalIgnoreCase) || m.Category.Contains(q, StringComparison.OrdinalIgnoreCase)))
                .ToList();
                
            if (results.Count == 0)
            {
                var noResultState = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 10,
                    Margin = new Thickness(0, 60, 0, 0)
                };
                noResultState.Children.Add(new TextBlock { Text = "🍽️💨", FontSize = 40, HorizontalAlignment = HorizontalAlignment.Center });
                noResultState.Children.Add(new TextBlock { Text = "No Dishes Found", FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, HorizontalAlignment = HorizontalAlignment.Center });
                noResultState.Children.Add(new TextBlock { Text = $"We couldn't find any dishes matching '{q}'. Try another search query.", FontSize = 12, Foreground = UIHelper.GrayBrush, HorizontalAlignment = HorizontalAlignment.Center });
                resultsPanel.Children.Add(noResultState);
                return;
            }

            bool isEven = false;
            foreach (var m in results)
            {
                var card = new Border
                {
                    Background = isEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(16, 12),
                    BorderBrush = UIHelper.BorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                isEven = !isEven;

                var cardRow = new DockPanel();
                
                var info = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Spacing = 4 };
                info.Children.Add(new TextBlock { Text = m.Name, FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });
                
                var badge = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
                badge.Children.Add(new Border
                {
                    Background = UIHelper.GetCategoryBrush(m.Category),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6, 2),
                    Child = new TextBlock { Text = m.Category, FontSize = 9, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                });
                info.Children.Add(badge);
                cardRow.Children.Add(info);

                var price = new TextBlock { Text = $"৳{m.Price:F2}", FontSize = 15, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush, VerticalAlignment = VerticalAlignment.Center };
                DockPanel.SetDock(price, Dock.Right);
                cardRow.Children.Add(price);

                card.Child = cardRow;
                resultsPanel.Children.Add(card);
            }
        };

        return root;
    }

    private Control BuildMyOrdersAndBillsPanel()
    {
        var mainDock = new DockPanel { Margin = new Thickness(24) };

        var topStack = new StackPanel { Spacing = 16, Margin = new Thickness(0, 0, 0, 16) };

        var title = new TextBlock 
        { 
            Text = "Active Orders & Bills Ledger", 
            FontSize = 22, 
            FontWeight = FontWeight.Bold, 
            Foreground = UIHelper.DarkBrush 
        };
        topStack.Children.Add(title);

        DockPanel.SetDock(topStack, Dock.Top);
        mainDock.Children.Add(topStack);

        var listStack = new StackPanel { Spacing = 14 };
        var scroll = new ScrollViewer { Content = listStack }; // dynamic full height!
        mainDock.Children.Add(scroll);

        void RefreshOrdersList()
        {
            listStack.Children.Clear();
            var orders = OrderRepository.GetAllOrders().Where(o => o.UserId == _currentUser.Id).ToList();
            var tables = TableRepository.GetAllTables();

            foreach (var order in orders)
            {
                var table = tables.FirstOrDefault(t => t.Id == order.TableId);
                var orderItems = OrderRepository.GetOrderItems(order.Id);
                var itemsSummary = string.Join(", ", orderItems.Select(i => $"{i.MenuItemName} (x{i.Quantity})"));

                var card = new Border
                {
                    Background = UIHelper.WhiteBrush,
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(20),
                    BoxShadow = UIHelper.CardShadow,
                    Margin = new Thickness(0, 0, 0, 12)
                };

                var cardStack = new StackPanel { Spacing = 10 };

                // Header
                var headerRow = new DockPanel();
                var idText = new TextBlock { Text = $"Order #{order.Id}", FontSize = 16, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
                var timeText = new TextBlock { Text = order.OrderTime.ToString("g"), FontSize = 12, Foreground = UIHelper.GrayBrush, VerticalAlignment = VerticalAlignment.Center };
                DockPanel.SetDock(timeText, Dock.Right);
                headerRow.Children.Add(timeText);
                headerRow.Children.Add(idText);
                cardStack.Children.Add(headerRow);

                // Table & items summary
                string waiterInfo = string.IsNullOrEmpty(order.ConfirmedBy) ? "" : $"  |  🧑‍🍳 Waiter: {order.ConfirmedBy}";
                var tableText = new TextBlock { Text = $"🪑 Table: {table?.TableNumber ?? order.TableId}  |  👥 Capacity: {table?.Capacity ?? 0} seats{waiterInfo}", FontSize = 13, Foreground = UIHelper.GrayBrush };
                var itemsText = new TextBlock { Text = $"🍽️ Items: {itemsSummary}", FontSize = 13, Foreground = UIHelper.DarkBrush, TextWrapping = TextWrapping.Wrap };
                cardStack.Children.Add(tableText);
                cardStack.Children.Add(itemsText);

                // Status Badge
                var badgePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
                
                IBrush badgeBg = UIHelper.GrayBrush;
                string statusLabel = order.Status;

                if (order.Status == "Pending")
                {
                    badgeBg = new SolidColorBrush(Color.Parse("#FF9F43")); // Orange
                    statusLabel = "Waiting for Staff Approval";
                }
                else if (order.Status == "Confirmed")
                {
                    badgeBg = UIHelper.BlueBrush;
                    statusLabel = "Preparing Food";
                }
                else if (order.Status == "Canceled")
                {
                    badgeBg = UIHelper.DangerBrush;
                    statusLabel = "Canceled by Staff";
                }
                else if (order.Status == "Completed")
                {
                    badgeBg = UIHelper.GreenBrush;
                    statusLabel = "Completed & Paid";
                }

                var statusBadge = new Border
                {
                    Background = badgeBg,
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 4),
                    Child = new TextBlock { Text = statusLabel, FontSize = 11, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                };
                badgePanel.Children.Add(statusBadge);
                cardStack.Children.Add(badgePanel);

                // Cancellation reason if Canceled
                if (order.Status == "Canceled" && !string.IsNullOrEmpty(order.CancelReason))
                {
                    var reasonBorder = new Border
                    {
                        Background = new SolidColorBrush(Color.Parse("#FFF5F5")),
                        BorderBrush = new SolidColorBrush(Color.Parse("#FFE3E3")),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(6),
                        Padding = new Thickness(12),
                        Margin = new Thickness(0, 4, 0, 0)
                    };
                    reasonBorder.Child = new TextBlock 
                    { 
                        Text = $"💬 Staff Message: {order.CancelReason}", 
                        FontSize = 12, 
                        FontWeight = FontWeight.Bold, 
                        Foreground = UIHelper.DangerBrush,
                        TextWrapping = TextWrapping.Wrap
                    };
                    cardStack.Children.Add(reasonBorder);
                }

                // Bill Section
                var bill = BillRepository.GetBillByOrderId(order.Id);
                if (bill != null)
                {
                    var billBorder = new Border
                    {
                        Background = new SolidColorBrush(Color.Parse("#F7F9FC")),
                        BorderBrush = UIHelper.BorderBrush,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(16),
                        Margin = new Thickness(0, 10, 0, 0)
                    };

                    var billStack = new StackPanel { Spacing = 8 };
                    
                    var billHeaderRow = new DockPanel();
                    var billTitleText = new TextBlock { Text = $"💳 Digital Bill #{bill.Id}", FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
                    string statusStr = bill.IsPaid ? "Paid" : "Unpaid / Pending";
                    IBrush statusBg = bill.IsPaid ? UIHelper.GreenBrush : UIHelper.DangerBrush;
                    if (order.Status == "Canceled")
                    {
                        statusStr = "Canceled";
                        statusBg = UIHelper.GrayBrush;
                    }

                    var billStatusText = new Border
                    {
                        Background = statusBg,
                        CornerRadius = new CornerRadius(4),
                        Padding = new Thickness(6, 2),
                        Child = new TextBlock { Text = statusStr, FontSize = 9, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
                    };
                    DockPanel.SetDock(billStatusText, Dock.Right);
                    billHeaderRow.Children.Add(billStatusText);
                    billHeaderRow.Children.Add(billTitleText);
                    billStack.Children.Add(billHeaderRow);

                    decimal subtotal = bill.Subtotal;
                    decimal tax = bill.Tax;
                    decimal grandTotal = bill.GrandTotal;

                    var priceDetails = new UniformGrid { Columns = 3 };
                    priceDetails.Children.Add(new StackPanel { Children = { new TextBlock { Text = "Subtotal", FontSize = 11, Foreground = UIHelper.GrayBrush }, new TextBlock { Text = $"৳{subtotal:F2}", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush } } });
                    priceDetails.Children.Add(new StackPanel { Children = { new TextBlock { Text = $"Tax ({bill.TaxRate:F1}%)", FontSize = 11, Foreground = UIHelper.GrayBrush }, new TextBlock { Text = $"৳{tax:F2}", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush } } });
                    priceDetails.Children.Add(new StackPanel { Children = { new TextBlock { Text = "Grand Total", FontSize = 11, Foreground = UIHelper.GrayBrush }, new TextBlock { Text = $"৳{grandTotal:F2}", FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush } } });
                    billStack.Children.Add(priceDetails);

                    // If bill is unpaid, show the "Pay & Confirm" button!
                    if (!bill.IsPaid && order.Status != "Canceled")
                    {
                        var payBtn = new Button
                        {
                            Content = "💳 Pay Bill Online",
                            Background = UIHelper.GreenBrush,
                            Foreground = UIHelper.WhiteBrush,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            Padding = new Thickness(0, 10),
                            CornerRadius = new CornerRadius(6),
                            FontWeight = FontWeight.Bold,
                            FontSize = 12,
                            BorderThickness = new Thickness(0),
                            Margin = new Thickness(0, 8, 0, 0)
                        };

                        var capBill = bill;
                        var capOrder = order;
                        payBtn.Click += (_, _) =>
                        {
                            BillRepository.MarkAsPaid(capBill.Id);
                            
                            // Reload this panel
                            LoadPanel("📦 My Orders & Bills");
                        };
                        billStack.Children.Add(payBtn);
                    }

                    billBorder.Child = billStack;
                    cardStack.Children.Add(billBorder);
                }

                card.Child = cardStack;
                listStack.Children.Add(card);
            }

            if (listStack.Children.Count == 0)
            {
                var emptyContainer = new StackPanel 
                { 
                    HorizontalAlignment = HorizontalAlignment.Center, 
                    VerticalAlignment = VerticalAlignment.Center, 
                    Spacing = 16,
                    Margin = new Thickness(0, 80, 0, 0)
                };
                emptyContainer.Children.Add(new TextBlock { Text = "📦🍃", FontSize = 48, HorizontalAlignment = HorizontalAlignment.Center });
                emptyContainer.Children.Add(new TextBlock { Text = "No Orders Placed Yet", FontSize = 16, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush, HorizontalAlignment = HorizontalAlignment.Center });
                emptyContainer.Children.Add(new TextBlock { Text = "Go to the 'Place Order' section to order delicious dishes tableside!", FontSize = 13, Foreground = UIHelper.GrayBrush, HorizontalAlignment = HorizontalAlignment.Center });
                listStack.Children.Add(emptyContainer);
            }
        }

        RefreshOrdersList();

        return mainDock;
    }
}

public class TableComboItem
{
    public RestaurantManagementSystem.Models.RestaurantTable Table { get; }
    public bool IsFree => Table.Status == "Free";
    public string DisplayText => $"Table {Table.TableNumber} (Capacity: {Table.Capacity} seats, {Table.Status})";

    public TableComboItem(RestaurantManagementSystem.Models.RestaurantTable table)
    {
        Table = table;
    }

    public override string ToString() => DisplayText;
}
