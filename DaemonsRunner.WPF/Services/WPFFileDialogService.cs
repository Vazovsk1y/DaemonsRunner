using Microsoft.Win32;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using DaemonsRunner.BuisnessLayer.Responses;
using System.IO;

namespace DaemonsRunner.Services;

public class WPFFileDialogService : IFileDialog
{
	private readonly ILogger<WPFFileDialogService> _logger;

	public WPFFileDialogService(ILogger<WPFFileDialogService> logger)
	{
		_logger = logger;
	}

	public DataResponse<IEnumerable<FileInfo>> StartDialog(
	string filter = "",
	string title = "Choose files:",
	bool multiselect = true)
	{
		var fileDialog = new OpenFileDialog
		{
			Multiselect = multiselect,
			Filter = filter,
			Title = title,
			RestoreDirectory = true,
		};

		var dialogResult = fileDialog.ShowDialog();

		if (dialogResult is bool result && result is true)
		{
			var data = GetFileInfos(fileDialog);
			return Response.Success(data, $"[{data.Count()}] files were selected.");
		}

		return Response.Fail<IEnumerable<FileInfo>>("No files were selected.");
	}

	private IEnumerable<FileInfo> GetFileInfos(OpenFileDialog fileDialog)
	{
		var fullFilesPath = fileDialog.FileNames;
		List<FileInfo> files = new();

		for (int i = 0; i < fullFilesPath.Length; i++)
		{
			files.Add(new FileInfo(fullFilesPath[i]));
		}

		_logger.LogInformation("Files count in selected files [{demonsCount}]", files.Count);
		return files;
	}
}
