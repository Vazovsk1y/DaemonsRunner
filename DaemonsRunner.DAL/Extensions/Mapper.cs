using DaemonsRunner.DAL.Repositories;
using DaemonsRunner.Domain.Models;
using System.Reflection;

namespace DaemonsRunner.DAL.Extensions;

public static class Mapper
{
    public static Script ToModel(this ScriptJsonModel model)
    {
        var scriptType = typeof(Script);
        var ctor = scriptType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ScriptId) }, null);

		if (ctor is null)
        {
            throw new InvalidOperationException("Unable to find suitable private ctor for creating script.");
        }

        var script = (Script)ctor.Invoke(new object[] { new ScriptId(model.id) });
        script.Title = model.title;
        script.Command = model.command;
        script.ExecutableFile = model.executable_file_path is null ? null : ExecutableFile.Create(model.executable_file_path);
        return script;
	}

    public static ScriptJsonModel ToJsonModel(this Script script)
    {
        return new ScriptJsonModel
        {
            id = script.Id.Value,
            title = script.Title,
            command = script.Command,
            executable_file_path = script.ExecutableFile?.Path
        };
    }
}
