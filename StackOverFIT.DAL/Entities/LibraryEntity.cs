namespace StackOverFIT.DAL.Entities;

public record LibraryEntity: IEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int NumOfGames { get; set; }
    public ICollection<VideoGameEntity> VideoGames { get; init; } = new List<VideoGameEntity>();
    public Guid? UserEntityId { get; set; }
    public required Guid Id { get; set; }
    public UserEntity? User { get; set; }
}
