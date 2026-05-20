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
    : ViewModelBase(messengerService), IRecipient<LibraryEditMessage>, IRecipient<LibraryDeleteMessage>
{
    [ObservableProperty]
    public partial IEnumerable<LibraryListModel> Libraries { get; set; } = [];

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        // 1. Fetch ONLY the current user's libraries
        var user = await userFacade.GetAsync(currentUserService.CurrentUserId);
        var userLibraries = user?.GameLibrary ?? [];

        // 2. Filter that specific list
        if (HideEmptyLibraries)
        {
            Libraries = userLibraries.Where(l => l.NumOfGames > 0).ToList();
        }
        else
        {
            Libraries = userLibraries;
        }
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

    // --- MENU STATE ---
    [ObservableProperty]
    private bool isFilterMenuVisible;

    [ObservableProperty]
    private bool hideEmptyLibraries = false;

    [RelayCommand]
    private void ToggleFilterMenu()
    {
        IsFilterMenuVisible = !IsFilterMenuVisible;
    }

    // New command strictly for the invisible backdrop
    [RelayCommand]
    private void CloseFilterMenu()
    {
        IsFilterMenuVisible = false;
    }

    partial void OnHideEmptyLibrariesChanged(bool value)
    {
        MainThread.BeginInvokeOnMainThread(async () => await LoadDataAsync());
    }

    public void Receive(LibraryEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(LibraryDeleteMessage message) => ForceDataRefreshOnNextAppearing();
}