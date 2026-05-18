using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using System;
using System.Collections.Generic;
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
    : ViewModelBase(messengerService), IRecipient<VideoGameEditMessage>, IRecipient<VideoGameDeleteMessage>
{
    [ObservableProperty]
    public partial IEnumerable<VideoGameListModel> VideoGames { get; set; } = [];

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        VideoGames = await videoGameFacade.GetAsync();
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

        await alertService.DisplayAsync("Done", $"Game added to \"{library.Name}\".");
    }

    public void Receive(VideoGameEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(VideoGameDeleteMessage message) => ForceDataRefreshOnNextAppearing();
}