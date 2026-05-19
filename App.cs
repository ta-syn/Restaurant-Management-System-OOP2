using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Themes.Fluent;
using Avalonia.Media;
using Avalonia.Styling;
using RestaurantManagementSystem.Views;

namespace RestaurantManagementSystem;

public class App : Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());

        // Global TextBox Focus & Hover overrides to guarantee legibility
        Resources["TextControlBackgroundFocused"] = Brushes.White;
        Resources["TextControlForegroundFocused"] = UIHelper.DarkBrush;
        Resources["TextControlBackgroundPointerOver"] = new SolidColorBrush(Color.Parse("#F5F5F5"));
        Resources["TextControlForegroundPointerOver"] = UIHelper.DarkBrush;

        // Global TextBox Alignment Style to prevent text clipping
        var textBoxStyle = new Style(x => x.OfType<TextBox>());
        textBoxStyle.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center));
        textBoxStyle.Setters.Add(new Setter(TextBox.PaddingProperty, new Thickness(12, 8))); // Perfectly balanced symmetric padding
        textBoxStyle.Setters.Add(new Setter(TextBox.MinHeightProperty, 38.0)); // Plentiful vertical area
        textBoxStyle.Setters.Add(new Setter(TextBox.CaretBrushProperty, UIHelper.DarkBrush)); // Ensure caret is highly contrasty and visible!
        Styles.Add(textBoxStyle);

        // Style the internal TextPresenter directly to ensure perfect alignment and zero clipping
        var textPresenterStyle = new Style(x => x.OfType<TextBox>().Template().Name("PART_TextPresenter"));
        textPresenterStyle.Setters.Add(new Setter(TextPresenter.VerticalAlignmentProperty, VerticalAlignment.Center));
        Styles.Add(textPresenterStyle);

        // Global TextBox Focus & Hover styles to guarantee perfect legibility/contrast in both Light & Dark modes
        var textBoxFocusStyle = new Style(x => x.OfType<TextBox>().Class(":focus"));
        textBoxFocusStyle.Setters.Add(new Setter(TextBox.BackgroundProperty, Brushes.White));
        textBoxFocusStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, UIHelper.DarkBrush));
        textBoxFocusStyle.Setters.Add(new Setter(TextBox.CaretBrushProperty, UIHelper.DarkBrush)); // High contrast dark caret on white background
        Styles.Add(textBoxFocusStyle);

        var textBoxHoverStyle = new Style(x => x.OfType<TextBox>().Class(":pointerover"));
        textBoxHoverStyle.Setters.Add(new Setter(TextBox.BackgroundProperty, new SolidColorBrush(Color.Parse("#F5F5F5"))));
        textBoxHoverStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, UIHelper.DarkBrush));
        textBoxHoverStyle.Setters.Add(new Setter(TextBox.CaretBrushProperty, UIHelper.DarkBrush));
        Styles.Add(textBoxHoverStyle);

        // Global Button overrides: Bind the ContentPresenter's hover/pressed background and foreground
        // directly to the Button's own Background and Foreground properties.
        // This prevents FluentTheme from overriding custom button colors to white or grey on hover/touch!
        
        var hoverStyle = new Style(x => x.OfType<Button>().Class(":pointerover").Template().Name("PART_ContentPresenter"));
        hoverStyle.Setters.Add(new Setter(ContentPresenter.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) }));
        hoverStyle.Setters.Add(new Setter(ContentPresenter.ForegroundProperty, new Binding("Foreground") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) }));
        hoverStyle.Setters.Add(new Setter(ContentPresenter.BorderBrushProperty, new Binding("BorderBrush") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) }));
        Styles.Add(hoverStyle);

        var pressedStyle = new Style(x => x.OfType<Button>().Class(":pressed").Template().Name("PART_ContentPresenter"));
        pressedStyle.Setters.Add(new Setter(ContentPresenter.BackgroundProperty, new Binding("Background") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) }));
        pressedStyle.Setters.Add(new Setter(ContentPresenter.ForegroundProperty, new Binding("Foreground") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) }));
        pressedStyle.Setters.Add(new Setter(ContentPresenter.BorderBrushProperty, new Binding("BorderBrush") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) }));
        Styles.Add(pressedStyle);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnLastWindowClose;
            desktop.MainWindow = new LoginWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
