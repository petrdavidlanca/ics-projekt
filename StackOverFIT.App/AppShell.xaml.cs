using CommunityToolkit.Mvvm.Input;
using StackOverFIT.App.Services;
using StackOverFIT.App.Views.User;

namespace StackOverFIT.App;

public partial class AppShell : Shell
{
    private readonly INavigationService _navigationService;
    public AppShell(INavigationService navigationService)
    {
        _navigationService = navigationService;
        InitializeComponent();

    }
}