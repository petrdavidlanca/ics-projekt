using System;

namespace StackOverFIT.App.Messages;

public record LibraryEditMessage
{
    public required Guid LibraryId { get; init; }
}