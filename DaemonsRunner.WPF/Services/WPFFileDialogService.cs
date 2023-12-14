using DaemonsRunner.Application.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DaemonsRunner.Application.Responses;
using System.IO;
using System.Windows.Forms;

namespace DaemonsRunner.Services;

public class WPFFileDialogService : IFileManager
{
    public DataResponse<FileInfo> SelectFile(string filter = "", string title = "Choose file:")
    {
        var fileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Multiselect = false,
            Filter = filter,
            Title = title,
            RestoreDirectory = true,
        };

        var dialogResult = fileDialog.ShowDialog();

        if (dialogResult is bool result && result is true)
        {
            return Response.Success(new FileInfo(fileDialog.FileName));
        }

        return Response.Fail<FileInfo>("No files were selected.");
    }

    public DataResponse<IEnumerable<FileInfo>> SelectFiles(string filter = "", string title = "Choose files:")
    {
        var fileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Multiselect = true,
            Filter = filter,
            Title = title,
            RestoreDirectory = true,
        };

        var dialogResult = fileDialog.ShowDialog();

        if (dialogResult is bool result && result is true)
        {
            var data = fileDialog.FileNames.Select(e => new FileInfo(e));
            return Response.Success(data, $"[{data.Count()}] files were selected.");
        }

        return Response.Fail<IEnumerable<FileInfo>>("No files were selected.");
    }

	public DataResponse<DirectoryInfo> SelectFolder(string title = "Choose folder:")
	{
		using var folderBrowserDialog = new FolderBrowserDialog()
		{
			ShowNewFolderButton = false,
			Description = title,
            UseDescriptionForTitle = true,
		};

		var dialogResult = folderBrowserDialog.ShowDialog();
		if (dialogResult is DialogResult.OK)
		{
			return Response.Success(new DirectoryInfo(folderBrowserDialog.SelectedPath));
		}

		return Response.Fail<DirectoryInfo>("No directory was selected.");
	}
}
