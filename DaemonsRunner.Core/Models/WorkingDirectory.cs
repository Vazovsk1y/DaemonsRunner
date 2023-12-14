namespace DaemonsRunner.Core.Models;

public record WorkingDirectory
{
    /// <summary>
    /// Working directory path.
    /// </summary>
    public required string Path { get; init; }
}
