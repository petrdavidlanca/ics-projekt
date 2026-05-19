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

    [ObservableProperty]
    private string pageTitle = "Create a new game";

    partial void OnIdChanged(Guid value)
    {
        if (value != Guid.Empty)
        {
            PageTitle = "Edit game";
            MainThread.BeginInvokeOnMainThread(async () => await LoadDataAsync());
        }
        else
        {
            PageTitle = "Create a new game";
            VideoGame = VideoGameDetailModel.Empty;
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

        await videoGameFacade.SaveAsync(VideoGame);
        MessengerService.Send(new VideoGameEditMessage { VideoGameId = VideoGame.Id });

        VideoGame = VideoGameDetailModel.Empty;
        Id = Guid.Empty;

        await navigationService.GoToAsync(NavigationService.VideoGameListRouteAbsolute);
    }

    [RelayCommand]
    private void Cancel()
    {
        VideoGame = VideoGameDetailModel.Empty;
        Id = Guid.Empty;

        navigationService.SendBackButtonPressed();
    }
}