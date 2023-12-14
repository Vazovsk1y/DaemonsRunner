namespace DaemonsRunner.DAL.DataModels;

public class ScriptJsonModel
{
#pragma warning disable IDE1006 // Naming Styles

    public required Guid id { get; init; }

    public string title { get; set; } = null!;

    public string command { get; set; } = null!;

    public string? working_directory_path { get; set; }

    public string runtime_type { get; set; } = null!;

#pragma warning restore IDE1006 // Naming Styles
}
