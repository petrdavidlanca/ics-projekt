using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using System;
using System.Threading.Tasks;

namespace StackOverFIT.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class UserEditViewModel(
    IUserFacade userFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)
{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial UserDetailModel User { get; set; } = UserDetailModel.Empty;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        User = await userFacade.GetAsync(Id)
                    ?? UserDetailModel.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await userFacade.SaveAsync(User);

        MessengerService.Send(new UserEditMessage { UserId = User.Id });

        navigationService.SendBackButtonPressed();
    }
}