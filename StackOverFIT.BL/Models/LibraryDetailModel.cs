using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class LibraryDetailModel : ModelBase
{
    [ObservableProperty]
    public required partial string name { get; set; }

    [ObservableProperty]
    public partial string? description { get; set; }

    [ObservableProperty]
    public partial int num_of_games { get; set; }

    public static LibraryDetailModel Empty => new()
    {
        Id = Guid.Empty,
        name = string.Empty,
        num_of_games = 0
    };
}