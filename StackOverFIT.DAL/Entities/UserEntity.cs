namespace StackOverFIT.DAL.Entities;

public record UserEntity: IEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? RealName { get; set; }
    public string? Surname { get; set; }
    public required int TotalNumOfGames { get; set; }
    public required int TotalNumOfLibraries { get; set; }
    public ICollection<LibraryEntity> GameLibrary { get; init; } = new List<LibraryEntity>();
    public required Guid Id { get; set; }
}
