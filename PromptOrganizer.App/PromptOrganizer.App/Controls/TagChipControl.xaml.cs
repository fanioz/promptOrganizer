using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PromptOrganizer.App.Controls;

public sealed partial class TagChipControl : UserControl
{
    public TagChipControl()
    {
        this.InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TagChipControl), new PropertyMetadata(string.Empty));
}
