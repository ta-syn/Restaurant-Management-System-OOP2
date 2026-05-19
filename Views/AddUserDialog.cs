using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RestaurantManagementSystem.Data;

namespace RestaurantManagementSystem.Views;

public class AddUserDialog : Window
{
    private TextBox _nameBox = new();
    private TextBox _usernameBox = new();
    private TextBox _passwordBox = new();
    private ComboBox _roleCombo = new();
    private TextBlock _errorText = new();

    public AddUserDialog()
    {
        Title = "Add New User";
        Width = 380;
        Height = 480;
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
                Text = "ADD STAFF MEMBER",
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

        // 2. Content Stack
        var stack = new StackPanel { Margin = new Thickness(24), Spacing = 12 };

        _nameBox = CreateStyledInput("Enter full name");
        _usernameBox = CreateStyledInput("Enter username");
        _passwordBox = CreateStyledInput("Enter password");
        _passwordBox.PasswordChar = '●';

        _roleCombo = new ComboBox
        {
            Width = double.NaN,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Padding = new Thickness(10, 6),
            Background = UIHelper.WhiteBrush,
            Foreground = UIHelper.DarkBrush,
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            CornerRadius = new CornerRadius(6)
        };
        _roleCombo.Items.Add("Admin");
        _roleCombo.Items.Add("Waiter");
        _roleCombo.Items.Add("Customer");
        _roleCombo.SelectedIndex = 1;

        _errorText = new TextBlock
        {
            Foreground = UIHelper.DangerBrush,
            FontSize = 12,
            IsVisible = false,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var saveBtn = new Button
        {
            Content = "➕ Add User Profile",
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

        stack.Children.Add(CreateStyledLabel("Full Name"));
        stack.Children.Add(_nameBox);
        stack.Children.Add(CreateStyledLabel("System Username"));
        stack.Children.Add(_usernameBox);
        stack.Children.Add(CreateStyledLabel("Secret Password"));
        stack.Children.Add(_passwordBox);
        stack.Children.Add(CreateStyledLabel("Authorized Role"));
        stack.Children.Add(_roleCombo);
        stack.Children.Add(_errorText);
        stack.Children.Add(saveBtn);

        var scroll = new ScrollViewer { Content = stack };
        root.Children.Add(scroll);

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
        string name = _nameBox.Text?.Trim() ?? "";
        string username = _usernameBox.Text?.Trim() ?? "";
        string password = _passwordBox.Text?.Trim() ?? "";
        string role = _roleCombo.SelectedItem?.ToString() ?? "Waiter";

        if (string.IsNullOrEmpty(name)) { _errorText.Text = "Name is required."; _errorText.IsVisible = true; return; }
        if (string.IsNullOrEmpty(username)) { _errorText.Text = "Username is required."; _errorText.IsVisible = true; return; }
        if (password.Length < 4) { _errorText.Text = "Password must be at least 4 characters."; _errorText.IsVisible = true; return; }

        UserRepository.AddUser(name, username, password, role);
        Close();
    }
}
