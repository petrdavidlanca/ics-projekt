using StackOverFIT.DAL.Enums;

namespace StackOverFIT.DAL.Entities;

public record VideoGameEntity : IEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int PegiRating { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public ICollection<GameGenres> GameGenres { get; init; } = new List<GameGenres>();
    public required Guid Id { get; set; }
    public ICollection<LibraryEntity> Libraries { get; init; } = new List<LibraryEntity>();
}
