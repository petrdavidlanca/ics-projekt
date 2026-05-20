using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackOverFIT.App.ViewModels;

public partial class LibraryListViewModel(
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IUserFacade userFacade,
    IMessengerService messengerService,
    ICurrentUserService currentUserService)
    : ViewModelBase(messengerService),
      IRecipient<LibraryEditMessage>,
      IRecipient<LibraryDeleteMessage>,
      IRecipient<ReloadDataMessage> // <-- Your new message receiver!
{
    [ObservableProperty]
    public partial IEnumerable<LibraryListModel> Libraries { get; set; } = [];

    // --- STATE VARIABLES ---
    private string currentSortBy = "name";
    private bool currentAscending = true;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isFilterMenuVisible;

    [ObservableProperty]
    private bool hideEmptyLibraries = false;

    // --- DATA LOADING & FILTERING ---
    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await RefreshListAsync();
    }

    // Consolidated method to handle searching, filtering, and sorting cleanly
    private async Task RefreshListAsync()
    {
        var user = await userFacade.GetAsync(currentUserService.CurrentUserId);
        IEnumerable<LibraryListModel> processedList = user?.GameLibrary ?? [];

        // 1. Apply Search
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            processedList = processedList.Where(l =>
                l.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Apply Empty Filter
        if (HideEmptyLibraries)
        {
            processedList = processedList.Where(l => l.NumOfGames > 0);
        }

        // 3. Apply Sort
        processedList = currentSortBy switch
        {
            "name" => currentAscending
                ? processedList.OrderBy(l => l.Name)
                : processedList.OrderByDescending(l => l.Name),

            "count" => currentAscending
                ? processedList.OrderBy(l => l.NumOfGames)
                : processedList.OrderByDescending(l => l.NumOfGames),

            _ => processedList.OrderBy(l => l.Name)
        };

        // 4. Update UI
        Libraries = processedList.ToList();
    }

    // --- COMMANDS ---
    partial void OnSearchTextChanged(string value)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshListAsync());
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await RefreshListAsync();
    }

    [RelayCommand]
    private async Task SortAsync()
    {
        string[] options = { "Name (A-Z)", "Name (Z-A)", "Games count (Low-High)", "Games count (High-Low)" };
        string? selected = await Shell.Current.DisplayActionSheet("Sort libraries by", "Cancel", null, options);

        if (selected is null || selected == "Cancel")
            return;

        switch (selected)
        {
            case "Name (A-Z)": currentSortBy = "name"; currentAscending = true; break;
            case "Name (Z-A)": currentSortBy = "name"; currentAscending = false; break;
            case "Games count (Low-High)": currentSortBy = "count"; currentAscending = true; break;
            case "Games count (High-Low)": currentSortBy = "count"; currentAscending = false; break;
        }

        await RefreshListAsync();
    }

    [RelayCommand]
    private void ToggleFilterMenu()
    {
        IsFilterMenuVisible = !IsFilterMenuVisible;
    }

    [RelayCommand]
    private void CloseFilterMenu()
    {
        IsFilterMenuVisible = false;
    }

    partial void OnHideEmptyLibrariesChanged(bool value)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshListAsync());
    }

    // --- NAVIGATION ---
    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await navigationService.GoToAsync(NavigationService.LibraryDetailRouteRelative,
            new Dictionary<string, object?>
            {
                [nameof(LibraryDetailViewModel.Id)] = id
            });
    }

    [RelayCommand]
    private async Task GoToEditAsync(Guid id)
    {
        await navigationService.GoToAsync(NavigationService.LibraryEditRouteRelative,
            new Dictionary<string, object?>
            {
                [nameof(LibraryDetailViewModel.Id)] = id
            });
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync(NavigationService.LibraryEditRouteRelative,
            new Dictionary<string, object?>
            {
                [nameof(LibraryEditViewModel.UserId)] = currentUserService.CurrentUserId
            });
    }

    // --- MESSAGES ---
    public void Receive(LibraryEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(LibraryDeleteMessage message) => ForceDataRefreshOnNextAppearing();

    public void Receive(ReloadDataMessage message)
    {
        // Safely ask the main thread to refresh the list when the message arrives
        MainThread.BeginInvokeOnMainThread(async () => await RefreshListAsync());
    }
}