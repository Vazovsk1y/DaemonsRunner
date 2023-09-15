using DaemonsRunner.Domain.Models;
using DaemonsRunner.ServiceLayer.Responses.DTOs;

namespace DaemonsRunner.ServiceLayer.Extensions;

internal static class Mapper
{
	public static ScriptDTO ToDTO(this Script script)
	{
		return new ScriptDTO
		(
			script.Id,
			script.Title,
			script.Command,
			script.ExecutableFile?.ToDTO()
		);
	}

	public static ExecutableFileDTO ToDTO(this ExecutableFile file) 
	{
		return new ExecutableFileDTO
		(
			file.Path,
			file.Name,
			file.Extension
		);
	}
}
