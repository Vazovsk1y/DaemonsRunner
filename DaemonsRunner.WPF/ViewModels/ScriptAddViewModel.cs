using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.BuisnessLayer.Responses;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using DaemonsRunner.ServiceLayer.Responses.DTOs;
using DaemonsRunner.WPF.Views.Windows;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptAddViewModel : ObservableObject
{
	private readonly IUserDialog<ScriptAddWindow> _userDialog;
	private readonly IDataBus _dataBus;
	private readonly IFileDialog _fileDialog;
	private readonly IScriptService _scriptService;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private string _scriptTitle;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private string _scriptCommand;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ExecutableFileName))]
	private FileInfo? _executableFile;

	public string? ExecutableFileName => ExecutableFile?.Name;

	public ScriptAddViewModel(
		IUserDialog<ScriptAddWindow> userDialog,
		IDataBus dataBus,
		IFileDialog fileDialog,
		IScriptService scriptService)
	{
		_userDialog = userDialog;
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
		_userDialog.CloseDialog();
	}

	private bool OnCanAccept()
	{
		return !string.IsNullOrWhiteSpace(ScriptTitle) && !string.IsNullOrWhiteSpace(ScriptCommand);
	}

	[RelayCommand]
	private void Cancel() => _userDialog.CloseDialog();

	[RelayCommand]
	private void SelectFile()
	{
		var response = _fileDialog.StartDialog(multiselect: false);
		if (response.OperationStatus is not StatusCode.Fail)
		{
			ExecutableFile = response.Data.First();
		}
	}
}
