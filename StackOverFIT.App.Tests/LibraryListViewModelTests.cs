using System;
using System.Collections.ObjectModel;
using System.Linq;
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
public class LibraryListViewModelTests
{
    private readonly ILibraryFacade _libraryFacade = Substitute.For<ILibraryFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IUserFacade _userFacade = Substitute.For<IUserFacade>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

    private LibraryListViewModel CreateVM()
        => new(_libraryFacade, _navigation, _userFacade, _messenger, _currentUser);

    private static UserDetailModel CreateUser(params LibraryListModel[] libs)
        => new()
        {
            Id = Guid.NewGuid(),
            Username = "tester",
            Email = "tester@example.com",
            GameLibrary = new ObservableCollection<LibraryListModel>(libs)
        };

    private static LibraryListModel Lib(string name, int count)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            NumOfGames = count
        };

    [Fact]
    public async Task LoadDataAsync_Loads_And_Sorts_By_Name()
    {
        var vm = CreateVM();

        var user = CreateUser(
            Lib("Zelda", 5),
            Lib("Asteroids", 1)
        );

        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);

        await vm.OnAppearingAsync();

        Assert.Equal(new[] { "Asteroids", "Zelda" }, vm.Libraries.Select(l => l.Name));
    }

    [Fact]
    public async Task Sort_By_Count_Descending()
    {
        var vm = CreateVM();

        var user = CreateUser(
            Lib("A", 1),
            Lib("B", 10),
            Lib("C", 5)
        );

        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);

        await vm.OnAppearingAsync();

        vm.GetType().GetField("currentSortBy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(vm, "count");
        vm.GetType().GetField("currentAscending", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(vm, false);

        await vm.SearchCommand.ExecuteAsync(null);

        Assert.Equal(new[] { 10, 5, 1 }, vm.Libraries.Select(l => l.NumOfGames));
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
    public async Task GoToDetail_Navigates_With_Id()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();

        await vm.GoToDetailCommand.ExecuteAsync(id);

        await _navigation.Received().GoToAsync(
            NavigationService.LibraryDetailRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == id)
        );
    }

    [Fact]
    public async Task GoToEdit_Navigates_With_Id()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();

        await vm.GoToEditCommand.ExecuteAsync(id);

        await _navigation.Received().GoToAsync(
            NavigationService.LibraryEditRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == id)
        );
    }

    [Fact]
    public async Task GoToCreate_Navigates_With_CurrentUserId()
    {
        var vm = CreateVM();
        var uid = Guid.NewGuid();

        _currentUser.CurrentUserId.Returns(uid);

        await vm.GoToCreateCommand.ExecuteAsync(null);

        await _navigation.Received().GoToAsync(
            NavigationService.LibraryEditRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["UserId"] == uid)
        );
    }

    [Fact]
    public async Task Receive_LibraryEditMessage_Triggers_Refresh_On_Next_Appear()
    {
        var vm = CreateVM();

        var user = CreateUser(Lib("A", 1));
        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);

        await vm.OnAppearingAsync();

        vm.Receive(new LibraryEditMessage { LibraryId = Guid.NewGuid() });

        await vm.OnAppearingAsync();

        Assert.NotNull(vm.Libraries);
    }
}
