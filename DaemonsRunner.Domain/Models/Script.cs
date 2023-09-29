using DaemonsRunner.Domain.Common;

namespace DaemonsRunner.Domain.Models;

public class Script : Entity<ScriptId>
{
    public ExecutableFile? ExecutableFile { get; set; }

    public string Title { get; set; }

    public string Command { get; set; }

    private Script(
        string title,
        string command,
        ExecutableFile? executableFile = null) : base()
    {
        ExecutableFile = executableFile;
        Title = title;
        Command = command;
    }

    // for EF
    private Script(ScriptId scriptId) : base(scriptId) { }

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

public record ScriptId(Guid Value) : IValueId<ScriptId>
{
    public static ScriptId Create() => new(Guid.NewGuid());
}
