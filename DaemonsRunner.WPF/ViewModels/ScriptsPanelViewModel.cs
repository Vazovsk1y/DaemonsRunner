using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.BuisnessLayer.Responses;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using DaemonsRunner.ViewModels.Interfaces;
using DaemonsRunner.WPF.Infrastructure.Extensions;
using DaemonsRunner.WPF.ViewModels;
using DaemonsRunner.WPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DaemonsRunner.ViewModels;

internal partial class ScriptsPanelViewModel : ObservableRecipient
{
    #region --Fields--

    private readonly IDataBus _dataBus;
    private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly IScriptExecutorViewModelFactory _scriptExecutorViewModelFactory;
	private readonly IUserDialog<ScriptAddWindow> _userDialog;
	private readonly ObservableCollection<ScriptExecutorViewModel> _runningScripts = new();
    private readonly ObservableCollection<ScriptViewModel> _scripts = new();
    private readonly ICollection<IDisposable> _subscriptions = new List<IDisposable>();
    private bool? _isStartButtonEnable = null;
    private bool? _isStopButtonEnable = null;

    #endregion

    #region --Properties--

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveScriptCommand))]
	private ScriptViewModel? _selectedScript;

	public bool? IsStartButtonEnable
    {
        get
        {
            bool defaultCondition = Scripts.Count > 0 &&
                  RunningScripts.Count == 0;

            return _isStartButtonEnable is bool condition
                ? condition
                : defaultCondition;
        }
        set
        {
            if (SetProperty(ref _isStartButtonEnable, value))
            {
				StartScriptsCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public bool? IsStopButtonEnable
    {
        get => _isStopButtonEnable is bool condition
                ? condition
                : RunningScripts.Count > 0;
        set
        {
            if (SetProperty(ref _isStopButtonEnable, value))
            {
                StopScriptsCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public ObservableCollection<ScriptExecutorViewModel> RunningScripts => _runningScripts;

    public ObservableCollection<ScriptViewModel> Scripts => _scripts;

    #endregion

    #region --Constructors--

    public ScriptsPanelViewModel() 
    {

    }

	public ScriptsPanelViewModel(
		IDataBus dataBus,
		IServiceScopeFactory serviceScopeFactory,
		IScriptExecutorViewModelFactory scriptExecutorViewModelFactory,
		IUserDialog<ScriptAddWindow> userDialog)
	{
		_scripts.CollectionChanged += (_, _) =>
	    {
		    StartScriptsCommand.NotifyCanExecuteChanged();
		    StopScriptsCommand.NotifyCanExecuteChanged();
	    };

		_runningScripts.CollectionChanged += (_, _) =>
		{
			StartScriptsCommand.NotifyCanExecuteChanged();
			StopScriptsCommand.NotifyCanExecuteChanged();
		};

		_dataBus = dataBus;
		_subscriptions.Add(_dataBus.RegisterHandler<ScriptExitedMessage>(OnScriptExited));
		_subscriptions.Add(_dataBus.RegisterHandler<ScriptAddedMessage>(OnScriptAdded));
		_serviceScopeFactory = serviceScopeFactory;
		_scriptExecutorViewModelFactory = scriptExecutorViewModelFactory;
		_userDialog = userDialog;
	}


	#endregion

	#region --Commands--

	[RelayCommand]
	private void AddScript() => _userDialog.ShowDialog();

	[RelayCommand(CanExecute = nameof(CanRemoveScript))]
    private async Task RemoveScript()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScriptService>();
        var response = await service.DeleteAsync(SelectedScript!.ScriptId);

		if (response.OperationStatus is StatusCode.Success)
		{
			await App.Current.Dispatcher.InvokeAsync(() => Scripts.Remove(SelectedScript));
		}
		_dataBus.Send(response.Description);
    }

    private bool CanRemoveScript() => SelectedScript is not null;

    [RelayCommand(CanExecute = nameof(OnCanStartScripts))]
    private async Task StartScripts()
    {
		if (StartScriptsCommand.IsRunning)
		{
			return;
		}

		IsStartButtonEnable = false;
		var scriptsToStart = Scripts.Where(e => e.IsReadyToStart).Select(e => e.ScriptId);

		var scope = _serviceScopeFactory.CreateScope();
		var service = scope.ServiceProvider.GetRequiredService<IScriptService>();

		await Task.Run(async () =>
		{
			foreach (var script in scriptsToStart)
			{
				var startingResponse = await service.StartAsync(script).ConfigureAwait(false);
				if (startingResponse.OperationStatus is StatusCode.Success)
				{
					var executorViewModel = _scriptExecutorViewModelFactory.CreateViewModel(startingResponse.Data!);
					await App.Current.Dispatcher.InvokeAsync(() => RunningScripts.Add(executorViewModel));
				}
				else
				{
					_dataBus.Send(startingResponse.Description);
				}
			}
		});

		IsStartButtonEnable = null;
		IsStopButtonEnable = null;
	}

    private bool OnCanStartScripts() => (bool)IsStartButtonEnable!;

    [RelayCommand(CanExecute = nameof(OnCanStopScripts))]
    private async Task StopScripts()
    {
		if (StartScriptsCommand.IsRunning)
		{
			return;
		}

		IsStopButtonEnable = false;
		await Task.Run(async () =>
		{
			foreach (var item in RunningScripts)
			{
				item.Stop();
				item.Dispose();
			}

			await App.Current.Dispatcher.InvokeAsync(() => RunningScripts.Clear());
		});

		_dataBus.Send($"All scripts were succsessfully stopped.");
		IsStartButtonEnable = null;
		IsStartButtonEnable = null;
	}

    private bool OnCanStopScripts() => (bool)IsStopButtonEnable!;

	#endregion

	#region --Methods--

	private async void OnScriptExited(ScriptExitedMessage message)
	{
		switch (message.ExitType)
		{
			case ExitType.ByTaskManager:
				{
					_dataBus.Send($"[{message.Sender.Title}] was killed by task manager.");
				}
				break;
			case ExitType.ByButtonInsideApp:
				{
					_dataBus.Send($"[{message.Sender.Title}] was successfully stopped.");
				}
				break;
		}

		await App.Current.Dispatcher.InvokeAsync(() =>
		{
			message.Sender.Stop();
			RunningScripts.Remove(message.Sender);
			message.Sender.Dispose();

			IsStopButtonEnable = null;
			IsStartButtonEnable = null;
		});
	}

	private async void OnScriptAdded(ScriptAddedMessage message)
	{
		await App.Current.Dispatcher.InvokeAsync(() => Scripts.Add(message.ScriptViewModel));
	}

	protected override async void OnActivated()
	{
		var scope = _serviceScopeFactory.CreateScope();
		var service = scope.ServiceProvider.GetRequiredService<IScriptService>();

		var response = await service.GetAllAsync();

		if (response.OperationStatus is StatusCode.Success)
		{
			foreach (var item in response.Data)
			{
				await App.Current.Dispatcher.InvokeAsync(() =>
				{
					Scripts.Add(item.ToViewModel());
				});
			}
		}
	}

	#endregion
}
