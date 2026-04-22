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

[QueryProperty(nameof(Id), nameof(Id))]
public partial class VideoGameDetailViewModel(
    IVideoGameFacade videoGameFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService)
    : ViewModelBase(messengerService), IRecipient<VideoGameEditMessage>
{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial VideoGameDetailModel? VideoGame { get; set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        VideoGame = await videoGameFacade.GetAsync(Id);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (VideoGame is not null)
        {
            try
            {
                await videoGameFacade.DeleteAsync(VideoGame.Id);
                MessengerService.Send(new VideoGameDeleteMessage());
                navigationService.SendBackButtonPressed();
            }
            catch (InvalidOperationException)
            {
                await alertService.DisplayAsync("Chyba", "Hru se nepodařilo smazat.");
            }
        }
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (VideoGame?.Id is not null)
        {
            await navigationService.GoToAsync("VideoGameEdit",
                new Dictionary<string, object?> { ["Id"] = VideoGame.Id });
        }
    }

    public void Receive(VideoGameEditMessage message)
    {
        if (message.VideoGameId == VideoGame?.Id)
        {
            ForceDataRefreshOnNextAppearing();
        }
    }
}