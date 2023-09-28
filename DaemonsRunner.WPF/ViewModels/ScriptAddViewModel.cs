using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.BuisnessLayer.Responses;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using DaemonsRunner.ServiceLayer.Responses.DTOs;
using DaemonsRunner.WPF.Views.Windows;
using System.IO;
using System.Threading.Tasks;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptAddViewModel : ObservableObject
{
	private readonly IDataBus _dataBus;
	private readonly IFileDialog _fileDialog;
	private readonly IScriptService _scriptService;
	private readonly IUserDialog<ScriptAddWindow> _scriptAddDialog;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private string _scriptTitle = string.Empty;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private string _scriptCommand = string.Empty;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ExecutableFileName))]
	private FileInfo? _executableFile;

	public string? ExecutableFileName => ExecutableFile?.Name ?? "*Имя исполняемого файла*";

	public ScriptAddViewModel(
		IUserDialog<ScriptAddWindow> scriptAddDialog,
		IDataBus dataBus,
		IFileDialog fileDialog,
		IScriptService scriptService)
	{
		_scriptAddDialog = scriptAddDialog;
		_dataBus = dataBus;
		_fileDialog = fileDialog;
		_scriptService = scriptService;
	}

	[RelayCommand(CanExecute = nameof(OnCanAccept))]
	private async Task Accept()
	{
		var dto = new ScriptAddDTO(ScriptTitle, ScriptCommand, ExecutableFile?.FullName);
		var response = await _scriptService.SaveAsync(dto);
		if (response.OperationStatus is StatusCode.Success)
		{
			_dataBus.Send(new ScriptAddedMessage(new ScriptViewModel
			{
				ScriptId = response.Data,
				Command = dto.Command,
				Title = dto.Title,
				ExecutableFileViewModel = ExecutableFile is null ? null : new ExecutableFileViewModel
				{
					Path = ExecutableFile.FullName,
					Name = ExecutableFile.Name,
					Extension = ExecutableFile.Extension
				}
			}));
		}

		_dataBus.Send(response.Description);
		_scriptAddDialog.CloseDialog();
	}

	private bool OnCanAccept()
	{
		return !string.IsNullOrWhiteSpace(ScriptTitle) && !string.IsNullOrWhiteSpace(ScriptCommand);
	}

	[RelayCommand]
	private void Cancel() => _scriptAddDialog.CloseDialog();

	[RelayCommand]
	private void SelectFile()
	{
		var response = _fileDialog.SelectFile();
		if (response.OperationStatus is StatusCode.Success)
		{
			ExecutableFile = response.Data;
		}
	}
}
