using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.Application.Responses;
using DaemonsRunner.Application.Services.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using DaemonsRunner.Application.Responses.DTOs;
using DaemonsRunner.WPF.Views.Windows;
using System.IO;
using System.Threading.Tasks;
using DaemonsRunner.Services;
using DaemonsRunner.Core.Enums;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptAddViewModel : ObservableObject
{
	private readonly IDataBus _dataBus;
	private readonly IFileManager _fileDialog;
	private readonly IScriptService _scriptService;
	private readonly IUserDialog<ScriptAddWindow> _scriptAddDialog;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private string _scriptTitle = string.Empty;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	private string _scriptCommand = string.Empty;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(WorkingDirectoryName))]
	private DirectoryInfo? _workingDirectory;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
	[NotifyPropertyChangedFor(nameof(RuntimeType))]
	private bool _isPowershell;

	public RuntimeType RuntimeType => IsPowershell ? RuntimeType.Powershell : RuntimeType.Cmd;

	public string? WorkingDirectoryName => WorkingDirectory?.Name ?? "*Имя исполняемого файла*";

	public ScriptAddViewModel(
		IUserDialog<ScriptAddWindow> scriptAddDialog,
		IDataBus dataBus,
		IFileManager fileDialog,
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
		var dto = new ScriptAddDTO(ScriptTitle, ScriptCommand, RuntimeType, WorkingDirectory?.FullName);
		var response = await _scriptService.SaveAsync(dto);
		if (response.OperationStatus is StatusCode.Success)
		{
			_dataBus.Send(new ScriptAddedMessage(new ScriptViewModel
			{
				ScriptId = response.Data,
				Command = dto.Command,
				Title = dto.Title,
				WorkingDirectory = WorkingDirectory,
				RuntimeType = RuntimeType,
			}));
		}

		_dataBus.Send(response.Description);
		_scriptAddDialog.CloseDialog();
	}

	private bool OnCanAccept()
	{
		return !string.IsNullOrWhiteSpace(ScriptTitle) 
			&& !string.IsNullOrWhiteSpace(ScriptCommand)
			&& WorkingDirectory is not { Exists: false };
	}

	[RelayCommand]
	private void Cancel() => _scriptAddDialog.CloseDialog();

	[RelayCommand]
	private void SelectFolder()
	{
		var response = _fileDialog.SelectFolder();
		if (response.OperationStatus is StatusCode.Success)
		{
			WorkingDirectory = response.Data;
		}
	}
}
