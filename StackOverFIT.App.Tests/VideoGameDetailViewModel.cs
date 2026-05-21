using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using StackOverFIT.App.Messages;
using StackOverFIT.App.Services;
using StackOverFIT.App.ViewModels;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;

namespace StackOverFIT.App.Tests;
public class VideoGameDetailViewModelTests
{
    private readonly IVideoGameFacade _gameFacade = Substitute.For<IVideoGameFacade>();
    private readonly INavigationService _navigation = Substitute.For<INavigationService>();
    private readonly IMessengerService _messenger = Substitute.For<IMessengerService>();
    private readonly IAlertService _alert = Substitute.For<IAlertService>();

    private VideoGameDetailViewModel CreateVM()
        => new(_gameFacade, _navigation, _messenger, _alert);

    private static VideoGameDetailModel Game(Guid id)
        => new()
        {
            Id = id,
            Name = "TestGame",
            PegiRating = 12,
            Description = "desc"
        };

    [Fact]
    public async Task LoadDataAsync_Loads_Game()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        vm.Id = id;

        var game = Game(id);
        _gameFacade.GetAsync(id).Returns(game);

        await vm.OnAppearingAsync();

        Assert.Equal(game, vm.VideoGame);
    }

    [Fact]
    public async Task Delete_Deletes_Game_And_Navigates_Back()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        var game = Game(id);
        vm.VideoGame = game;

        await vm.DeleteCommand.ExecuteAsync(null);

        await _gameFacade.Received().DeleteAsync(id);
        _messenger.Received().Send(Arg.Any<VideoGameDeleteMessage>());
        _navigation.Received().SendBackButtonPressed();
    }

    [Fact]
    public async Task Delete_Shows_Error_On_Exception()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        var game = Game(id);
        vm.VideoGame = game;

        _gameFacade.DeleteAsync(id).Throws(new InvalidOperationException());

        await vm.DeleteCommand.ExecuteAsync(null);

        await _alert.Received().DisplayAsync("Chyba", "Hru se nepodařilo smazat.");
    }

    [Fact]
    public async Task GoToEdit_Navigates_With_Id()
    {
        var vm = CreateVM();
        var id = Guid.NewGuid();
        var game = Game(id);
        vm.VideoGame = game;

        await vm.GoToEditCommand.ExecuteAsync(null);

        await _navigation.Received().GoToAsync(
            NavigationService.VideoGameEditRouteRelative,
            Arg.Is<Dictionary<string, object?>>(d => (Guid)d["Id"] == id)
        );
    }

    [Fact]
    public async Task Play_Shows_Alert()
    {
        var vm = CreateVM();

        await vm.PlayCommand.ExecuteAsync(null);

        await _alert.Received().DisplayAsync("Play", "Game launched!");
    }
}
