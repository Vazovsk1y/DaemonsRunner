using DaemonsRunner.DAL.DataModels;
using DaemonsRunner.Core.Models;
using System.Reflection;
using DaemonsRunner.Core.Enums;

namespace DaemonsRunner.DAL.Extensions;

public static class Mapper
{
    public static Script ToDomainModel(this ScriptJsonModel jsonModel)
    {
        var scriptType = typeof(Script);
        var ctor = scriptType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ScriptId), typeof(string), typeof(string), typeof(RuntimeType) }, null);

		if (ctor is null)
        {
            throw new InvalidOperationException("Unable to find suitable private ctor for creating script.");
        }

        var script = (Script)ctor.Invoke(new object[] { new ScriptId(jsonModel.id), jsonModel.title, jsonModel.command, Enum.Parse<RuntimeType>(jsonModel.runtime_type) });
        script.WorkingDirectory = string.IsNullOrWhiteSpace(jsonModel.working_directory_path) ? null : new WorkingDirectory { Path = jsonModel.working_directory_path };
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
