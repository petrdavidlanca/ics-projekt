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
using System.Threading.Tasks;
using Xunit;

namespace StackOverFIT.App.Tests;
public class UserListViewModelTests
{
    private readonly IUserFacade _userFacade = Substitute.For<IUserFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();
    private readonly IAlertService _alert = Substitute.For<IAlertService>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

    private UserListViewModel CreateVM()
        => new(_userFacade, _navigation, _messenger, _alert, _currentUser);

    private static UserListModel User(string name)
        => new()
        {
            Id = Guid.NewGuid(),
            Username = name,
            Email = $"{name}@example.com",
        };

    private static ObservableCollection<UserListModel> Users(params UserListModel[] users)
        => new(users);

    [Fact]
    public async Task LoadDataAsync_Loads_Users()
    {
        var vm = CreateVM();
        var list = Users(User("A"), User("B"));

        _userFacade.GetAsync().Returns(list);

        await vm.OnAppearingAsync();

        Assert.Equal(list, vm.Users);
    }

    [Fact]
    public async Task GoToDetail_Navigates_And_Sets_CurrentUserId()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();

        await vm.GoToDetailCommand.ExecuteAsync(id);

        Assert.Equal(id, _currentUser.CurrentUserId);
        await _navigation.Received().GoToAsync(
            NavigationService.UserDetailRouteRelative,
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
            NavigationService.UserEditRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == id)
        );
    }

    [Fact]
    public async Task GoToCreate_Navigates()
    {
        var vm = CreateVM();

        await vm.GoToCreateCommand.ExecuteAsync(null);

        await _navigation.Received().GoToAsync(NavigationService.UserEditRouteRelative);
    }

    [Fact]
    public async Task Delete_Deletes_User_And_Reloads()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        var list = Users(User("A"));

        _userFacade.GetAsync().Returns(list);

        await vm.DeleteCommand.ExecuteAsync(id);

        await _userFacade.Received().DeleteAsync(id);
        _messenger.Received().Send(Arg.Any<UserDeleteMessage>());
        await _userFacade.Received().GetAsync();
    }

    [Fact]
    public async Task Delete_Shows_Error_On_Exception()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();

        _userFacade.DeleteAsync(id).Throws(new InvalidOperationException());

        await vm.DeleteCommand.ExecuteAsync(id);

        await _alert.Received().DisplayAsync("Chyba", "Uživatele se nepodařilo smazat.");
    }

    [Fact]
    public async Task Receive_UserEditMessage_Triggers_Refresh()
    {
        var vm = CreateVM();
        var list = Users(User("A"));

        _userFacade.GetAsync().Returns(list);

        await vm.OnAppearingAsync();

        vm.Receive(new UserEditMessage { UserId = Guid.NewGuid() });

        await vm.OnAppearingAsync();
        await vm.OnAppearingAsync();

        Assert.Equal(list, vm.Users);
    }

    [Fact]
    public async Task Receive_UserDeleteMessage_Triggers_Refresh()
    {
        var vm = CreateVM();
        var list = Users(User("A"));

        _userFacade.GetAsync().Returns(list);

        await vm.OnAppearingAsync();

        vm.Receive(new UserDeleteMessage());

        await vm.OnAppearingAsync();
        await vm.OnAppearingAsync();

        Assert.Equal(list, vm.Users);
    }

    [Fact]
    public async Task Receive_ReloadDataMessage_Refreshes_Immediately()
    {
        var vm = CreateVM();
        var list = Users(User("A"));

        _userFacade.GetAsync().Returns(list);

        vm.Receive(new ReloadDataMessage());

        Assert.Equal(list, vm.Users);
    }
}
