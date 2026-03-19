using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class UserListModel : ModelBase
{
    [ObservableProperty]
    public required partial string username { get; set; }

    [ObservableProperty]
    public required partial string email { get; set; }

    public static UserListModel empty => new()
    {
        Id = Guid.Empty,
        username = string.Empty,
        email = string.Empty
    };
}