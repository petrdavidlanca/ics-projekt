namespace StackOverFIT.DAL.Entities;

public record library_entity: IEntity
{
    public required string name { get; set; }
    public string description { get; set; }
    public int num_of_games { get; set; }
    public ICollection<video_game_entity> video_games { get; init; } = new List<video_game_entity>();
    public required Guid Id { get; set; }
}
