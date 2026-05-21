using CommunityToolkit.Mvvm.ComponentModel;

namespace StackOverFIT.BL.Models;

public partial class VideoGameListModel : ModelBase
{
    [ObservableProperty]
    public required partial string Name { get; set; }

    public string? Description { get; set; }

    public int PegiRating { get; set; }

    [ObservableProperty]
    public partial string? ImageUrl { get; set; }

    public static VideoGameListModel Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty
    };
}