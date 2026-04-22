using System;

namespace StackOverFIT.App.Messages;

public record VideoGameEditMessage
{
    public required Guid VideoGameId { get; init; }
}