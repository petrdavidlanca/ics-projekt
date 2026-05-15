using StackOverFIT.App.Models;
using StackOverFIT.App.Views.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackOverFIT.App.Views.Library;
using StackOverFIT.App.Views.VideoGame;

namespace StackOverFIT.App.Services;

public class NavigationService : INavigationService
{
    public const string UserListRouteAbsolute = "//users";
    public const string UserDetailRouteRelative = "//app/profile/userdetail";
    public const string UserEditRouteRelative = "/edit";

    public const string VideoGameListRouteAbsolute = "//app/store/videogames";
    public const string VideoGameDetailRouteRelative = "/detail";
    public const string VideoGameEditRouteRelative = "//app/creategame/newgame";

    public const string LibraryListRouteAbsolute = "//app/library/libraries";
    public const string LibraryDetailRouteRelative = "/detail";
    public const string LibraryEditRouteRelative = "/edit";

    public IEnumerable<RouteModel> Routes { get; } = new List<RouteModel>
    {
        new(UserListRouteAbsolute + UserDetailRouteRelative, typeof(UserDetailView)),
        new(UserListRouteAbsolute + UserEditRouteRelative, typeof(UserEditView)),
        new(UserDetailRouteRelative + UserEditRouteRelative, typeof(UserEditView)),
        
        new(LibraryListRouteAbsolute + LibraryDetailRouteRelative, typeof(LibraryDetailView)),
        new(LibraryListRouteAbsolute + LibraryEditRouteRelative, typeof(LibraryEditView)),
        new(LibraryListRouteAbsolute + LibraryDetailRouteRelative + LibraryEditRouteRelative, typeof(LibraryEditView)),
        
        new(VideoGameListRouteAbsolute + VideoGameDetailRouteRelative, typeof(VideoGameDetailView)),
        new(VideoGameListRouteAbsolute + VideoGameEditRouteRelative, typeof(VideoGameEditView)),
        new(VideoGameListRouteAbsolute + VideoGameDetailRouteRelative + VideoGameEditRouteRelative, typeof(VideoGameEditView)),
    };

    public async Task GoToAsync(string route)
        => await Shell.Current.GoToAsync(route);

    public async Task GoToAsync(string route, IDictionary<string, object?> parameters)
        => await Shell.Current.GoToAsync(route, parameters);

    public bool SendBackButtonPressed()
        => Shell.Current.SendBackButtonPressed();
}