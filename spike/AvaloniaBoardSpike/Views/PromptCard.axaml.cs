using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaBoardSpike.Views;

public partial class PromptCard : UserControl
{
    public static readonly RoutedEvent<RoutedEventArgs> EditRequestedEvent =
        RoutedEvent.Register<PromptCard, RoutedEventArgs>(nameof(EditRequestedEvent), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> CopyRequestedEvent =
        RoutedEvent.Register<PromptCard, RoutedEventArgs>(nameof(CopyRequestedEvent), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs> EditRequested
    {
        add => AddHandler(EditRequestedEvent, value);
        remove => RemoveHandler(EditRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs> CopyRequested
    {
        add => AddHandler(CopyRequestedEvent, value);
        remove => RemoveHandler(CopyRequestedEvent, value);
    }

    public PromptCard()
    {
        InitializeComponent();

        // Class toggles mirror the Uno card's own event wiring; the pure pseudo-class
        // path (:pointerover on CardRoot) is also wired in XAML above.
        var root = this.FindControl<Border>("CardRoot")!;
        root.PointerEntered += (_, _) => Classes.Add("hover");
        root.PointerExited += (_, _) => { Classes.Remove("hover"); Classes.Remove("pressed"); };
        root.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                Classes.Add("pressed");
        };
        root.PointerReleased += (_, _) => Classes.Remove("pressed");
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private PromptCardModel? Card => DataContext as PromptCardModel;

    private void OnFavoriteClick(object? sender, RoutedEventArgs e)
    {
        if (Card is { } card)
            card.IsFavorite = !card.IsFavorite;
    }

    private void OnCopyClick(object? sender, RoutedEventArgs e)
        => RaiseEvent(new RoutedEventArgs(CopyRequestedEvent));

    private void OnEditClick(object? sender, RoutedEventArgs e)
        => RaiseEvent(new RoutedEventArgs(EditRequestedEvent));
}
