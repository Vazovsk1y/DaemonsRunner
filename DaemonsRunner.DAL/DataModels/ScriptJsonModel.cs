namespace DaemonsRunner.DAL.DataModels;

public class ScriptJsonModel
{
#pragma warning disable IDE1006 // Naming Styles

    public required Guid id { get; init; }

    public string title { get; set; } = null!;

    public string command { get; set; } = null!;

    public string? executable_file_path { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}
