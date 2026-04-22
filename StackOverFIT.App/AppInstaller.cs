using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using StackOverFIT.App.Services;
using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App;

public static class AppInstaller
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        services.AddSingleton<IMessengerService, MessengerService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAlertService, AlertService>();

        services.AddTransient<UserListViewModel>();
        services.AddTransient<UserDetailViewModel>();
        services.AddTransient<UserEditViewModel>();

        services.AddTransient<LibraryListViewModel>();
        services.AddTransient<LibraryDetailViewModel>();
        services.AddTransient<LibraryEditViewModel>();

        services.AddTransient<VideoGameListViewModel>();
        services.AddTransient<VideoGameDetailViewModel>();
        services.AddTransient<VideoGameEditViewModel>();

        //potom i views napr. services.AddTransient<UserListView>();

        return services;
    }
}