using StackOverFIT.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackOverFIT.App.Services;

public interface INavigationService
{
    IEnumerable<RouteModel> Routes { get; }

    Task GoToAsync(string route);
    Task GoToAsync(string route, IDictionary<string, object?> parameters);
    bool SendBackButtonPressed();
}