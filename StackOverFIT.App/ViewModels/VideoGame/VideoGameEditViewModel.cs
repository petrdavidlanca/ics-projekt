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
    IAlertService alertService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)

{
    [ObservableProperty]
    private Guid _id;

    partial void OnIdChanged(Guid value)
    {
        if (value != Guid.Empty)
        {
            MainThread.BeginInvokeOnMainThread(async () => await LoadDataAsync());
        }
    }

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
        if (Id != Guid.Empty)
        {
            VideoGame.Id = Id;
        }

        if (string.IsNullOrWhiteSpace(VideoGame.Name)
            || string.IsNullOrWhiteSpace(VideoGame.Description) 
            || VideoGame.PegiRating <= 0)
        {
            await alertService.DisplayAsync("Missing","Name, Description or Pegi rating");
            return;
        }
        await videoGameFacade.SaveAsync(VideoGame);

        MessengerService.Send(new VideoGameEditMessage { VideoGameId = VideoGame.Id });

        await navigationService.GoToAsync(NavigationService.VideoGameListRouteAbsolute);
    }
    [RelayCommand]
    private void Cancel() => navigationService.SendBackButtonPressed();
}