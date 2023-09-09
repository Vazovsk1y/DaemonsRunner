using DaemonsRunner.ServiceLayer.Responses.DTOs;
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
			ExecutableFileViewModel = scriptDTO.ExecutableFile?.ToViewModel(),
		};
	}

	public static ExecutableFileViewModel ToViewModel(this ExecutableFileDTO executableFileDTO)
	{
		return new ExecutableFileViewModel
		{
			Path = executableFileDTO.Path,
			Extension = executableFileDTO.Extension,
			Name = executableFileDTO.Name,
		};
	}
}
