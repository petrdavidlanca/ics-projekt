namespace StackOverFIT.DAL.Options;

public record DALOptions
{
    public string DatabaseDirectory { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string DatabaseFilePath => Path.Combine(DatabaseDirectory, DatabaseName);

    public bool RecreateDatabaseEachTime { get; set; } = false;

    public bool SeedDemoData { get; set; } = false;
}
