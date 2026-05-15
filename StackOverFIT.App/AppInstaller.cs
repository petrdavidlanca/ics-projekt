using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using StackOverFIT.App.Services;
using StackOverFIT.App.ViewModels;
using StackOverFIT.App.Views.Library;
using StackOverFIT.App.Views.User;
using StackOverFIT.App.Views.VideoGame;

namespace StackOverFIT.App;

public static class AppInstaller
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<AppShell>();
        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        services.AddSingleton<IMessengerService, MessengerService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

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
        services.AddTransient<UserListView>();
        services.AddTransient<UserDetailView>();
        services.AddTransient<UserEditView>();

        services.AddTransient<LibraryListView>();
        services.AddTransient<LibraryDetailView>();
        services.AddTransient<LibraryEditView>();

        services.AddTransient<VideoGameListView>();
        services.AddTransient<VideoGameDetailView>();
        services.AddTransient<VideoGameEditView>();
        return services;
    }
}