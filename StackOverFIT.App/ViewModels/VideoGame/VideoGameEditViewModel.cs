using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackOverFIT.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class VideoGameEditViewModel(
    IVideoGameFacade videoGameFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)

{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial VideoGameDetailModel VideoGame { get; set; } = VideoGameDetailModel.Empty;

    public List<int> AvailablePegiRatings { get; } = new List<int> { 3, 7, 12, 16, 18 };

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        VideoGame = await videoGameFacade.GetAsync(Id)
                    ?? VideoGameDetailModel.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await videoGameFacade.SaveAsync(VideoGame);

        MessengerService.Send(new VideoGameEditMessage { VideoGameId = VideoGame.Id });

        await navigationService.GoToAsync(NavigationService.VideoGameListRouteAbsolute);
    }
    [RelayCommand]
    private void Cancel() => navigationService.SendBackButtonPressed();
}