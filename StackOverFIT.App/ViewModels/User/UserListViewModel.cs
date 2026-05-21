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

public partial class UserListViewModel(
    IUserFacade userFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService,
    ICurrentUserService currentUserService)
    : ViewModelBase(messengerService), IRecipient<UserEditMessage>, IRecipient<UserDeleteMessage>, IRecipient<ReloadDataMessage>
{
    [ObservableProperty]
    public partial IEnumerable<UserListModel> Users { get; set; } = [];

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        Users = await userFacade.GetAsync();
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        currentUserService.CurrentUserId = id;
        await navigationService.GoToAsync(NavigationService.UserDetailRouteRelative,
            new Dictionary<string, object?> { [nameof(UserEditViewModel.Id)] = id });
    }

    [RelayCommand]
    private async Task GoToEditAsync(Guid id)
    {
        await navigationService.GoToAsync(NavigationService.UserEditRouteRelative,
            new Dictionary<string, object?> { [nameof(UserEditViewModel.Id)] = id });
    }

    [RelayCommand]
    private async Task DeleteAsync(Guid id)
    {
        try
        {
            await userFacade.DeleteAsync(id);
            MessengerService.Send(new UserDeleteMessage());
            await LoadDataAsync();
        }
        catch (InvalidOperationException)
        {
            await alertService.DisplayAsync("Chyba", "Uživatele se nepodařilo smazat.");
        }
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync(NavigationService.UserEditRouteRelative);
    }

    public void Receive(UserEditMessage message)
    {
        ForceDataRefreshOnNextAppearing();
    }

    public void Receive(UserDeleteMessage message)
    {
        ForceDataRefreshOnNextAppearing();
    }
    public async void Receive(ReloadDataMessage message)
    {
        await LoadDataAsync();
    }
}