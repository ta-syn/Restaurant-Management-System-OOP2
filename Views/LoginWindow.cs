using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using System;

namespace RestaurantManagementSystem.Views;

public class LoginWindow : Window
{
    private TextBox _usernameBox = new();
    private TextBox _passwordBox = new();
    private TextBlock _errorText = new();

    public LoginWindow()
    {
        Title = "Sign In — Restaurant Management System";
        Width = 460;
        Height = 620;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        CanResize = false;

        // Force Light Theme variant for controls inside this window so inputs stay highly readable under focus
        RequestedThemeVariant = ThemeVariant.Light;
        
        // Gradient dark background
        Background = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.Parse("#1A1A2E"), 0.0),
                new GradientStop(Color.Parse("#16213E"), 1.0)
            }
        };

        BuildUI();
    }

    private void BuildUI()
    {
        var rootStack = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 16
        };

        // Center card container
        var card = new Border
        {
            Width = 400,
            Background = UIHelper.WhiteBrush,
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(36, 32),
            BoxShadow = UIHelper.GlowShadow,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var cardStack = new StackPanel { Spacing = 18 };

        // 60px red circle with restaurant icon 🍽️
        var logoBorder = new Border
        {
            Width = 60,
            Height = 60,
            CornerRadius = new CornerRadius(30),
            Background = UIHelper.AccentBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 4),
            Child = new TextBlock
            {
                Text = "🍽️",
                FontSize = 28,
                Foreground = UIHelper.WhiteBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var titleText = new TextBlock
        {
            Text = "Restaurant Management System",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };

        var subtitleText = new TextBlock
        {
            Text = "Sign in to your account",
            FontSize = 13,
            Foreground = UIHelper.GrayBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, -8, 0, 8)
        };

        // Username section with 👤 icon prefix inside InnerLeftContent
        var userLabelStack = new StackPanel { Spacing = 6 };
        var userLabel = new TextBlock { Text = "Username", FontSize = 12, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
        
        _usernameBox = new TextBox
        {
            Watermark = "Enter username",
            Background = new SolidColorBrush(Color.Parse("#F5F5F5")),
            Foreground = UIHelper.DarkBrush,
            FontSize = 13,
            CornerRadius = new CornerRadius(8),
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            Padding = new Thickness(12, 10),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            VerticalAlignment = VerticalAlignment.Center,
            InnerLeftContent = new TextBlock
            {
                Text = "👤",
                FontSize = 14,
                Foreground = UIHelper.GrayBrush,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 4, 0)
            }
        };
        
        userLabelStack.Children.Add(userLabel);
        userLabelStack.Children.Add(_usernameBox);
  
        // Password section with 🔒 icon prefix inside InnerLeftContent
        var passLabelStack = new StackPanel { Spacing = 6 };
        var passLabel = new TextBlock { Text = "Password", FontSize = 12, FontWeight = FontWeight.Bold, Foreground = UIHelper.DarkBrush };
        
        var revealBtn = new TextBlock
        {
            Text = "👁️",
            FontSize = 14,
            Foreground = UIHelper.GrayBrush,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 12, 0),
            Cursor = new Cursor(StandardCursorType.Hand)
        };
  
        _passwordBox = new TextBox
        {
            Watermark = "Enter password",
            PasswordChar = '●',
            Background = new SolidColorBrush(Color.Parse("#F5F5F5")),
            Foreground = UIHelper.DarkBrush,
            FontSize = 13,
            CornerRadius = new CornerRadius(8),
            BorderThickness = new Thickness(1),
            BorderBrush = UIHelper.BorderBrush,
            Padding = new Thickness(12, 10),
            VerticalContentAlignment = VerticalAlignment.Center,
            CaretBrush = UIHelper.DarkBrush,
            VerticalAlignment = VerticalAlignment.Center,
            InnerLeftContent = new TextBlock
            {
                Text = "🔒",
                FontSize = 14,
                Foreground = UIHelper.GrayBrush,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 4, 0)
            },
            InnerRightContent = revealBtn
        };

        revealBtn.PointerPressed += (s, e) =>
        {
            if (_passwordBox.PasswordChar == '●')
            {
                _passwordBox.PasswordChar = '\0'; // Show text
                revealBtn.Text = "👁️‍🗨️"; // Toggle symbol
            }
            else
            {
                _passwordBox.PasswordChar = '●'; // Hide text
                revealBtn.Text = "👁️";
            }
        };

        passLabelStack.Children.Add(passLabel);
        passLabelStack.Children.Add(_passwordBox);

        // Error message
        _errorText = new TextBlock
        {
            Foreground = UIHelper.DangerBrush,
            FontSize = 12,
            IsVisible = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 4, 0, 0)
        };

        // Login button
        var loginBtn = new Border
        {
            Background = UIHelper.AccentBrush,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(0, 14),
            Cursor = new Cursor(StandardCursorType.Hand),
            Child = new TextBlock
            {
                Text = "LOGIN",
                Foreground = UIHelper.WhiteBrush,
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        loginBtn.PointerPressed += (s, e) => OnLoginClick(null, null);
        loginBtn.PointerEntered += (s, e) => { loginBtn.Background = new SolidColorBrush(Color.Parse("#D83A56")); };
        loginBtn.PointerExited += (s, e) => { loginBtn.Background = UIHelper.AccentBrush; };

        // Enter key support
        _passwordBox.KeyDown += (_, e) => { if (e.Key == Key.Return) OnLoginClick(null, null); };

        // Registration toggle row
        var registerLinkStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 4,
            Margin = new Thickness(0, 8, 0, 0)
        };

        var noAccountText = new TextBlock
        {
            Text = "Don't have an account?",
            FontSize = 12,
            Foreground = UIHelper.GrayBrush
        };

        var registerLink = new TextBlock
        {
            Text = "Register here",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.AccentBrush,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        registerLink.PointerPressed += (s, e) =>
        {
            new RegisterWindow().Show();
            Close();
        };

        registerLinkStack.Children.Add(noAccountText);
        registerLinkStack.Children.Add(registerLink);

        cardStack.Children.Add(logoBorder);
        cardStack.Children.Add(titleText);
        cardStack.Children.Add(subtitleText);
        cardStack.Children.Add(userLabelStack);
        cardStack.Children.Add(passLabelStack);
        cardStack.Children.Add(_errorText);
        cardStack.Children.Add(loginBtn);
        cardStack.Children.Add(registerLinkStack);

        card.Child = cardStack;
        rootStack.Children.Add(card);

        Content = rootStack;
    }

    private void OnLoginClick(object? sender, object? e)
    {
        try
        {
            string username = _usernameBox.Text?.Trim() ?? "";
            string password = _passwordBox.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please fill in all fields.");
                return;
            }

            User? user = UserRepository.GetUserByCredentials(username, password);

            if (user == null)
            {
                ShowError("Invalid username or password.");
                return;
            }

            user.ShowDashboard();

            Window dashboard = user.Role switch
            {
                "Admin" => new AdminDashboardWindow(user),
                "Waiter" => new WaiterDashboardWindow(user),
                "Customer" => new CustomerDashboardWindow(user),
                _ => throw new Exception("Unknown role")
            };

            dashboard.Show();
            Close();
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        _errorText.Text = message;
        _errorText.IsVisible = true;
    }
}
