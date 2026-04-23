using CommunityToolkit.Mvvm.ComponentModel;

namespace StackOverFIT.BL.Models;

public partial class LibraryDetailModel : ModelBase
{
    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public partial int NumOfGames { get; set; }

    [ObservableProperty]
    public partial System.Collections.ObjectModel.ObservableCollection<VideoGameListModel> VideoGames { get; set; } = new();

    public static LibraryDetailModel Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        NumOfGames = 0
    };
}