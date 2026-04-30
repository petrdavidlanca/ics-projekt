using System;

namespace StackOverFIT.DAL.Entities;

public record LibraryVideoGameEntity : IEntity
{
    public required Guid Id { get; set; }
    public required Guid LibraryId { get; set; }
    public LibraryEntity? Library { get; set; }
    public required Guid VideoGameId { get; set; }
    public VideoGameEntity? VideoGame { get; set; }
}