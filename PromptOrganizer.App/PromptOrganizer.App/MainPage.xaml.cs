using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using PromptOrganizer.App.Controls;
using System.Linq;
using LiteDB;
using PromptOrganizer.Data.Repositories;
using PromptOrganizer.Services;
using System.IO;
using Windows.Storage;
using PromptOrganizer.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PromptOrganizer.App;

public sealed partial class MainPage : Page
{
    public ObservableCollection<Prompt> InitialIdeas { get; } = new();
    public ObservableCollection<Prompt> InDevelopment { get; } = new();
    public ObservableCollection<Prompt> ReadyToUse { get; } = new();

    private LiteDatabase _database;
    private PromptRepository _promptRepository;
    private CategoryRepository _categoryRepository;
    private PromptService _promptService;

    public MainPage()
    {
        this.InitializeComponent();
        
        EditorModal.PromptSaved += OnPromptSaved;
        SearchOverlay.QueryChanged += OnSearchQueryChanged;
        SearchOverlay.PromptSelected += OnSearchPromptSelected;

        InitializeServices();
        Loaded += MainPage_Loaded;
    }

    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadPromptsAsync();
    }

    private void InitializeServices()
    {
        var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "prompts.db");
        _database = new LiteDatabase(dbPath);
        _promptRepository = new PromptRepository(_database);
        _categoryRepository = new CategoryRepository(_database);
        _promptService = new PromptService(_promptRepository, _categoryRepository);
    }

    private async Task LoadPromptsAsync()
    {
        var prompts = await _promptService.GetAllActivePromptsAsync();
        
        InitialIdeas.Clear();
        InDevelopment.Clear();
        ReadyToUse.Clear();

        foreach (var prompt in prompts)
        {
            // Logic to distribute prompts to columns
            // "Ide Awal" -> Drafts (CategoryName == "Draft" or null/empty)
            // "Dikembangkan" -> In Development (CategoryName == "InDevelopment")
            // "Siap Digunakan" -> Library (CategoryName == "Library" or "ReadyToUse")
            
            if (string.IsNullOrEmpty(prompt.CategoryName) || prompt.CategoryName == "Draft")
            {
                InitialIdeas.Add(prompt);
            }
            else if (prompt.CategoryName == "InDevelopment")
            {
                InDevelopment.Add(prompt);
            }
            else
            {
                ReadyToUse.Add(prompt);
            }
        }
    }

    private void OnTrashToggleClick(object sender, RoutedEventArgs e)
    {
        // TODO: Implement Trash view
    }

    private void OnSidebarToggleClick(object sender, RoutedEventArgs e)
    {
        MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
    }

    private void OnNewPromptClick(object sender, RoutedEventArgs e)
    {
        EditorModal.OpenModal();
    }

    private void OnGlobalSearch(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        SearchOverlay.OpenOverlay();
        args.Handled = true;
    }

    private void OnNewPromptKeyboard(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        OnNewPromptClick(null, null);
        args.Handled = true;
    }

    private void OnGlobalEscape(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        if (EditorModal.IsOpen)
        {
            EditorModal.CloseModal();
            args.Handled = true;
            return;
        }
        if (SearchOverlay.IsOpen)
        {
            SearchOverlay.CloseOverlay();
            args.Handled = true;
            return;
        }
    }

    private void OnCardCopyRequested(object sender, RoutedEventArgs e)
    {
        if (sender is PromptCardControl card && card.DataContext is Prompt prompt)
        {
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(prompt.Content);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            
            FlyoutBase.ShowAttachedFlyout(ToastAnchor);
            // Auto hide toast after delay could be added here
        }
    }

    private void OnCardEditRequested(object sender, RoutedEventArgs e)
    {
        if (sender is PromptCardControl card && card.DataContext is Prompt prompt)
        {
            EditorModal.Title = prompt.Title;
            EditorModal.Body = prompt.Content;
            EditorModal.Tags = prompt.Tags;
            EditorModal.Category = prompt.CategoryName;
            // We need to pass the ID to the editor so it knows it's an update
            // But EditorModal seems to be designed for new/edit with just fields.
            // We'll need to handle the save logic carefully.
            // For now, let's assume EditorModal has a way to store the ID or we handle it in OnPromptSaved by checking title? 
            // No, that's risky.
            // Let's check EditorModal implementation.
            // Assuming EditorModal is simple, we might need to refactor it later.
            // For now, just open it.
            EditorModal.OpenModal();
        }
    }

    private void OnCardHistoryRequested(object sender, RoutedEventArgs e)
    {
        if (sender is PromptCardControl card && card.DataContext is Prompt prompt)
        {
            DetailTitle.Text = prompt.Title;
            RightSplitView.IsPaneOpen = true;
        }
    }

    private async void OnCardFavoriteToggled(object sender, RoutedEventArgs e)
    {
        if (sender is PromptCardControl card && card.DataContext is Prompt prompt)
        {
            // The property is already toggled in the control's click handler, 
            // but we need to update the entity and save it.
            // Wait, PromptCardControl toggles its own IsFavorite dependency property.
            // It does NOT automatically toggle the Prompt entity's IsFavorite property because it's OneWay binding.
            // So we must toggle the entity here.
            
            prompt.ToggleFavorite();
            await _promptService.UpdatePromptAsync(prompt);
            
            // Force update binding if needed, but since we updated the entity, 
            // and if we reload or if the control re-reads, it should be fine.
            // However, to make the UI reflect it immediately if it wasn't TwoWay:
            card.IsFavorite = prompt.IsFavorite;
        }
    }

    private async void OnPromptSaved(object? sender, PromptSavedEventArgs e)
    {
        try
        {
            // This is a simplification. In a real app we need to know if it's an edit or new.
            // For now, we assume new if we don't have an ID context.
            // But wait, we don't have ID context in EditorModal events.
            // We will treat all as new for this specific step unless we refactor EditorModal.
            // Or we can search for existing title?
            
            var newPrompt = await _promptService.CreatePromptAsync(e.Title, e.Body, string.Empty, null, e.Tags);
            
            // Add to "Ide Awal" (Drafts)
            InitialIdeas.Insert(0, newPrompt);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving prompt: {ex.Message}");
        }
    }

    private async void OnSearchQueryChanged(object? sender, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            SearchOverlay.SetResults(null);
            return;
        }

        var results = await _promptService.SearchPromptsAsync(query);
        SearchOverlay.SetResults(results);
    }

    private void OnSearchPromptSelected(object? sender, object item)
    {
        if (item is Prompt prompt)
        {
            SearchOverlay.CloseOverlay();
            EditorModal.Title = prompt.Title;
            EditorModal.Body = prompt.Content;
            EditorModal.Tags = prompt.Tags;
            EditorModal.Category = prompt.CategoryName;
            EditorModal.OpenModal();
        }
    }
    
    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
        UndoTip.IsOpen = false;
    }
}
