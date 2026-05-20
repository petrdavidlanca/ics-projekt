using System.Threading.Tasks;
using System;
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
public class UserDetailViewModelTests
{
    private readonly IUserFacade _userFacade = Substitute.For<IUserFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();
    private readonly IAlertService _alert = Substitute.For<IAlertService>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

    private UserDetailViewModel CreateVM()
        => new(_userFacade, _navigation, _messenger, _alert, _currentUser);

    private static UserDetailModel CreateUser(Guid id)
        => new()
        {
            Id = id,
            Username = "tester",
            Email = "tester@example.com",
            GameLibrary = new ObservableCollection<LibraryListModel>()
        };

    [Fact]
    public async Task LoadDataAsync_Loads_User_By_Id()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        vm.Id = id;

        var user = CreateUser(id);
        _userFacade.GetAsync(id).Returns(user);

        await vm.OnAppearingAsync();

        Assert.Equal(user, vm.User);
    }

    [Fact]
    public async Task LoadDataAsync_Loads_CurrentUser_When_Id_Empty()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        vm.Id = Guid.Empty;

        _currentUser.CurrentUserId.Returns(id);
        var user = CreateUser(id);
        _userFacade.GetAsync(id).Returns(user);

        await vm.OnAppearingAsync();

        Assert.Equal(user, vm.User);
    }

    [Fact]
    public async Task GoToLibraries_Navigates()
    {
        var vm = CreateVM();

        await vm.GoToLibrariesCommand.ExecuteAsync(null);

        await _navigation.Received().GoToAsync(NavigationService.LibraryListRouteAbsolute);
    }

    [Fact]
    public async Task GoToEdit_Navigates_With_Id()
    {
        var vm = CreateVM();
        var user = CreateUser(Guid.NewGuid());
        vm.User = user;

        await vm.GoToEditCommand.ExecuteAsync(null);

        await _navigation.Received().GoToAsync(
            NavigationService.UserEditRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == user.Id)
        );
    }

    [Fact]
    public async Task GoToLibraryDetail_Navigates_With_Id()
    {
        var vm = CreateVM();
        var libId = Guid.NewGuid();

        await vm.GoToLibraryDetailCommand.ExecuteAsync(libId);

        await _navigation.Received().GoToAsync(
            NavigationService.LibraryListRouteAbsolute + NavigationService.LibraryDetailRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == libId)
        );
    }

    [Fact]
    public async Task Receive_ReloadDataMessage_Refreshes_Immediately()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        vm.Id = id;

        var user = CreateUser(id);
        _userFacade.GetAsync(id).Returns(user);

        vm.Receive(new ReloadDataMessage());

        Assert.Equal(user, vm.User);
    }
}
