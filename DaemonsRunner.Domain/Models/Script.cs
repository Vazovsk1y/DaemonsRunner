using DaemonsRunner.Domain.Common;

namespace DaemonsRunner.Domain.Models;

public class Script : IExecutableScript
{
    public ExecutableFile? ExecutableFile { get; }

    public string Title { get; }

    public string Command { get; }

    private Script(
        string title,
        string command,
        ExecutableFile? executableFile = null)
    {
        ExecutableFile = executableFile;
        Title = title;
        Command = command;
    }

    public static Script Create(
        string title,
        string command,
        ExecutableFile? executableFile = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

        return new Script(title, command, executableFile);
    }
}
