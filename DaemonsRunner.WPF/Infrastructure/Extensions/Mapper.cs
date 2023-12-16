using DaemonsRunner.Application.Responses.DTOs;
using DaemonsRunner.Core.Models;
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
			RuntimeType = scriptDTO.RuntimeType,
		};
	}

	public static ScriptViewModel ToViewModel(this ScriptAddDTO scriptAddDTO, ScriptId scriptId)
	{
		return new ScriptViewModel
		{
			ScriptId = scriptId,
			Title = scriptAddDTO.Title,
			Command = scriptAddDTO.Command,
			WorkingDirectory = string.IsNullOrWhiteSpace(scriptAddDTO.WorkingDirectoryPath) ? null : new System.IO.DirectoryInfo(scriptAddDTO.WorkingDirectoryPath),
			RuntimeType = scriptAddDTO.RuntimeType,
		};
	}
}
