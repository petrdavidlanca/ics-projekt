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
public partial class LibraryDetailViewModel(
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService)
    : ViewModelBase(messengerService), IRecipient<LibraryEditMessage>
{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial LibraryDetailModel? Library { get; set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        Library = await libraryFacade.GetAsync(Id);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Library is not null)
        {
            try
            {
                await libraryFacade.DeleteAsync(Library.Id);
                MessengerService.Send(new LibraryDeleteMessage());
                navigationService.SendBackButtonPressed();
            }
            catch (InvalidOperationException)
            {
                await alertService.DisplayAsync("Chyba", "Knihovnu se nepodařilo smazat.");
            }
        }
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (Library?.Id is not null)
        {
            await navigationService.GoToAsync("LibraryEdit",
                new Dictionary<string, object?> { ["Id"] = Library.Id });
        }
    }

    public void Receive(LibraryEditMessage message)
    {
        if (message.LibraryId == Library?.Id)
        {
            ForceDataRefreshOnNextAppearing();
        }
    }
}