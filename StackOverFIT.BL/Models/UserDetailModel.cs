using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class UserDetailModel : ModelBase
{
    [ObservableProperty]
    public required partial string Username { get; set; }

    [ObservableProperty]
    public required partial string Email { get; set; }

    [ObservableProperty]
    public partial string? RealName { get; set; }

    [ObservableProperty]
    public partial string? Surname { get; set; }

    [ObservableProperty]
    public partial int TotalNumOfGames { get; set; }

    [ObservableProperty]
    public partial int TotalNumOfLibraries { get; set; }

    public static UserDetailModel Empty => new()
    {
        Id = Guid.Empty,
        Username = string.Empty,
        Email = string.Empty,
        TotalNumOfGames = 0,
        TotalNumOfLibraries = 0
    };
}