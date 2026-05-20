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
    : ViewModelBase(messengerService), IRecipient<LibraryEditMessage>, IRecipient<LibraryDeleteMessage>, IRecipient<ReloadDataMessage>
{
    [ObservableProperty]
    public partial IEnumerable<LibraryListModel> Libraries { get; set; } = [];

    private string currentSortBy = "name";
    private bool currentAscending = true;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isFilterMenuVisible;

    [ObservableProperty]
    private bool hideEmptyLibraries = false;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await RefreshListAsync();
    }

    private async Task RefreshListAsync()
    {
        var user = await userFacade.GetAsync(currentUserService.CurrentUserId);
        IEnumerable<LibraryListModel> userLibraries = user?.GameLibrary ?? [];

        if (HideEmptyLibraries)
        {
            userLibraries = userLibraries.Where(l => l.NumOfGames > 0).ToList();
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            userLibraries = userLibraries
                .Where(l => l.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        Libraries = currentSortBy switch
        {
            "name" when currentAscending => userLibraries.OrderBy(l => l.Name).ToList(),
            "name" when !currentAscending => userLibraries.OrderByDescending(l => l.Name).ToList(),
            "games" when currentAscending => userLibraries.OrderBy(l => l.NumOfGames).ToList(),
            "games" when !currentAscending => userLibraries.OrderByDescending(l => l.NumOfGames).ToList(),
            _ => userLibraries.ToList()
        };
    }

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
        string[] options = { "Name (A-Z)", "Name (Z-A)", "Most Games", "Least Games" };
        string? selected = await Shell.Current.DisplayActionSheet("Sort libraries by", "Cancel", null, options);

        if (selected is null || selected == "Cancel")
            return;

        switch (selected)
        {
            case "Name (A-Z)": currentSortBy = "name"; currentAscending = true; break;
            case "Name (Z-A)": currentSortBy = "name"; currentAscending = false; break;
            case "Most Games": currentSortBy = "games"; currentAscending = false; break;
            case "Least Games": currentSortBy = "games"; currentAscending = true; break;
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
    private async Task GoToDetailAsync(Guid id)
        => await navigationService.GoToAsync(NavigationService.LibraryDetailRouteRelative,
            new Dictionary<string, object?>
            {
                [nameof(LibraryDetailViewModel.Id)] = id
            });

    [RelayCommand]
    private async Task GoToCreateAsync()
        => await navigationService.GoToAsync(NavigationService.LibraryEditRouteRelative,
            new Dictionary<string, object?>
            {
                [nameof(LibraryEditViewModel.UserId)] = currentUserService.CurrentUserId
            });

    public void Receive(LibraryEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(LibraryDeleteMessage message) => ForceDataRefreshOnNextAppearing();
    public async void Receive(ReloadDataMessage message)
    {
        await LoadDataAsync();
    }
}