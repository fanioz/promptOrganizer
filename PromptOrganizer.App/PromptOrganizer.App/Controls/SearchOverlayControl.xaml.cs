using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PromptOrganizer.App.Controls;

public sealed partial class SearchOverlayControl : UserControl
{
    public event EventHandler<string>? QueryChanged;
    public event EventHandler<object>? PromptSelected;

    public SearchOverlayControl()
    {
        this.InitializeComponent();
    }

    public bool IsOpen { get; private set; }

    public void OpenOverlay()
    {
        Root.Visibility = Visibility.Visible;
        VisualStateManager.GoToState(this, "Open", true);
        QueryBox.Focus(FocusState.Programmatic);
        QueryBox.Text = string.Empty;
        ResultsList.ItemsSource = null;
        IsOpen = true;
    }

    public void CloseOverlay()
    {
        VisualStateManager.GoToState(this, "Closed", true);
        Root.Visibility = Visibility.Collapsed;
        IsOpen = false;
    }

    public void SetResults(object results)
    {
        ResultsList.ItemsSource = results;
    }

    private void QueryBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        QueryChanged?.Invoke(this, QueryBox.Text);
    }

    private void ResultsList_ItemClick(object sender, ItemClickEventArgs e)
    {
        PromptSelected?.Invoke(this, e.ClickedItem);
    }
}
