using DaemonsRunner.BuisnessLayer.Responses;

namespace DaemonsRunner.BuisnessLayer.Services.Interfaces;

/// <summary>
/// Dialog service for interacting with system class OpenFileDialog.
/// </summary>
public interface IFileDialog
{
    /// <summary>
    /// Start dialog with file system and provides you a response that contain all selected files in this dialog.
    /// </summary>
    public DataResponse<IEnumerable<FileInfo>> StartDialog(
        string filter = "all files (*.) | *.", 
        string title = "Choose files:",
        bool multiselect = true,
        CancellationToken cancellationToken = default);
}
