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
    IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<LibraryEditMessage>, IRecipient<LibraryDeleteMessage>
{
    [ObservableProperty]
    public partial IEnumerable<LibraryListModel> Libraries { get; set; } = [];

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        Libraries = await libraryFacade.GetAsync();
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
        => await navigationService.GoToAsync("LibraryDetail",
            new Dictionary<string, object?> { ["Id"] = id });

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync("LibraryEdit");
    }

    public void Receive(LibraryEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(LibraryDeleteMessage message) => ForceDataRefreshOnNextAppearing();
}