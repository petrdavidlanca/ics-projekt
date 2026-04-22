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
public partial class LibraryEditViewModel(
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)
{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial LibraryDetailModel Library { get; set; } = LibraryDetailModel.Empty;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Library = await libraryFacade.GetAsync(Id)
                    ?? LibraryDetailModel.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await libraryFacade.SaveAsync(Library);

        MessengerService.Send(new LibraryEditMessage { LibraryId = Library.Id });

        navigationService.SendBackButtonPressed();
    }
}