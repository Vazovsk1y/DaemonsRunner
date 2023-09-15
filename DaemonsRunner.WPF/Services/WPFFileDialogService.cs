using Microsoft.Win32;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DaemonsRunner.BuisnessLayer.Responses;
using System.IO;

namespace DaemonsRunner.Services;

public class WPFFileDialogService : IFileDialog
{
    public DataResponse<FileInfo> SelectFile(string filter = "", string title = "Choose file:")
    {
        var fileDialog = new OpenFileDialog
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
        var fileDialog = new OpenFileDialog
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
}
