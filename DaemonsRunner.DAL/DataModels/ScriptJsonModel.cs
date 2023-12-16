namespace DaemonsRunner.DAL.DataModels;

public class ScriptJsonModel
{
#pragma warning disable IDE1006 // Naming Styles

    public required Guid id { get; init; }

    public required string title { get; set; }

    public required string command { get; set; }

    public string? working_directory_path { get; set; }

    public required string runtime_type { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}
