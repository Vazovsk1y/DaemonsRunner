using DaemonsRunner.DAL.DataModels;
using DaemonsRunner.Core.Models;
using System.Reflection;
using DaemonsRunner.Core.Enums;

namespace DaemonsRunner.DAL.Extensions;

public static class Mapper
{
    public static Script ToModel(this ScriptJsonModel model)
    {
        var scriptType = typeof(Script);
        var ctor = scriptType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ScriptId), typeof(string), typeof(string), typeof(RuntimeType) }, null);

		if (ctor is null)
        {
            throw new InvalidOperationException("Unable to find suitable private ctor for creating script.");
        }

        var script = (Script)ctor.Invoke(new object[] { new ScriptId(model.id), model.title, model.command, Enum.Parse<RuntimeType>(model.runtime_type) });
        script.WorkingDirectory = string.IsNullOrWhiteSpace(model.working_directory_path) ? null : new WorkingDirectory { Path = model.working_directory_path };
        return script;
	}

    public static ScriptJsonModel ToJsonModel(this Script script)
    {
        return new ScriptJsonModel
        {
            id = script.Id.Value,
            title = script.Title,
            command = script.Command,
            working_directory_path = script.WorkingDirectory?.Path,
            runtime_type = script.RuntimeType.ToString(),
        };
    }
}
