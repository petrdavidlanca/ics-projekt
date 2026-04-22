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
    IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<UserEditMessage>, IRecipient<UserDeleteMessage>
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
        => await navigationService.GoToAsync(NavigationService.UserDetailRouteRelative,
            new Dictionary<string, object?>
            {
                [nameof(UserDetailViewModel.Id)] = id
            }
        );

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
}