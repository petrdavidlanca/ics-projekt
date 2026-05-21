using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.App.ViewModels;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using Xunit;

namespace StackOverFIT.App.Tests;
public class VideoGameListViewModelTests
{
    private readonly IVideoGameFacade _gameFacade = Substitute.For<IVideoGameFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IAlertService _alert = Substitute.For<IAlertService>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IUserFacade _userFacade = Substitute.For<IUserFacade>();
    private readonly ILibraryFacade _libraryFacade = Substitute.For<ILibraryFacade>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();

    private VideoGameListViewModel CreateVM()
        => new(_gameFacade, _navigation, _alert, _currentUser, _userFacade, _libraryFacade, _messenger);

    private static VideoGameListModel Game(string name, int pegi)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            PegiRating = pegi
        };

    [Fact]
    public async Task LoadDataAsync_Loads_Games()
    {
        var vm = CreateVM();
        var list = new List<VideoGameListModel> { Game("A", 12), Game("B", 18) };

        _gameFacade.GetFilteredGamesAsync("", "name", true, null).Returns(list);

        await vm.OnAppearingAsync();

        Assert.Equal(list, vm.VideoGames);
    }

    [Fact]
    public async Task GoToDetail_Navigates_With_Id()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();

        await vm.GoToDetailCommand.ExecuteAsync(id);

        await _navigation.Received().GoToAsync(
            NavigationService.VideoGameDetailRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == id)
        );
    }

    [Fact]
    public async Task GoToCreate_Navigates()
    {
        var vm = CreateVM();

        await vm.GoToCreateCommand.ExecuteAsync(null);

        await _navigation.Received().GoToAsync(
            NavigationService.VideoGameEditRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == Guid.Empty)
        );
    }

    [Fact]
    public async Task AddToLibrary_Shows_Error_When_No_Libraries()
    {
        var vm = CreateVM();
        var user = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Username = "u",
            Email = "u@e.com",
            GameLibrary = new ObservableCollection<LibraryListModel>()
        };

        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);

        await vm.AddToLibraryCommand.ExecuteAsync(Guid.NewGuid());

        await _alert.Received().DisplayAsync("No libraries", "Create a library first in the Library tab.");
    }

    [Fact]
    public async Task ToggleSortMenu_Works()
    {
        var vm = CreateVM();

        vm.ToggleSortMenuCommand.Execute(null);
        Assert.True(vm.IsSortMenuVisible);
        Assert.False(vm.IsFilterMenuVisible);

        vm.ToggleSortMenuCommand.Execute(null);
        Assert.False(vm.IsSortMenuVisible);
    }

    [Fact]
    public async Task ToggleFilterMenu_Works()
    {
        var vm = CreateVM();

        vm.ToggleFilterMenuCommand.Execute(null);
        Assert.True(vm.IsFilterMenuVisible);
        Assert.False(vm.IsSortMenuVisible);

        vm.ToggleFilterMenuCommand.Execute(null);
        Assert.False(vm.IsFilterMenuVisible);
    }

    [Fact]
    public void CloseMenus_Closes_Both()
    {
        var vm = CreateVM();
        vm.IsFilterMenuVisible = true;
        vm.IsSortMenuVisible = true;

        vm.CloseMenusCommand.Execute(null);

        Assert.False(vm.IsFilterMenuVisible);
        Assert.False(vm.IsSortMenuVisible);
    }

    [Fact]
    public async Task Receive_VideoGameEditMessage_Triggers_Refresh()
    {
        var vm = CreateVM();
        var list = new List<VideoGameListModel> { Game("A", 12) };

        _gameFacade.GetFilteredGamesAsync("", "name", true, null).Returns(list);

        await vm.OnAppearingAsync();

        vm.Receive(new VideoGameEditMessage { VideoGameId = Guid.NewGuid() });

        await vm.OnAppearingAsync();
        await vm.OnAppearingAsync();

        Assert.Equal(list, vm.VideoGames);
    }

    [Fact]
    public async Task Receive_ReloadDataMessage_Refreshes_Immediately()
    {
        var vm = CreateVM();
        var list = new List<VideoGameListModel> { Game("A", 12) };

        _gameFacade.GetFilteredGamesAsync("", "name", true, null).Returns(list);

        vm.Receive(new ReloadDataMessage());

        Assert.Equal(list, vm.VideoGames);
    }
}
