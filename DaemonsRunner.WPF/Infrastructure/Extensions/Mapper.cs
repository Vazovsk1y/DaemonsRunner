using DaemonsRunner.Application.Responses.DTOs;
using DaemonsRunner.WPF.ViewModels;

namespace DaemonsRunner.WPF.Infrastructure.Extensions;

internal static class Mapper
{
	public static ScriptViewModel ToViewModel(this ScriptDTO scriptDTO)
	{
		return new ScriptViewModel
		{
			ScriptId = scriptDTO.Id,
			Title = scriptDTO.Title,
			Command = scriptDTO.Command,
			WorkingDirectory = string.IsNullOrWhiteSpace(scriptDTO.WorkingDirectoryPath) ? null : new System.IO.DirectoryInfo(scriptDTO.WorkingDirectoryPath),
		};
	}
}
