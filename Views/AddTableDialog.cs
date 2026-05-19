using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RestaurantManagementSystem.Data;

namespace RestaurantManagementSystem.Views;

public class AddTableDialog : Window
{
    private TextBox _tableNumBox = new();
    private TextBox _capacityBox = new();
    private TextBlock _errorText = new();

    public AddTableDialog()
    {
        Title = "Add Table";
        Width = 340;
        Height = 320;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        Background = UIHelper.WhiteBrush;

        BuildUI();
    }

    private void BuildUI()
    {
        var root = new DockPanel();

        // 1. Header Bar
        var header = new Border
        {
            Height = 60,
            Background = UIHelper.AccentBrush,
            Child = new TextBlock
            {
                Text = "ADD NEW DINING TABLE",
                Foreground = UIHelper.WhiteBrush,
                FontSize = 15,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                LetterSpacing = 1.0
            }
        };
        DockPanel.SetDock(header, Dock.Top);
        root.Children.Add(header);

        // 2. Content stack
        var stack = new StackPanel { Margin = new Thickness(24), Spacing = 12 };

        _tableNumBox = CreateStyledInput("e.g. 9");
        _capacityBox = CreateStyledInput("e.g. 4");
        _errorText = new TextBlock
        {
            Foreground = UIHelper.DangerBrush,
            FontSize = 12,
            IsVisible = false,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var saveBtn = new Button
        {
            Content = "➕ Add Table to Seating",
            Background = UIHelper.GreenBrush,
            Foreground = UIHelper.WhiteBrush,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(0, 12),
            CornerRadius = new CornerRadius(8),
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0),
            FontSize = 13
        };
        saveBtn.Click += OnSave;

        stack.Children.Add(CreateStyledLabel("Table Number"));
        stack.Children.Add(_tableNumBox);
        stack.Children.Add(CreateStyledLabel("Seating Capacity"));
        stack.Children.Add(_capacityBox);
        stack.Children.Add(_errorText);
        stack.Children.Add(saveBtn);

        root.Children.Add(stack);
        Content = root;
    }

    private TextBox CreateStyledInput(string watermark)
    {
        return new TextBox
        {
            Watermark = watermark,
            Padding = new Thickness(12, 8),
            VerticalContentAlignment = VerticalAlignment.Center,
            Background = UIHelper.LightGrayBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(6),
            FontSize = 13,
            Foreground = UIHelper.DarkBrush,
            CaretBrush = UIHelper.DarkBrush
        };
    }

    private TextBlock CreateStyledLabel(string text)
    {
        return new TextBlock
        {
            Text = text.ToUpper(),
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            Margin = new Thickness(0, 4, 0, 0),
            LetterSpacing = 0.5
        };
    }

    private void OnSave(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _errorText.IsVisible = false;
        if (!int.TryParse(_tableNumBox.Text, out int tableNum) || tableNum <= 0) { _errorText.Text = "Please enter a valid table number."; _errorText.IsVisible = true; return; }
        if (!int.TryParse(_capacityBox.Text, out int capacity) || capacity <= 0) { _errorText.Text = "Please enter a valid capacity."; _errorText.IsVisible = true; return; }
        TableRepository.AddTable(tableNum, capacity);
        Close();
    }
}
