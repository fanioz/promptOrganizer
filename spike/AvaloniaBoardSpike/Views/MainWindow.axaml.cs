using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AvaloniaBoardSpike.Views;

public partial class MainWindow : Window
{
    private bool _dark;
    private bool _modalOpen;
    private CancellationTokenSource? _modalCloseCts;

    public BoardColumn IdeAwal { get; } = MockData.IdeAwal();
    public BoardColumn Dikembangkan { get; } = MockData.Dikembangkan();
    public BoardColumn SiapDigunakan { get; } = MockData.SiapDigunakan();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        AddHandler(PromptCard.EditRequestedEvent, OnCardEdit);
        AddHandler(PromptCard.CopyRequestedEvent, OnCardCopy);

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private static PromptCardModel? CardFrom(RoutedEventArgs e)
        => (e.Source as Control)?.DataContext as PromptCardModel;

    private void OnCardEdit(object? sender, RoutedEventArgs e)
    {
        if (CardFrom(e) is { } card)
            OpenEditor(card);
    }

    private async void OnCardCopy(object? sender, RoutedEventArgs e)
    {
        if (CardFrom(e) is not { } card)
            return;
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null)
            await clipboard.SetTextAsync(card.Body);
    }

    private async void OnCopyBodyClick(object? sender, RoutedEventArgs e)
    {
        var body = this.FindControl<TextBox>("EditorBody")!;
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null)
            await clipboard.SetTextAsync(body.Text ?? "");
    }

    private void OnNewPrompt(object? sender, RoutedEventArgs e) => OpenEditor(null);

    private void OnThemeToggle(object? sender, RoutedEventArgs e)
    {
        _dark = !_dark;
        if (Application.Current is { } app)
            app.RequestedThemeVariant = _dark ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    private void OnBackdropPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Only close when the backdrop itself is pressed, not a click bubbling from the panel.
        if (ReferenceEquals(sender, e.Source))
            CloseEditor();
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e) => CloseEditor();

    private void OpenEditor(PromptCardModel? card)
    {
        _modalCloseCts?.Cancel();
        _modalOpen = true;

        var heading = this.FindControl<TextBlock>("EditorHeading")!;
        var title = this.FindControl<TextBox>("EditorTitle")!;
        var tags = this.FindControl<TextBox>("EditorTags")!;
        var body = this.FindControl<TextBox>("EditorBody")!;
        var root = this.FindControl<Grid>("ModalRoot")!;
        var backdrop = this.FindControl<Border>("Backdrop")!;
        var panel = this.FindControl<Border>("ModalPanel")!;

        heading.Text = card is null ? "New Prompt" : "Edit Prompt";
        title.Text = card?.Title ?? "";
        tags.Text = card is null ? "" : string.Join(", ", card.Tags);
        body.Text = card?.Body ?? "";

        root.IsVisible = true;
        backdrop.Opacity = 0.4;
        panel.Opacity = 1;
        panel.RenderTransform = TransformOperations.Parse("scale(1)");
        body.Focus();
    }

    private async void CloseEditor()
    {
        if (!_modalOpen)
            return;
        _modalOpen = false;
        _modalCloseCts?.Cancel();
        _modalCloseCts = new CancellationTokenSource();
        var token = _modalCloseCts.Token;

        var backdrop = this.FindControl<Border>("Backdrop")!;
        var panel = this.FindControl<Border>("ModalPanel")!;
        backdrop.Opacity = 0;
        panel.Opacity = 0;
        panel.RenderTransform = TransformOperations.Parse("scale(0.98)");
        try
        {
            await Task.Delay(220, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (!_modalOpen)
            this.FindControl<Grid>("ModalRoot")!.IsVisible = false;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && _modalOpen)
        {
            CloseEditor();
            e.Handled = true;
        }
        else if (e.Key == Key.N && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            OpenEditor(null);
            e.Handled = true;
        }
        else if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control) && _modalOpen)
        {
            CloseEditor();
            e.Handled = true;
        }
        base.OnKeyDown(e);
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        await Task.Delay(150); // let the first frame present
        if (Program.StartupStopwatch.IsRunning)
            Program.StartupStopwatch.Stop();
        var p = Process.GetCurrentProcess();
        Console.WriteLine(
            $"PROTOTYPE-SPIKE: startup-to-first-frame {Program.StartupStopwatch.ElapsedMilliseconds} ms; " +
            $"working set {p.WorkingSet64 / 1024 / 1024} MB; private memory {p.PagedMemorySize64 / 1024 / 1024} MB");
        if (Program.Autoclose)
        {
            await Task.Delay(400);
            Close();
        }
    }
}
