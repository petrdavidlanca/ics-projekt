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

public partial class VideoGameListViewModel(
    IVideoGameFacade videoGameFacade,
    INavigationService navigationService,
    IAlertService alertService,
    ICurrentUserService currentUserService,
    IUserFacade userFacade,
    ILibraryFacade libraryFacade,
    IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<VideoGameEditMessage>, IRecipient<VideoGameDeleteMessage>, IRecipient<ReloadDataMessage>
{
    [ObservableProperty]
    public partial IEnumerable<VideoGameListModel> VideoGames { get; set; } = [];

    [ObservableProperty]
    private bool isSortMenuVisible;

    [ObservableProperty]
    private bool isFilterMenuVisible;

    [ObservableProperty]
    private bool showAdultGames = true;

    private string currentSortBy = "name";
    private bool currentAscending = true;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await RefreshListAsync();
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
        => await navigationService.GoToAsync(NavigationService.VideoGameDetailRouteRelative,
            new Dictionary<string, object?> { ["Id"] = id });

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync(NavigationService.VideoGameEditRouteRelative,
            new Dictionary<string, object?> { ["Id"] = Guid.Empty });
    }

    [RelayCommand]
    private async Task AddToLibraryAsync(Guid gameId)
    {
        var user = await userFacade.GetAsync(currentUserService.CurrentUserId);
        var libraries = user?.GameLibrary.ToList() ?? [];

        if (libraries.Count == 0)
        {
            await alertService.DisplayAsync("No libraries", "Create a library first in the Library tab.");
            return;
        }

        var libraryNames = libraries.Select(l => l.Name).ToArray();
        var selected = await Shell.Current.DisplayActionSheet(
            "Add to library", "Cancel", null, libraryNames);

        if (selected is null || selected == "Cancel")
            return;

        var library = libraries.First(l => l.Name == selected);
        await libraryFacade.AddVideoGameAsync(library.Id, gameId);

        MessengerService.Send(new LibraryEditMessage { LibraryId = library.Id });

        await alertService.DisplayAsync("Done", $"Game added to \"{library.Name}\".");
    }

    [RelayCommand]
    private void ToggleSortMenu()
    {
        IsSortMenuVisible = !IsSortMenuVisible;
        IsFilterMenuVisible = false;
    }

    [RelayCommand]
    private void ToggleFilterMenu()
    {
        IsFilterMenuVisible = !IsFilterMenuVisible;
        IsSortMenuVisible = false;
    }

    [RelayCommand]
    private void CloseMenus()
    {
        IsFilterMenuVisible = false;
        IsSortMenuVisible = false;
    }

    partial void OnShowAdultGamesChanged(bool value)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshListAsync());
    }

    [RelayCommand]
    private async Task SortAsync()
    {
        string[] options = { "Name (A-Z)", "Name (Z-A)", "PEGI Rating (Low-High)", "PEGI Rating (High-Low)", "Newest first", "Oldest first" };
        string? selected = await Shell.Current.DisplayActionSheet("Sort games by", "Cancel", null, options);

        if (selected is null || selected == "Cancel")
            return;

        switch (selected)
        {
            case "Name (A-Z)": currentSortBy = "name"; currentAscending = true; break;
            case "Name (Z-A)": currentSortBy = "name"; currentAscending = false; break;
            case "PEGI Rating (Low-High)": currentSortBy = "rating"; currentAscending = true; break;
            case "PEGI Rating (High-Low)": currentSortBy = "rating"; currentAscending = false; break;
            case "Newest first": currentSortBy = "date"; currentAscending = false; break;
            case "Oldest first": currentSortBy = "date"; currentAscending = true; break;
        }

        await RefreshListAsync();
    }

    private async Task RefreshListAsync()
    {
        int? maxPegi = ShowAdultGames ? null : 16;

        VideoGames = await videoGameFacade.GetFilteredGamesAsync(SearchText, currentSortBy, currentAscending, maxPegi);
    }

    [ObservableProperty]
    private string searchText = string.Empty;

    partial void OnSearchTextChanged(string value)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshListAsync());
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await RefreshListAsync();
    }

    public void Receive(VideoGameEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(VideoGameDeleteMessage message) => ForceDataRefreshOnNextAppearing();
    public async void Receive(ReloadDataMessage message)
    {
        await LoadDataAsync();
    }
}