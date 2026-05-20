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
public class UserEditViewModelTests
{
    private readonly IUserFacade _userFacade = Substitute.For<IUserFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IAlertService _alert = Substitute.For<IAlertService>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();

    private UserEditViewModel CreateVM()
        => new(_userFacade, _navigation, _alert, _messenger);

    private static UserDetailModel CreateUser(Guid id)
        => new()
        {
            Id = id,
            Username = "tester",
            Email = "tester@example.com",
            GameLibrary = new ObservableCollection<LibraryListModel>()
        };

    [Fact]
    public async Task LoadDataAsync_Loads_User()
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
    public async Task Save_Shows_Error_When_Username_Or_Email_Missing()
    {
        var vm = CreateVM();
        vm.User = CreateUser(Guid.NewGuid());
        vm.User.Username = "";
        vm.User.Email = "";

        await vm.SaveCommand.ExecuteAsync(null);

        await _alert.Received().DisplayAsync("Missing", "Username or email.");
        await _userFacade.DidNotReceive().SaveAsync(Arg.Any<UserDetailModel>());
    }

    [Fact]
    public async Task Save_Shows_Error_When_Email_Invalid()
    {
        var vm = CreateVM();
        vm.User = CreateUser(Guid.NewGuid());
        vm.User.Email = "wrongemail";

        await vm.SaveCommand.ExecuteAsync(null);

        await _alert.Received().DisplayAsync("Wrong email", "Email missing '@'");
        await _userFacade.DidNotReceive().SaveAsync(Arg.Any<UserDetailModel>());
    }

    [Fact]
    public async Task Save_Saves_User()
    {
        var vm = CreateVM();
        var user = CreateUser(Guid.NewGuid());
        vm.User = user;

        await vm.SaveCommand.ExecuteAsync(null);

        await _userFacade.Received().SaveAsync(user);
        _messenger.Received().Send(Arg.Is<UserEditMessage>(m => m.UserId == user.Id));
        _navigation.Received().SendBackButtonPressed();
    }

    [Fact]
    public void Cancel_Navigates_Back()
    {
        var vm = CreateVM();

        vm.CancelCommand.Execute(null);

        _navigation.Received().SendBackButtonPressed();
    }
}
