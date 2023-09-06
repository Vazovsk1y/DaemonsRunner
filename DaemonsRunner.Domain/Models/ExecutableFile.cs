using DaemonsRunner.Domain.Exceptions.Base;

namespace DaemonsRunner.Domain.Models;

public class ExecutableFile
{
    /// <summary>
    /// File extension.
    /// </summary>
    public string Extension { get; }

    /// <summary>
    /// File path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// File name includes extension.
    /// </summary>
    public string Name { get; }

    private ExecutableFile(
        string name,
        string path,
        string extension)
    {
        Extension = extension;
        Name = name;
        Path = path;
    }

    public static ExecutableFile Create(string path)
    {
        string extension = System.IO.Path.GetExtension(path);
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new DomainException("File extension wasn't correct.");
        }

        string name = System.IO.Path.GetFileName(path);
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException($"Unable to extract the file name from [{path}].");
        }

        return new ExecutableFile(name, path, extension);
    }
}
