using CommunityToolkit.Mvvm.Input;
using StackOverFIT.App.Services;
using StackOverFIT.App.Messages;

namespace StackOverFIT.App;

public partial class AppShell : Shell
{
    private readonly IMessengerService _messengerService;
    public AppShell(IMessengerService messengerService)
    {
        _messengerService = messengerService;
        InitializeComponent();

    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        _messengerService.Send(new ReloadDataMessage());
    }
}