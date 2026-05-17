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
    IAlertService alertService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)
{
    public Guid Id { get; set; }
    public bool IsExistingUser => Id != Guid.Empty;

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
        if (string.IsNullOrWhiteSpace(User.Username) || string.IsNullOrWhiteSpace(User.Email))
        {
            await alertService.DisplayAsync(
                "Missing",
                "Username or email."
                );
            return;
        }

        if (!User.Email.Contains('@'))
        {
            await alertService.DisplayAsync(
            "Wrong email",
            "Email missing '@'"
                );
            return;
        }
        await userFacade.SaveAsync(User);

        MessengerService.Send(new UserEditMessage { UserId = User.Id });

        navigationService.SendBackButtonPressed();
    }

    [RelayCommand]
    private void Cancel()
    {
        navigationService.SendBackButtonPressed();
    }
}