using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class VideoGameListModel : ModelBase
{
    [ObservableProperty]
    public required partial string name { get; set; }

    [ObservableProperty]
    public partial string? ImageUrl { get; set; }

    public static VideoGameListModel Empty => new()
    {
        Id = Guid.Empty,
        name = string.Empty
    };
}