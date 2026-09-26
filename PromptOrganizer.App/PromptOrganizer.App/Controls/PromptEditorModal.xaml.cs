using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace PromptOrganizer.App.Controls;

public sealed partial class PromptEditorModal : UserControl
{
    public event EventHandler<PromptSavedEventArgs>? PromptSaved;

    public PromptEditorModal()
    {
        this.InitializeComponent();
    }

    public bool IsOpen { get; private set; }

    public void OpenModal()
    {
        Root.Visibility = Visibility.Visible;
        VisualStateManager.GoToState(this, "Open", true);
        IsOpen = true;
        TitleTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    }

    public void CloseModal()
    {
        VisualStateManager.GoToState(this, "Closed", true);
        Root.Visibility = Visibility.Collapsed;
        IsOpen = false;
        ClearForm();
    }

    private void ClearForm()
    {
        TitleTextBox.Text = string.Empty;
        CategoryTextBox.Text = string.Empty;
        TagsTextBox.Text = string.Empty;
        BodyTextBox.Text = string.Empty;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        CloseModal();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            return;
        }

        var args = new PromptSavedEventArgs
        {
            Title = TitleTextBox.Text.Trim(),
            Category = CategoryTextBox.Text.Trim(),
            Tags = TagsTextBox.Text.Trim(),
            Body = BodyTextBox.Text.Trim()
        };

        PromptSaved?.Invoke(this, args);
        CloseModal();
    }

    public string Title
    {
        get => TitleTextBox?.Text ?? string.Empty;
        set
        {
            if (TitleTextBox != null)
                TitleTextBox.Text = value;
        }
    }

    public string Category
    {
        get => CategoryTextBox?.Text ?? string.Empty;
        set
        {
            if (CategoryTextBox != null)
                CategoryTextBox.Text = value;
        }
    }

    public string Tags
    {
        get => TagsTextBox?.Text ?? string.Empty;
        set
        {
            if (TagsTextBox != null)
                TagsTextBox.Text = value;
        }
    }

    public string Body
    {
        get => BodyTextBox?.Text ?? string.Empty;
        set
        {
            if (BodyTextBox != null)
                BodyTextBox.Text = value;
        }
    }
}

public class PromptSavedEventArgs : EventArgs
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
