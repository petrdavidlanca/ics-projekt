using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.App.ViewModels;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StackOverFIT.App.Tests;

public class LibraryDetailViewModelTests
{
    private readonly ILibraryFacade _libraryFacade = Substitute.For<ILibraryFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();
    private readonly IUserFacade _userFacade = Substitute.For<IUserFacade>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IAlertService _alert = Substitute.For<IAlertService>();

    private LibraryDetailViewModel CreateVM()
        => new(_libraryFacade, _navigation, _messenger, _userFacade, _currentUser, _alert);

    private static LibraryDetailModel CreateLibrary(params VideoGameListModel[] games)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Library",
            VideoGames = new ObservableCollection<VideoGameListModel>(games),
            NumOfGames = games.Length
        };

    [Fact]
    public async Task LoadDataAsync_Loads_Library_When_Id_Is_Set()
    {
        var vm = CreateVM();
        var lib = CreateLibrary();
        vm.Id = lib.Id;

        _libraryFacade.GetAsync(lib.Id).Returns(lib);

        await vm.OnAppearingAsync();

        Assert.Equal(lib, vm.Library);
    }

    [Fact]
    public async Task LoadDataAsync_Loads_User_Default_Library_When_Id_Empty()
    {
        var vm = CreateVM();
        var lib = CreateLibrary();

        var user = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Username = "test",
            Email = "test@example.com",
            GameLibrary = new ObservableCollection<LibraryListModel>
            {
                new() { Id = lib.Id, Name = lib.Name }
            }
        };

        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);
        _libraryFacade.GetAsync(lib.Id).Returns(lib);

        await vm.OnAppearingAsync();

        Assert.Equal(lib.Id, vm.Id);
        Assert.Equal(lib, vm.Library);
    }

    [Fact]
    public async Task SearchText_Filters_Games()
    {
        var vm = CreateVM();
        var lib = CreateLibrary(
            new VideoGameListModel { Id = Guid.NewGuid(), Name = "Witcher", PegiRating = 18 },
            new VideoGameListModel { Id = Guid.NewGuid(), Name = "Minecraft", PegiRating = 7 }
        );

        vm.Id = lib.Id;
        _libraryFacade.GetAsync(lib.Id).Returns(lib);

        await vm.OnAppearingAsync();

        vm.SearchText = "mine";

        Assert.Single(vm.DisplayedGames);
        Assert.Equal("Minecraft", vm.DisplayedGames.First().Name);
    }

    [Fact]
    public async Task ShowAdultGames_Filters_Out_18Plus()
    {
        var vm = CreateVM();
        var lib = CreateLibrary(
            new VideoGameListModel { Id = Guid.NewGuid(), Name = "Cyberpunk", PegiRating = 18 },
            new VideoGameListModel { Id = Guid.NewGuid(), Name = "Mario", PegiRating = 3 }
        );

        vm.Id = lib.Id;
        _libraryFacade.GetAsync(lib.Id).Returns(lib);

        await vm.OnAppearingAsync();

        vm.ShowAdultGames = false;

        Assert.Single(vm.DisplayedGames);
        Assert.Equal("Mario", vm.DisplayedGames.First().Name);
    }

    [Fact]
    public async Task Sorting_By_Name_Works()
    {
        var vm = CreateVM();
        var lib = CreateLibrary(
            new VideoGameListModel { Id = Guid.NewGuid(), Name = "Zelda", PegiRating = 12 },
            new VideoGameListModel { Id = Guid.NewGuid(), Name = "Asteroids", PegiRating = 3 }
        );

        vm.Id = lib.Id;
        _libraryFacade.GetAsync(lib.Id).Returns(lib);

        await vm.OnAppearingAsync();

        Assert.Equal(new[] { "Asteroids", "Zelda" }, vm.DisplayedGames.Select(g => g.Name));
    }

    [Fact]
    public void ToggleFilterMenu_Toggles_State()
    {
        var vm = CreateVM();

        Assert.False(vm.IsFilterMenuVisible);

        vm.ToggleFilterMenuCommand.Execute(null);
        Assert.True(vm.IsFilterMenuVisible);

        vm.ToggleFilterMenuCommand.Execute(null);
        Assert.False(vm.IsFilterMenuVisible);
    }

    [Fact]
    public void CloseFilterMenu_Closes_Menu()
    {
        var vm = CreateVM();
        vm.IsFilterMenuVisible = true;

        vm.CloseFilterMenuCommand.Execute(null);

        Assert.False(vm.IsFilterMenuVisible);
    }

    [Fact]
    public async Task Delete_Deletes_Library_And_Navigates_Back()
    {
        var vm = CreateVM();
        var lib = CreateLibrary();
        vm.Library = lib;

        await vm.DeleteCommand.ExecuteAsync(null);

        await _libraryFacade.Received().DeleteAsync(lib.Id);
        _messenger.Received().Send(Arg.Any<LibraryDeleteMessage>());
        _navigation.Received().SendBackButtonPressed();
    }

    [Fact]
    public async Task Delete_Shows_Alert_On_Error()
    {
        var vm = CreateVM();
        var lib = CreateLibrary();
        vm.Library = lib;

        _libraryFacade.DeleteAsync(lib.Id).Throws(new InvalidOperationException());

        await vm.DeleteCommand.ExecuteAsync(null);

        await _alert.Received().DisplayAsync("Chyba", "Knihovnu se nepodařilo smazat.");
    }

    [Fact]
    public async Task RemoveVideoGame_Removes_Game_From_List()
    {
        var game = new VideoGameListModel { Id = Guid.NewGuid(), Name = "Test", PegiRating = 12 };
        var lib = CreateLibrary(game);

        var vm = CreateVM();
        vm.Library = lib;

        await vm.RemoveVideoGameCommand.ExecuteAsync(game.Id);

        Assert.Empty(vm.Library.VideoGames);
        Assert.Equal(0, vm.Library.NumOfGames);
    }
}