using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Views;

public class OrderItemsWindow : Window
{
    public OrderItemsWindow(int orderId, Bill? bill = null)
    {
        Title = $"Order #{orderId} Details";
        Width = 480;
        Height = 520;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        CanResize = false;
        Background = UIHelper.WhiteBrush;

        BuildUI(orderId, bill);
    }

    private void BuildUI(int orderId, Bill? bill)
    {
        if (bill == null)
        {
            bill = BillRepository.GetBillByOrderId(orderId);
        }
        var root = new DockPanel();

        // 1. Accent Colored Header Bar
        var header = new Border
        {
            Height = 60,
            Background = UIHelper.AccentBrush,
            Child = new TextBlock
            {
                Text = $"ORDER #{orderId} LOGS",
                Foreground = UIHelper.WhiteBrush,
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                LetterSpacing = 1.0
            }
        };
        DockPanel.SetDock(header, Dock.Top);
        root.Children.Add(header);

        // Form Area
        var stack = new StackPanel { Margin = new Thickness(24), Spacing = 14 };

        var order = OrderRepository.GetOrderById(orderId);
        if (order != null)
        {
            var metaBorder = new Border
            {
                Background = UIHelper.LightGrayBrush,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 10),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var metaStack = new StackPanel { Spacing = 4 };
            metaStack.Children.Add(new TextBlock { Text = $"🪑 Table ID: {order.TableId}", FontSize = 12, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });
            metaStack.Children.Add(new TextBlock { Text = $"👤 Placed by: {order.WaiterName ?? $"User #{order.UserId}"}", FontSize = 12, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush });
            if (!string.IsNullOrEmpty(order.ConfirmedBy))
            {
                metaStack.Children.Add(new TextBlock { Text = $"🧑‍🍳 Confirmed By: {order.ConfirmedBy}", FontSize = 12, FontWeight = FontWeight.Bold, Foreground = UIHelper.BlueBrush });
            }
            metaStack.Children.Add(new TextBlock { Text = $"📅 Date/Time: {order.OrderTime:g}", FontSize = 12, Foreground = UIHelper.GrayBrush });
            metaBorder.Child = metaStack;
            stack.Children.Add(metaBorder);
        }

        stack.Children.Add(new TextBlock
        {
            Text = "Ordered Items List",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush
        });

        // Replacing ListBox with Styled Scrollable StackPanel Rows
        var itemsStack = new StackPanel { Spacing = 6 };
        var itemsScroll = new ScrollViewer { Content = itemsStack, Height = 200 };
        stack.Children.Add(itemsScroll);

        var allMenuItems = MenuRepository.GetAllMenuItems();
        var items = OrderRepository.GetOrderItems(orderId);
        decimal subtotal = 0;
        bool isEven = false;
        foreach (var item in items)
        {
            var itemBorder = new Border
            {
                Background = isEven ? new SolidColorBrush(Color.Parse("#FAFAFA")) : UIHelper.WhiteBrush,
                Padding = new Thickness(12, 10),
                CornerRadius = new CornerRadius(6),
                BorderBrush = UIHelper.BorderBrush,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
            isEven = !isEven;

            var itemRow = new DockPanel();
            
            var imageBorder = new Border
            {
                Width = 36,
                Height = 36,
                CornerRadius = new CornerRadius(6),
                ClipToBounds = true,
                Background = UIHelper.LightGrayBrush,
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(imageBorder, Dock.Left);

            var menuItem = allMenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
            if (menuItem != null && !string.IsNullOrEmpty(menuItem.ImagePath))
            {
                var bmp = UIHelper.LoadBitmap(menuItem.ImagePath);
                if (bmp != null)
                {
                    imageBorder.Child = new Image
                    {
                        Source = bmp,
                        Stretch = Stretch.UniformToFill
                    };
                }
                else
                {
                    imageBorder.Child = new TextBlock
                    {
                        Text = UIHelper.GetCategoryEmoji(menuItem.Category),
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
            }
            else
            {
                imageBorder.Child = new TextBlock
                {
                    Text = "🍽️",
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            var labelText = new TextBlock 
            { 
                Text = $"{item.MenuItemName}", 
                FontSize = 13, 
                FontWeight = FontWeight.Bold, 
                Foreground = UIHelper.DarkBrush,
                VerticalAlignment = VerticalAlignment.Center 
            };
            
            var priceText = new TextBlock 
            { 
                Text = $"x{item.Quantity} = ৳{item.Subtotal:F2}", 
                FontSize = 13, 
                Foreground = UIHelper.AccentBrush,
                VerticalAlignment = VerticalAlignment.Center 
            };
            DockPanel.SetDock(priceText, Dock.Right);
            
            itemRow.Children.Add(priceText);
            itemRow.Children.Add(imageBorder);
            itemRow.Children.Add(labelText);
            itemBorder.Child = itemRow;
            itemsStack.Children.Add(itemBorder);

            subtotal += item.Subtotal;
        }

        if (bill != null)
        {
            var separator = new Border { Height = 1, Background = UIHelper.BorderBrush, Margin = new Thickness(0, 8) };
            stack.Children.Add(separator);

            void AddRow(string label, string value, bool bold = false)
            {
                var row = new DockPanel();
                var lbl = new TextBlock { Text = label, FontSize = 13, FontWeight = bold ? FontWeight.Bold : FontWeight.Normal, Foreground = bold ? UIHelper.DarkBrush : UIHelper.GrayBrush };
                var val = new TextBlock { Text = value, FontSize = 13, FontWeight = bold ? FontWeight.Bold : FontWeight.Normal, Foreground = bold ? UIHelper.AccentBrush : UIHelper.DarkBrush };
                DockPanel.SetDock(val, Dock.Right);
                row.Children.Add(val);
                row.Children.Add(lbl);
                stack.Children.Add(row);
            }

            AddRow("Subtotal:", $"৳{bill.Subtotal:F2}");
            AddRow($"VAT ({bill.TaxRate:F1}%):", $"৳{bill.Tax:F2}");
            AddRow("Grand Total:", $"৳{bill.GrandTotal:F2}", bold: true);
            
            var statusBorder = new Border
            {
                Background = bill.IsPaid ? UIHelper.GreenBrush : UIHelper.DangerBrush,
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 4),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 4, 0, 0),
                Child = new TextBlock { Text = bill.IsPaid ? "✅ PAID" : "⏳ UNPAID", FontSize = 9, FontWeight = FontWeight.Bold, Foreground = UIHelper.WhiteBrush }
            };
            
            var statusRow = new DockPanel();
            var statusLabel = new TextBlock { Text = "Status:", FontSize = 13, Foreground = UIHelper.GrayBrush, VerticalAlignment = VerticalAlignment.Center };
            DockPanel.SetDock(statusBorder, Dock.Right);
            statusRow.Children.Add(statusBorder);
            statusRow.Children.Add(statusLabel);
            stack.Children.Add(statusRow);

            if (bill.IsPaid)
            {
                var timeRow = new DockPanel { Margin = new Thickness(0, 4, 0, 0) };
                var timeLabel = new TextBlock { Text = "Paid At:", FontSize = 13, Foreground = UIHelper.GrayBrush };
                var timeVal = new TextBlock { Text = $"{bill.BillTime:g}", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.GreenBrush };
                DockPanel.SetDock(timeVal, Dock.Right);
                timeRow.Children.Add(timeVal);
                timeRow.Children.Add(timeLabel);
                stack.Children.Add(timeRow);

                if (!string.IsNullOrEmpty(bill.CompletedBy))
                {
                    var staffRow = new DockPanel { Margin = new Thickness(0, 4, 0, 0) };
                    var staffLabel = new TextBlock { Text = "Served By:", FontSize = 13, Foreground = UIHelper.GrayBrush };
                    var staffVal = new TextBlock { Text = bill.CompletedBy, FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.BlueBrush };
                    DockPanel.SetDock(staffVal, Dock.Right);
                    staffRow.Children.Add(staffVal);
                    staffRow.Children.Add(staffLabel);
                    stack.Children.Add(staffRow);
                }
            }
        }
        else
        {
            var summaryBorder = new Border
            {
                Background = UIHelper.LightGrayBrush,
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(14, 10)
            };
            var summaryRow = new DockPanel();
            var summaryLabel = new TextBlock { Text = "Subtotal:", FontSize = 13, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
            var summaryVal = new TextBlock { Text = $"৳{subtotal:F2}", FontSize = 14, FontWeight = FontWeight.Bold, Foreground = UIHelper.AccentBrush };
            DockPanel.SetDock(summaryVal, Dock.Right);
            summaryRow.Children.Add(summaryVal);
            summaryRow.Children.Add(summaryLabel);
            summaryBorder.Child = summaryRow;
            stack.Children.Add(summaryBorder);
        }

        var closeBtn = new Button
        {
            Content = "Close Receipt",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Background = UIHelper.AccentBrush,
            Foreground = UIHelper.WhiteBrush,
            Padding = new Thickness(0, 12),
            CornerRadius = new CornerRadius(8),
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0),
            Margin = new Thickness(0, 10, 0, 0)
        };
        closeBtn.Click += (_, _) => Close();
        stack.Children.Add(closeBtn);

        var mainScroll = new ScrollViewer { Content = stack };
        root.Children.Add(mainScroll);

        Content = root;
    }
}
