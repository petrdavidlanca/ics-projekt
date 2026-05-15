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

public partial class LibraryListViewModel(
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IUserFacade userFacade,
    IMessengerService messengerService,
    ICurrentUserService currentUserService)
    : ViewModelBase(messengerService), IRecipient<LibraryEditMessage>, IRecipient<LibraryDeleteMessage>
{
    [ObservableProperty]
    public partial IEnumerable<LibraryListModel> Libraries { get; set; } = [];

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        var user = await userFacade.GetAsync(currentUserService.CurrentUserId);
        Libraries = user?.GameLibrary ?? [];
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
        => await navigationService.GoToAsync(NavigationService.LibraryDetailRouteRelative,
            new Dictionary<string, object?>
            {
                [nameof(LibraryDetailViewModel.Id)] = id
            });

    [RelayCommand]
    private async Task GoToCreateAsync()
        => await navigationService.GoToAsync(NavigationService.LibraryEditRouteRelative,
            new Dictionary<string, object?>
            {
                // Předej UserId přímo – obejde problém s ICurrentUserService
                [nameof(LibraryEditViewModel.UserId)] = currentUserService.CurrentUserId
            });

    public void Receive(LibraryEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(LibraryDeleteMessage message) => ForceDataRefreshOnNextAppearing();
}