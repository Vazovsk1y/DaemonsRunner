using DaemonsRunner.BuisnessLayer.Responses;

namespace DaemonsRunner.BuisnessLayer.Services.Interfaces;

public interface IFileDialog
{
    DataResponse<IEnumerable<FileInfo>> SelectFiles(
        string filter = "",
        string title = "Choose files:"
        );

    DataResponse<FileInfo> SelectFile(
        string filter = "",
        string title = "Choose file:"
        );
}
