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
[QueryProperty(nameof(UserId), nameof(UserId))]
public partial class LibraryEditViewModel(
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [ObservableProperty]
    public partial LibraryDetailModel Library { get; set; } = LibraryDetailModel.Empty;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        if (Id != Guid.Empty)
        {
            var loaded = await libraryFacade.GetAsync(Id) ?? LibraryDetailModel.Empty;
            Library = loaded;
        }
        else
        {
            Library = LibraryDetailModel.Empty;
            Library.UserId = UserId;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        Library.VideoGames.Clear();

        await libraryFacade.SaveAsync(Library);

        MessengerService.Send(new LibraryEditMessage { LibraryId = Library.Id });

        navigationService.SendBackButtonPressed();
    }

    [RelayCommand]
    private void Cancel() => navigationService.SendBackButtonPressed();
}