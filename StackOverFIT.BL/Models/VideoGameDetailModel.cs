using CommunityToolkit.Mvvm.ComponentModel;

namespace StackOverFIT.BL.Models;

public partial class VideoGameDetailModel : ModelBase
{
    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public required partial string Description { get; set; }

    [ObservableProperty]
    public partial int PegiRating { get; set; }

    [ObservableProperty]
    public partial string? ImageUrl { get; set; }

    [ObservableProperty]
    public partial System.Collections.ObjectModel.ObservableCollection<StackOverFIT.DAL.Enums.GameGenres> GameGenres { get; set; } = new();

    public static VideoGameDetailModel Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        Description = string.Empty,
        PegiRating = 0
    };
}