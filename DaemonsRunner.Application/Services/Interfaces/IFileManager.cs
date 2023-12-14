using DaemonsRunner.Application.Responses;

namespace DaemonsRunner.Application.Services.Interfaces;

public interface IFileManager
{
    DataResponse<IEnumerable<FileInfo>> SelectFiles(
        string filter = "",
        string title = "Choose files:"
        );

    DataResponse<FileInfo> SelectFile(
        string filter = "",
        string title = "Choose file:"
        );

    DataResponse<DirectoryInfo> SelectFolder(string title = "Choose folder:");
}
