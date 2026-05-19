using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using System;
using System.IO;

namespace RestaurantManagementSystem.Views;

public class MenuItemDialog : Window
{
    private readonly RestaurantManagementSystem.Models.MenuItem? _existingItem;
    private TextBox _nameBox = new();
    private TextBox _categoryBox = new();
    private TextBox _priceBox = new();
    private TextBlock _errorText = new();
    
    private Border _imagePreview = new();
    private string _selectedImagePath = "";

    public MenuItemDialog(RestaurantManagementSystem.Models.MenuItem? item = null)
    {
        _existingItem = item;
        Title = item == null ? "Add Menu Item" : "Edit Menu Item";
        Width = 380;
        Height = 560; // Expanded to perfectly accommodate image preview
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        Background = UIHelper.WhiteBrush;

        if (item != null)
        {
            _selectedImagePath = item.ImagePath;
        }

        BuildUI();
        UpdateImagePreview();
    }

    private void BuildUI()
    {
        var root = new DockPanel();

        // 1. Accent Colored Header Bar
        var header = new Border
        {
            Height = 60,
            Background = UIHelper.AccentBrush,
            Child = new TextBlock
            {
                Text = (Title ?? "").ToUpper(),
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

        // 2. Form Content Area
        var stack = new StackPanel { Margin = new Thickness(24), Spacing = 12 };

        _nameBox = CreateStyledInput("Enter item name", _existingItem?.Name ?? "");
        _categoryBox = CreateStyledInput("e.g. Fast Food, Drinks", _existingItem?.Category ?? "");
        _priceBox = CreateStyledInput("e.g. 150.00", _existingItem?.Price.ToString("F2") ?? "");

        _errorText = new TextBlock
        {
            Foreground = UIHelper.DangerBrush,
            FontSize = 12,
            IsVisible = false,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        // Image Selection Card & Button
        var imageLabel = CreateStyledLabel("Food Photo Preview");
        
        var imageCard = new Border
        {
            Height = 110,
            Background = UIHelper.LightGrayBrush,
            CornerRadius = new CornerRadius(8),
            BorderBrush = UIHelper.BorderBrush,
            BorderThickness = new Thickness(1),
            ClipToBounds = true
        };
        
        _imagePreview = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        imageCard.Child = _imagePreview;

        var uploadBtn = new Button
        {
            Content = "📸 Select Food Photo",
            Background = UIHelper.BlueBrush,
            Foreground = UIHelper.WhiteBrush,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(0, 8),
            CornerRadius = new CornerRadius(6),
            FontWeight = FontWeight.Bold,
            FontSize = 11,
            BorderThickness = new Thickness(0),
            Margin = new Thickness(0, 2, 0, 6)
        };
        uploadBtn.Click += OnUploadImageClick;

        var saveBtn = new Button
        {
            Content = _existingItem == null ? "➕ Add Item to Menu" : "💾 Save Menu Item",
            Background = UIHelper.GreenBrush,
            Foreground = UIHelper.WhiteBrush,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(0, 12),
            CornerRadius = new CornerRadius(8),
            FontWeight = FontWeight.Bold,
            BorderThickness = new Thickness(0),
            FontSize = 13,
            Margin = new Thickness(0, 8, 0, 0)
        };
        saveBtn.Click += OnSave;

        stack.Children.Add(CreateStyledLabel("Item Name"));
        stack.Children.Add(_nameBox);
        stack.Children.Add(CreateStyledLabel("Food Category"));
        stack.Children.Add(_categoryBox);
        stack.Children.Add(CreateStyledLabel("Price (৳)"));
        stack.Children.Add(_priceBox);
        
        stack.Children.Add(imageLabel);
        stack.Children.Add(imageCard);
        stack.Children.Add(uploadBtn);
        
        stack.Children.Add(_errorText);
        stack.Children.Add(saveBtn);

        var scroll = new ScrollViewer { Content = stack };
        root.Children.Add(scroll);

        Content = root;
    }

    private void UpdateImagePreview()
    {
        if (string.IsNullOrEmpty(_selectedImagePath))
        {
            _imagePreview.Child = new TextBlock
            {
                Text = "No Photo Uploaded",
                FontSize = 11,
                Foreground = UIHelper.GrayBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }
        else
        {
            try
            {
                var bitmap = UIHelper.LoadBitmap(_selectedImagePath);
                if (bitmap != null)
                {
                    _imagePreview.Child = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill
                    };
                }
                else
                {
                    _imagePreview.Child = new TextBlock
                    {
                        Text = "⚠️ Unable to load image file",
                        FontSize = 10,
                        Foreground = UIHelper.DangerBrush,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
            }
            catch
            {
                _imagePreview.Child = new TextBlock
                {
                    Text = "⚠️ Unable to load image file",
                    FontSize = 10,
                    Foreground = UIHelper.DangerBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
        }
    }

    private async void OnUploadImageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose Food Photo",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });

        if (files != null && files.Count > 0)
        {
            try
            {
                var file = files[0];
                string srcPath = file.Path.LocalPath;

                string targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "restaurant_images");
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                string ext = Path.GetExtension(srcPath);
                string uniqueName = $"food_{Guid.NewGuid()}{ext}";
                string destPath = Path.Combine(targetDir, uniqueName);

                File.Copy(srcPath, destPath, overwrite: true);

                _selectedImagePath = destPath;
                UpdateImagePreview();
            }
            catch (Exception ex)
            {
                _errorText.Text = $"Image copy failed: {ex.Message}";
                _errorText.IsVisible = true;
            }
        }
    }

    private TextBox CreateStyledInput(string watermark, string text = "")
    {
        return new TextBox
        {
            Watermark = watermark,
            Text = text,
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
        string category = _categoryBox.Text?.Trim() ?? "";
        string priceStr = _priceBox.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(name)) { _errorText.Text = "Name cannot be empty."; _errorText.IsVisible = true; return; }
        if (string.IsNullOrEmpty(category)) { _errorText.Text = "Category cannot be empty."; _errorText.IsVisible = true; return; }
        if (!decimal.TryParse(priceStr, out decimal price)) { _errorText.Text = "Please enter a valid price."; _errorText.IsVisible = true; return; }
        if (price <= 0) { _errorText.Text = "Price must be greater than 0."; _errorText.IsVisible = true; return; }

        if (_existingItem == null) MenuRepository.AddMenuItem(name, category, price, _selectedImagePath);
        else MenuRepository.UpdateMenuItem(_existingItem.Id, name, category, price, _selectedImagePath);
        Close();
    }
}
