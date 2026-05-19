using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Views;

public class RegisterWindow : Window
{
    private TextBox _nameBox = new();
    private TextBox _usernameBox = new();
    private TextBox _passwordBox = new();
    private TextBox _confirmPasswordBox = new();

    private TextBlock _nameError = new();
    private TextBlock _usernameError = new();
    private TextBlock _passwordError = new();
    private TextBlock _confirmPasswordError = new();
    
    private TextBlock _successText = new();

    public RegisterWindow()
    {
        Title = "Create Account — Restaurant Management System";
        Width = 480;
        Height = 700;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        CanResize = false;

        // Force Light Theme variant for controls inside this window so inputs stay highly readable under focus
        RequestedThemeVariant = ThemeVariant.Light;

        // Gradient dark background matching login
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
            Spacing = 12
        };

        // White card center (width 420px, border-radius 16px, glow shadow)
        var card = new Border
        {
            Width = 420,
            Background = UIHelper.WhiteBrush,
            CornerRadius = new CornerRadius(16),
            Padding = new Thickness(32, 28),
            BoxShadow = UIHelper.GlowShadow,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var cardStack = new StackPanel { Spacing = 12 };

        // 60px blue circle (#3498DB) with 📝 icon
        var logoBorder = new Border
        {
            Width = 60,
            Height = 60,
            CornerRadius = new CornerRadius(30),
            Background = UIHelper.BlueBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 4),
            Child = new TextBlock
            {
                Text = "📝",
                FontSize = 28,
                Foreground = UIHelper.WhiteBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var titleText = new TextBlock
        {
            Text = "Create Account",
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var subtitleText = new TextBlock
        {
            Text = "Join Restaurant Management System",
            FontSize = 13,
            Foreground = UIHelper.GrayBrush,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, -6, 0, 10)
        };

        // Success Indicator
        _successText = new TextBlock
        {
            Foreground = UIHelper.GreenBrush,
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = false,
            Margin = new Thickness(0, 0, 0, 8)
        };

        // 1. Full Name field with 👤 icon prefix
        _nameBox = CreateStyledInput("👤", "Enter full name");
        _nameError = CreateErrorText();

        // 2. Username field with 🔑 icon prefix
        _usernameBox = CreateStyledInput("🔑", "Enter username");
        _usernameError = CreateErrorText();

        // 3. Password field with 🔒 icon prefix
        _passwordBox = CreateStyledInput("🔒", "Enter password", isPassword: true);
        _passwordError = CreateErrorText();

        // 4. Confirm Password field with 🔒 icon prefix
        _confirmPasswordBox = CreateStyledInput("🔒", "Confirm password", isPassword: true);
        _confirmPasswordError = CreateErrorText();

        // Register Button
        var registerBtn = new Border
        {
            Background = UIHelper.BlueBrush,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(0, 14),
            Cursor = new Cursor(StandardCursorType.Hand),
            Margin = new Thickness(0, 8, 0, 0),
            Child = new TextBlock
            {
                Text = "REGISTER",
                Foreground = UIHelper.WhiteBrush,
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        registerBtn.PointerPressed += (s, e) => OnRegisterClick(null, null);
        registerBtn.PointerEntered += (s, e) => { registerBtn.Background = new SolidColorBrush(Color.Parse("#2980B9")); };
        registerBtn.PointerExited += (s, e) => { registerBtn.Background = UIHelper.BlueBrush; };

        // Login redirect link
        var loginRedirectStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 4,
            Margin = new Thickness(0, 6, 0, 0)
        };

        var hasAccountText = new TextBlock
        {
            Text = "Already have an account?",
            FontSize = 12,
            Foreground = UIHelper.GrayBrush
        };

        var loginLink = new TextBlock
        {
            Text = "Login here",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.AccentBrush,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        loginLink.PointerPressed += (s, e) =>
        {
            new LoginWindow().Show();
            Close();
        };

        loginRedirectStack.Children.Add(hasAccountText);
        loginRedirectStack.Children.Add(loginLink);

        // Populate Card
        cardStack.Children.Add(logoBorder);
        cardStack.Children.Add(titleText);
        cardStack.Children.Add(subtitleText);
        cardStack.Children.Add(_successText);
        
        cardStack.Children.Add(CreateFieldLabel("Full Name"));
        cardStack.Children.Add(_nameBox);
        cardStack.Children.Add(_nameError);

        cardStack.Children.Add(CreateFieldLabel("Username"));
        cardStack.Children.Add(_usernameBox);
        cardStack.Children.Add(_usernameError);

        cardStack.Children.Add(CreateFieldLabel("Password"));
        cardStack.Children.Add(_passwordBox);
        cardStack.Children.Add(_passwordError);

        cardStack.Children.Add(CreateFieldLabel("Confirm Password"));
        cardStack.Children.Add(_confirmPasswordBox);
        cardStack.Children.Add(_confirmPasswordError);

        cardStack.Children.Add(registerBtn);
        cardStack.Children.Add(loginRedirectStack);

        card.Child = cardStack;
        rootStack.Children.Add(card);

        Content = rootStack;
    }

    private TextBox CreateStyledInput(string iconText, string watermark, bool isPassword = false)
    {
        var box = new TextBox
        {
            Watermark = watermark,
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
                Text = iconText,
                FontSize = 14,
                Foreground = UIHelper.GrayBrush,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 4, 0)
            }
        };

        if (isPassword)
        {
            box.PasswordChar = '●';

            var revealBtn = new TextBlock
            {
                Text = "👁️",
                FontSize = 14,
                Foreground = UIHelper.GrayBrush,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 12, 0),
                Cursor = new Cursor(StandardCursorType.Hand)
            };

            revealBtn.PointerPressed += (s, e) =>
            {
                if (box.PasswordChar == '●')
                {
                    box.PasswordChar = '\0'; // Show text
                    revealBtn.Text = "👁️‍🗨️"; // Toggle symbol
                }
                else
                {
                    box.PasswordChar = '●'; // Hide text
                    revealBtn.Text = "👁️";
                }
            };

            box.InnerRightContent = revealBtn;
        }

        return box;
    }

