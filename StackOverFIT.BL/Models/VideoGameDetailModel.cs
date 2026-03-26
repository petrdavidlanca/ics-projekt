using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class VideoGameDetailModel : ModelBase
{
    [ObservableProperty]
    public required partial string name { get; set; }

    [ObservableProperty]
    public required partial string description { get; set; }

    [ObservableProperty]
    public partial int pegi_rating { get; set; }

    [ObservableProperty]
    public partial string? ImageUrl { get; set; }

    public static VideoGameDetailModel Empty => new()
    {
        Id = Guid.Empty,
        name = string.Empty,
        description = string.Empty,
        pegi_rating = 0
    };
}