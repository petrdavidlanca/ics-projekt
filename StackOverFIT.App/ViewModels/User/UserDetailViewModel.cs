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
    IAlertService alertService)
    : ViewModelBase(messengerService), IRecipient<UserEditMessage>
{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial UserDetailModel? User { get; set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        User = await userFacade.GetAsync(Id);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (User is not null)
        {
            try
            {
                await userFacade.DeleteAsync(User.Id);
                MessengerService.Send(new UserDeleteMessage());
                navigationService.SendBackButtonPressed();
            }
            catch (InvalidOperationException)
            {
                await alertService.DisplayAsync("Chyba", "Uživatele se nepodařilo smazat. Pravděpodobně k němu existují navázaná data.");
            }
        }
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

    public void Receive(UserEditMessage message)
    {
        if (message.UserId == User?.Id)
        {
            ForceDataRefreshOnNextAppearing();
        }
    }
}