using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Input;

namespace PromptOrganizer.App.Controls;

public sealed partial class PromptCardControl : UserControl
{
    public PromptCardControl()
    {
        this.InitializeComponent();
        VisibleTags = new ObservableCollection<string>();
        VisualStateManager.GoToState(this, "Normal", false);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(PromptCardControl), new PropertyMetadata(string.Empty));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(PromptCardControl), new PropertyMetadata(string.Empty));

    public Brush LabelBrush
    {
        get => (Brush)GetValue(LabelBrushProperty);
        set => SetValue(LabelBrushProperty, value);
    }
    public static readonly DependencyProperty LabelBrushProperty =
        DependencyProperty.Register(nameof(LabelBrush), typeof(Brush), typeof(PromptCardControl), new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(0,0,0,0))));

    public IEnumerable<string> Tags
    {
        get => (IEnumerable<string>)GetValue(TagsProperty);
        set => SetValue(TagsProperty, value);
    }
    public static readonly DependencyProperty TagsProperty =
        DependencyProperty.Register(nameof(Tags), typeof(IEnumerable<string>), typeof(PromptCardControl), new PropertyMetadata(null, OnTagsChanged));

    public int MaxTagsToShow
    {
        get => (int)GetValue(MaxTagsToShowProperty);
        set => SetValue(MaxTagsToShowProperty, value);
    }
    public static readonly DependencyProperty MaxTagsToShowProperty =
        DependencyProperty.Register(nameof(MaxTagsToShow), typeof(int), typeof(PromptCardControl), new PropertyMetadata(3, OnTagsChanged));

    public ObservableCollection<string> VisibleTags { get; }

    public int OverflowCount { get; private set; }

    private static void OnTagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (PromptCardControl)d;
        ctrl.UpdateTags();
    }

    private void UpdateTags()
    {
        VisibleTags.Clear();
        var source = Tags?.ToList() ?? new List<string>();
        var take = Math.Min(source.Count, MaxTagsToShow);
        for (var i = 0; i < take; i++) VisibleTags.Add(source[i]);
        OverflowCount = source.Count - take;
        if (OverflowChip != null)
        {
            if (OverflowCount > 0)
            {
                OverflowChip.Text = $"+{OverflowCount}";
                OverflowChip.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
            else
            {
                OverflowChip.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }
    }

    public string BodyPreview
    {
        get => (string)GetValue(BodyPreviewProperty);
        set => SetValue(BodyPreviewProperty, value);
    }
    public static readonly DependencyProperty BodyPreviewProperty =
        DependencyProperty.Register(nameof(BodyPreview), typeof(string), typeof(PromptCardControl), new PropertyMetadata(string.Empty));

    public event RoutedEventHandler OpenRequested;
    public event RoutedEventHandler HistoryRequested;
    public event RoutedEventHandler CopyRequested;
    public event RoutedEventHandler FavoriteToggled;
    public event RoutedEventHandler EditRequested;
    public event RoutedEventHandler DeleteRequested;

    public int VersionCount
    {
        get => (int)GetValue(VersionCountProperty);
        set => SetValue(VersionCountProperty, value);
    }
    public static readonly DependencyProperty VersionCountProperty =
        DependencyProperty.Register(nameof(VersionCount), typeof(int), typeof(PromptCardControl), new PropertyMetadata(1));

    public bool IsFavorite
    {
        get => (bool)GetValue(IsFavoriteProperty);
        set => SetValue(IsFavoriteProperty, value);
    }
    public static readonly DependencyProperty IsFavoriteProperty =
        DependencyProperty.Register(nameof(IsFavorite), typeof(bool), typeof(PromptCardControl), new PropertyMetadata(false, OnFavoriteChanged));

    private static void OnFavoriteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (PromptCardControl)d;
        ctrl.UpdateFavoriteVisual();
    }

    private void UpdateFavoriteVisual()
    {
        if (FavoriteButton != null)
        {
            var accent = (Brush)Application.Current.Resources["AccentVioletBrush"];
            var normal = (Brush)Application.Current.Resources["SecondaryButtonForegroundBrush"];
            FavoriteButton.Foreground = IsFavorite ? accent : normal;
        }
    }

    private void OnOpenInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        OpenRequested?.Invoke(this, new RoutedEventArgs());
        args.Handled = true;
    }

    private void OnCopyInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        CopyRequested?.Invoke(this, new RoutedEventArgs());
        args.Handled = true;
    }

    private void OnFavoriteInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        FavoriteToggled?.Invoke(this, new RoutedEventArgs());
        args.Handled = true;
    }

    private void FavoriteButton_Click(object sender, RoutedEventArgs e)
    {
        FavoriteToggled?.Invoke(this, new RoutedEventArgs());
    }

    private void HistoryButton_Click(object sender, RoutedEventArgs e)
    {
        HistoryRequested?.Invoke(this, new RoutedEventArgs());
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        CopyRequested?.Invoke(this, new RoutedEventArgs());
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        EditRequested?.Invoke(this, new RoutedEventArgs());
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        DeleteRequested?.Invoke(this, new RoutedEventArgs());
    }

    private void CardBorder_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "PointerOver", true);
    }

    private void CardBorder_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Normal", true);
    }

    private void CardBorder_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Pressed", true);
    }

    private void CardBorder_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "PointerOver", true);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateFavoriteVisual();
        var sb = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        var fade = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation
        {
            To = 1,
            Duration = new Microsoft.UI.Xaml.Duration(System.TimeSpan.FromMilliseconds(140))
        };
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(fade, CardBorder);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(fade, "Opacity");
        sb.Children.Add(fade);
        sb.Begin();
    }
}
