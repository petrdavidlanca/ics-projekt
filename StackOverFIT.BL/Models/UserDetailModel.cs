using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class UserDetailModel : ModelBase
{
    [ObservableProperty]
    public required partial string username { get; set; }

    [ObservableProperty]
    public required partial string email { get; set; }

    [ObservableProperty]
    public partial string? real_name { get; set; }

    [ObservableProperty]
    public partial string? surname { get; set; }

    [ObservableProperty]
    public partial int total_num_of_games { get; set; }

    [ObservableProperty]
    public partial int total_num_of_libraries { get; set; }

    public static UserDetailModel Empty => new()
    {
        Id = Guid.Empty,
        username = string.Empty,
        email = string.Empty,
        total_num_of_games = 0,
        total_num_of_libraries = 0
    };
}