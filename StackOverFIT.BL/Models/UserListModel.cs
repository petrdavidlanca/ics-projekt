using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StackOverFIT.BL.Models;

public partial class UserListModel : ModelBase
{
    [ObservableProperty]
    public required partial string Username { get; set; }

    [ObservableProperty]
    public required partial string Email { get; set; }

    public static UserListModel Empty => new()
    {
        Id = Guid.Empty,
        Username = string.Empty,
        Email = string.Empty
    };
}