    private TextBlock CreateFieldLabel(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            Foreground = UIHelper.DarkBrush,
            Margin = new Thickness(0, 2, 0, -2)
        };
    }

    private TextBlock CreateErrorText()
    {
        return new TextBlock
        {
            Foreground = UIHelper.DangerBrush,
            FontSize = 10,
            IsVisible = false,
            Margin = new Thickness(4, 2, 0, 0)
        };
    }

    private bool DoesUsernameExist(string username)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @u";
            cmd.Parameters.AddWithValue("@u", username);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    private async void OnRegisterClick(object? sender, object? e)
    {
        _nameError.IsVisible = false;
        _usernameError.IsVisible = false;
        _passwordError.IsVisible = false;
        _confirmPasswordError.IsVisible = false;
        _successText.IsVisible = false;

        string name = _nameBox.Text?.Trim() ?? "";
        string username = _usernameBox.Text?.Trim() ?? "";
        string password = _passwordBox.Text?.Trim() ?? "";
        string confirmPassword = _confirmPasswordBox.Text?.Trim() ?? "";

        bool isValid = true;

        if (string.IsNullOrWhiteSpace(name))
        {
            _nameError.Text = "Full Name is required.";
            _nameError.IsVisible = true;
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(username))
        {
            _usernameError.Text = "Username is required.";
            _usernameError.IsVisible = true;
            isValid = false;
        }
        else if (DoesUsernameExist(username))
        {
            _usernameError.Text = "Username already exists.";
            _usernameError.IsVisible = true;
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            _passwordError.Text = "Password is required.";
            _passwordError.IsVisible = true;
            isValid = false;
        }
        else if (password.Length < 4)
        {
            _passwordError.Text = "Password must be at least 4 characters.";
            _passwordError.IsVisible = true;
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            _confirmPasswordError.Text = "Confirm Password is required.";
            _confirmPasswordError.IsVisible = true;
            isValid = false;
        }
        else if (password != confirmPassword)
        {
            _confirmPasswordError.Text = "Passwords do not match.";
            _confirmPasswordError.IsVisible = true;
            isValid = false;
        }

        if (!isValid) return;

        try
        {
            // Create user as Customer role by default
            UserRepository.AddUser(name, username, password, "Customer");

            _successText.Text = "Account created successfully!";
            _successText.IsVisible = true;

            // Wait 1 second and redirect to Login
            await Task.Delay(1000);
            
            new LoginWindow().Show();
            Close();
        }
        catch (Exception ex)
        {
            _usernameError.Text = $"Database Error: {ex.Message}";
            _usernameError.IsVisible = true;
        }
    }
}
