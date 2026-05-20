using System.Threading.Tasks;
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
public class LibraryEditViewModelTests
{
    private readonly ILibraryFacade _libraryFacade = Substitute.For<ILibraryFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();
    private readonly IAlertService _alert = Substitute.For<IAlertService>();
    private readonly IUserFacade _userFacade = Substitute.For<IUserFacade>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

    private LibraryEditViewModel CreateVM()
        => new(_libraryFacade, _navigation, _messenger, _alert, _userFacade, _currentUser);

    private static LibraryDetailModel CreateLibrary(string name = "MyLib")
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            UserId = Guid.NewGuid(),
            VideoGames = new ObservableCollection<VideoGameListModel>(),
            NumOfGames = 0
        };

    [Fact]
    public async Task LoadDataAsync_Loads_Existing_Library_When_Id_Set()
    {
        var vm = CreateVM();
        var lib = CreateLibrary("LoadedLib");
        vm.Id = lib.Id;

        _libraryFacade.GetAsync(lib.Id).Returns(lib);

        await vm.OnAppearingAsync();

        Assert.Equal("LoadedLib", vm.Library.Name);
    }

    [Fact]
    public async Task LoadDataAsync_Creates_Empty_Library_When_Id_Empty()
    {
        var vm = CreateVM();
        vm.Id = Guid.Empty;
        vm.UserId = Guid.NewGuid();

        await vm.OnAppearingAsync();

        Assert.Equal(vm.UserId, vm.Library.UserId);
        Assert.Equal("", vm.Library.Name);
    }

    [Fact]
    public async Task Save_Shows_Validation_Error_When_Name_Empty()
    {
        var vm = CreateVM();
        vm.Library = CreateLibrary("");
        vm.Library.Name = "   "; // whitespace

        await vm.SaveCommand.ExecuteAsync(null);

        await _alert.Received().DisplayAsync("Validation Error", "Please enter a name for the library.");
        await _libraryFacade.DidNotReceive().SaveAsync(Arg.Any<LibraryDetailModel>());
    }

    [Fact]
    public async Task Save_Shows_Error_When_Duplicate_Name()
    {
        var vm = CreateVM();
        vm.Library = CreateLibrary("Games");

        var user = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Username = "tester",
            Email = "tester@example.com",
            GameLibrary = new ObservableCollection<LibraryListModel>
            {
                new() { Id = Guid.NewGuid(), Name = "Games" } // duplicate
            }
        };

        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);

        await vm.SaveCommand.ExecuteAsync(null);

        await _alert.Received().DisplayAsync("Validation Error", "You already have a library named 'Games'.");
        await _libraryFacade.DidNotReceive().SaveAsync(Arg.Any<LibraryDetailModel>());
    }

    [Fact]
    public async Task Save_Saves_New_Library()
    {
        var vm = CreateVM();
        vm.Id = Guid.Empty;
        vm.Library = CreateLibrary("NewLib");

        var user = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Username = "tester",
            Email = "tester@example.com",
            GameLibrary = new ObservableCollection<LibraryListModel>()
        };

        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);

        await vm.SaveCommand.ExecuteAsync(null);

        await _libraryFacade.Received().SaveAsync(Arg.Is<LibraryDetailModel>(l => l.Name == "NewLib"));
        _messenger.Received().Send(Arg.Any<LibraryEditMessage>());
        await _navigation.Received().GoToAsync(NavigationService.LibraryListRouteAbsolute);
    }

    [Fact]
    public async Task Save_Updates_Existing_Library()
    {
        var vm = CreateVM();
        var lib = CreateLibrary("UpdatedName");
        vm.Id = lib.Id;
        vm.Library = lib;

        var user = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            Username = "tester",
            Email = "tester@example.com",
            GameLibrary = new ObservableCollection<LibraryListModel>()
        };

        _currentUser.CurrentUserId.Returns(user.Id);
        _userFacade.GetAsync(user.Id).Returns(user);

        await vm.SaveCommand.ExecuteAsync(null);

        await _libraryFacade.Received().SaveAsync(Arg.Is<LibraryDetailModel>(l => l.Id == lib.Id));
    }

    [Fact]
    public void Cancel_Navigates_Back()
    {
        var vm = CreateVM();

        vm.CancelCommand.Execute(null);

        _navigation.Received().SendBackButtonPressed();
    }
}
