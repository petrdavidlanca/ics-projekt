namespace StackOverFIT.DAL.Entities;

public record user_entity: IEntity
{
    public required string username { get; set; }
    public required string email { get; set; }
    public string? real_name { get; set; }
    public string? surname { get; set; }
    public required int total_num_of_games { get; set; }
    public required int total_num_of_libraries { get; set; }
    public ICollection<library_entity> game_library { get; init; } = new List<library_entity>();
    public required Guid Id { get; set; }
}
