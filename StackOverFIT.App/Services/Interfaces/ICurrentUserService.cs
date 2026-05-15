using System;

namespace StackOverFIT.App.Services;

public interface ICurrentUserService
{
    Guid CurrentUserId { get; set; }
}