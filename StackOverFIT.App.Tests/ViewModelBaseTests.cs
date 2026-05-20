using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using StackOverFIT.App.Services;
using StackOverFIT.App.ViewModels;
using Xunit;

namespace StackOverFIT.App.Tests;
public class ViewModelBaseTests
{
    private class FakeMessengerService : IMessengerService
    {
        public IMessenger Messenger { get; } = WeakReferenceMessenger.Default;

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            Messenger.Send(message);
        }
    }

    private class TestViewModel : ViewModelBase
    {
        public int LoadCount { get; private set; }

        public TestViewModel(IMessengerService messengerService)
            : base(messengerService)
        {
        }

        protected override Task LoadDataAsync()
        {
            LoadCount++;
            return Task.CompletedTask;
        }
        public void TriggerForceRefresh()
        {
            ForceDataRefreshOnNextAppearing();
        }
    }

    [Fact]
    public async Task OnAppearingAsync_Loads_Data_Only_Once()
    {
        var vm = new TestViewModel(new FakeMessengerService());

        await vm.OnAppearingAsync();
        await vm.OnAppearingAsync();

        Assert.Equal(1, vm.LoadCount);
    }

    [Fact]
    public async Task ForceDataRefreshOnNextAppearing_Triggers_Reload()
    {
        var vm = new TestViewModel(new FakeMessengerService());

        await vm.OnAppearingAsync();
        vm.TriggerForceRefresh();
        await vm.OnAppearingAsync();

        Assert.Equal(2, vm.LoadCount);
    }

    [Fact]
    public void Constructor_Sets_IsActive()
    {
        var vm = new TestViewModel(new FakeMessengerService());

        Assert.True(vm.IsActive);
    }

    [Fact]
    public async Task LoadDataAsync_Is_Called_On_First_Appear()
    {
        var vm = new TestViewModel(new FakeMessengerService());

        await vm.OnAppearingAsync();

        Assert.Equal(1, vm.LoadCount);
    }
}
