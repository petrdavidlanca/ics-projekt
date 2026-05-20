using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackOverFIT.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class LibraryDetailViewModel(
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IUserFacade userFacade,
    ICurrentUserService currentUserService,
    IAlertService alertService)
    : ViewModelBase(messengerService), IRecipient<LibraryEditMessage>
{
    public Guid Id { get; set; }

    [ObservableProperty]
    public partial LibraryDetailModel? Library { get; set; }

    [ObservableProperty]
    private IEnumerable<VideoGameListModel> displayedGames = [];

    // --- STATE VARIABLES ---
    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isFilterMenuVisible;

    [ObservableProperty]
    private bool showAdultGames = true;

    private string currentSortBy = "name";
    private bool currentAscending = true;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        if (Id == Guid.Empty)
        {
            var user = await userFacade.GetAsync(currentUserService.CurrentUserId);
            Id = user?.GameLibrary.FirstOrDefault()?.Id ?? Guid.Empty;
        }

        if (Id != Guid.Empty)
        {
            Library = await libraryFacade.GetAsync(Id);
            RefreshListAsync(); // Apply filters as soon as it loads!
        }
    }

    // --- FILTERING, SORTING, SEARCHING LOGIC ---
    private void RefreshListAsync()
    {
        if (Library?.VideoGames == null) return;

        var games = Library.VideoGames.AsEnumerable();

        // 1. Apply Search
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            games = games.Where(g => g.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Apply 18+ Filter
        if (!ShowAdultGames)
        {
            // Note: If you get a red underline on PegiRating here, it means it's missing from your VideoGameListModel! 
            // Just add `public int PegiRating { get; set; }` to that model.
            games = games.Where(g => g.PegiRating <= 16);
        }

        // 3. Apply Sorting
        games = currentSortBy switch
        {
            "name" => currentAscending ? games.OrderBy(g => g.Name) : games.OrderByDescending(g => g.Name),
            "rating" => currentAscending ? games.OrderBy(g => g.PegiRating) : games.OrderByDescending(g => g.PegiRating),
            _ => games.OrderBy(g => g.Name)
        };

        // 4. Update the UI
        DisplayedGames = games.ToList();
    }

    // --- COMMANDS ---
    partial void OnSearchTextChanged(string value) => RefreshListAsync();
    partial void OnShowAdultGamesChanged(bool value) => RefreshListAsync();

    [RelayCommand]
    private void Search() => RefreshListAsync();

    [RelayCommand]
    private async Task SortAsync()
    {
        string[] options = { "Name (A-Z)", "Name (Z-A)", "Rating (Low-High)", "Rating (High-Low)" };
        string? selected = await Shell.Current.DisplayActionSheet("Sort games by", "Cancel", null, options);

        if (selected is null || selected == "Cancel") return;

        switch (selected)
        {
            case "Name (A-Z)": currentSortBy = "name"; currentAscending = true; break;
            case "Name (Z-A)": currentSortBy = "name"; currentAscending = false; break;
            case "Rating (Low-High)": currentSortBy = "rating"; currentAscending = true; break;
            case "Rating (High-Low)": currentSortBy = "rating"; currentAscending = false; break;
        }

        RefreshListAsync();
    }

    [RelayCommand]
    private void ToggleFilterMenu() => IsFilterMenuVisible = !IsFilterMenuVisible;

    [RelayCommand]
    private void CloseFilterMenu() => IsFilterMenuVisible = false;

    // --- EXISTING COMMANDS ---
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
            await navigationService.GoToAsync(NavigationService.LibraryEditRouteRelative,
                new Dictionary<string, object?> { ["Id"] = Library.Id });
        }
    }

    [RelayCommand]
    private async Task RemoveVideoGameAsync(Guid videoGameId)
    {
        if (Library is null) return;

        await libraryFacade.RemoveVideoGameAsync(Library.Id, videoGameId);
        MessengerService.Send(new LibraryEditMessage { LibraryId = Library.Id });

        var game = Library.VideoGames.FirstOrDefault(g => g.Id == videoGameId);
        if (game is not null)
        {
            Library.VideoGames.Remove(game);
            Library.NumOfGames--;
            OnPropertyChanged(nameof(Library));

            // Re-apply filters so the removed game disappears from the screen!
            RefreshListAsync();
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