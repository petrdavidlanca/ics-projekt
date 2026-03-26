using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class LibraryDetailModel : ModelBase
{
    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public partial int NumOfGames { get; set; }

    public static LibraryDetailModel Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        NumOfGames = 0
    };
}