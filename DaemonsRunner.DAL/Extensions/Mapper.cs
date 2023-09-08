using DaemonsRunner.DAL.Repositories;
using DaemonsRunner.Domain.Models;
using System.Reflection;

namespace DaemonsRunner.DAL.Extensions;

public static class Mapper
{
    public static Script ToModel(this ScriptJsonModel model)
    {
        var scriptType = typeof(Script);
        var ctor = scriptType.GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ScriptId) }, null);

        if (ctor is null)
        {
            throw new InvalidOperationException("Unable to find suitable private ctor for creating script.");
        }

        var script = (Script)ctor.Invoke(new object[] { new ScriptId(model.Id) });
        script.Title = model.Title;
        script.Command = model.Command;
        script.ExecutableFile = model.ExecutableFilePath is null ? null : ExecutableFile.Create(model.ExecutableFilePath);
        return script;
	}

    public static ScriptJsonModel ToJsonModel(this Script script)
    {
        return new ScriptJsonModel
        {
            Id = script.Id.Value,
            Title = script.Title,
            Command = script.Command,
            ExecutableFilePath = script.ExecutableFile?.Path
        };
    }
}
