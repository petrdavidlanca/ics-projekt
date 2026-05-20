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
public partial class UserDetailViewModel(
    IUserFacade userFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService,
    ICurrentUserService currentUserService)
    : ViewModelBase(messengerService),
      IRecipient<UserEditMessage>,
      IRecipient<LibraryEditMessage>,
      IRecipient<LibraryDeleteMessage>,
      IRecipient<VideoGameEditMessage>,
      IRecipient<VideoGameDeleteMessage>
{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial UserDetailModel? User { get; set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        var userId = Id != Guid.Empty ? Id : currentUserService.CurrentUserId;
        User = await userFacade.GetAsync(userId);
    }

    [RelayCommand]
    private async Task GoToLibrariesAsync()
    {
        await navigationService.GoToAsync(NavigationService.LibraryListRouteAbsolute);
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (User?.Id is not null)
        {
            await navigationService.GoToAsync(NavigationService.UserEditRouteRelative,
                new Dictionary<string, object?> { ["Id"] = User.Id });
        }
    }

    [RelayCommand]
    private async Task LogOutAsync()
    {
        currentUserService.CurrentUserId = Guid.Empty;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current!.Windows[0].Page = new AppShell(navigationService);
        });

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoToLibraryDetailAsync(Guid libraryId)
    {
        await navigationService.GoToAsync(NavigationService.LibraryListRouteAbsolute + NavigationService.LibraryDetailRouteRelative,
            new Dictionary<string, object?> { ["Id"] = libraryId });
    }

    public void Receive(UserEditMessage message)
    {
        if (message.UserId == User?.Id)
        {
            ForceDataRefreshOnNextAppearing();
        }
    }

    public void Receive(LibraryEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(LibraryDeleteMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(VideoGameEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(VideoGameDeleteMessage message) => ForceDataRefreshOnNextAppearing();
}