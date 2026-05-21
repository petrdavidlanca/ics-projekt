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
    IMessengerService messengerService,
    IAlertService alertService,
    IUserFacade userFacade,
    ICurrentUserService currentUserService)
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
        if (!string.IsNullOrEmpty(Library.Name))
        {
            Library.Name = Library.Name.Trim();
        }

        if (string.IsNullOrWhiteSpace(Library.Name))
        {
            await alertService.DisplayAsync("Validation Error", "Please enter a name for the library.");
            return;
        }

        var user = await userFacade.GetAsync(currentUserService.CurrentUserId);
        var existingLibraries = user?.GameLibrary ?? [];

        bool isDuplicate = existingLibraries.Any(l =>
            l.Name.Equals(Library.Name, StringComparison.OrdinalIgnoreCase) &&
            l.Id != Library.Id);

        if (isDuplicate)
        {
            await alertService.DisplayAsync("Validation Error", $"You already have a library named '{Library.Name}'.");
            return;
        }

        if (Id != Guid.Empty)
        {
            Library.Id = Id;
        }
        Library.VideoGames.Clear();
        await libraryFacade.SaveAsync(Library);

        MessengerService.Send(new LibraryEditMessage { LibraryId = Library.Id });

        Library = LibraryDetailModel.Empty;
        Id = Guid.Empty;

        await navigationService.GoToAsync(NavigationService.LibraryListRouteAbsolute);
    }

    [RelayCommand]
    private void Cancel() => navigationService.SendBackButtonPressed();
}