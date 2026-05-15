using System;

namespace StackOverFIT.App.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid CurrentUserId { get; set; } = Guid.Empty;
}