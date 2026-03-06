using StackOverFIT.DAL.Enums;

namespace StackOverFIT.DAL.Entities;

public record video_game_entity : IEntity
{
    public required string name { get; set; }
    public required string description { get; set; }
    public required int pegi_rating { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<game_genres> Ingredients { get; init; } = new List<game_genres>();
    public required Guid Id { get; set; }
}
