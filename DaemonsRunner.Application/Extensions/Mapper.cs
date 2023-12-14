using DaemonsRunner.Core.Models;
using DaemonsRunner.Application.Responses.DTOs;

namespace DaemonsRunner.Application.Extensions;

internal static class Mapper
{
	public static ScriptDTO ToDTO(this Script script)
	{
		return new ScriptDTO
		(
			script.Id,
			script.Title,
			script.Command,
			script.WorkingDirectory?.Path
		);
	}
}
