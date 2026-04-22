using StackOverFIT.App.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackOverFIT.App.Services;

public class NavigationService : INavigationService
{
    public const string UserListRouteAbsolute = "//users";
    public const string UserDetailRouteRelative = "/detail";
    public const string UserEditRouteRelative = "/edit";

    public const string LibraryListRouteAbsolute = "//libraries";
    public const string LibraryDetailRouteRelative = "/detail";
    public const string LibraryEditRouteRelative = "/edit";

    public const string VideoGameListRouteAbsolute = "//videogames";
    public const string VideoGameDetailRouteRelative = "/detail";
    public const string VideoGameEditRouteRelative = "/edit";

    public IEnumerable<RouteModel> Routes { get; } = new List<RouteModel>
    {
        /*az bude views (XAML)
        new(UserListRouteAbsolute, typeof(UserListView)),
        new(UserListRouteAbsolute + UserDetailRouteRelative, typeof(UserDetailView)),
        new(UserListRouteAbsolute + UserEditRouteRelative, typeof(UserEditView)),
        new(UserListRouteAbsolute + UserDetailRouteRelative + UserEditRouteRelative, typeof(UserEditView)),
        
        // + library a videogame
        */
    };

    public async Task GoToAsync(string route)
        => await Shell.Current.GoToAsync(route);

    public async Task GoToAsync(string route, IDictionary<string, object?> parameters)
        => await Shell.Current.GoToAsync(route, parameters);

    public bool SendBackButtonPressed()
        => Shell.Current.SendBackButtonPressed();
}