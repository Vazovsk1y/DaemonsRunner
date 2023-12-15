using DaemonsRunner.Core.Common;
using DaemonsRunner.Core.Enums;

namespace DaemonsRunner.Core.Models;

public class Script : Entity<ScriptId>
{
    public WorkingDirectory? WorkingDirectory { get; set; }

    public required string Title { get; set; }

    public required string Command { get; set; }

    public required RuntimeType RuntimeType { get; set; }

    public Script() : base() { }

    // for mapper
    private Script(ScriptId scriptId, string title, string command, RuntimeType runtimeType) : base(scriptId) 
    {
        Title = title;
        Command = command;
        RuntimeType = runtimeType;
    }
}

public record ScriptId(Guid Value) : IValueId<ScriptId>
{
    public static ScriptId Create() => new(Guid.NewGuid());
}
