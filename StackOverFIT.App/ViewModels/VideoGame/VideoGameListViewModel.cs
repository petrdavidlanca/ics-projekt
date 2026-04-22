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
        => await navigationService.GoToAsync("VideoGameDetail",
            new Dictionary<string, object?> { ["Id"] = id });

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync("VideoGameEdit");
    }

    public void Receive(VideoGameEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(VideoGameDeleteMessage message) => ForceDataRefreshOnNextAppearing();
}