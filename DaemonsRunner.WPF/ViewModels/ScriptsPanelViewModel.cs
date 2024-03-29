﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.Application.Responses;
using DaemonsRunner.Application.Services.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using DaemonsRunner.Services;
using DaemonsRunner.WPF.Infrastructure.Extensions;
using DaemonsRunner.WPF.Infrastructure.Messages;
using DaemonsRunner.WPF.ViewModels.Interfaces;
using DaemonsRunner.WPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptsPanelViewModel : ObservableRecipient
{
    #region --Fields--

    private readonly IDataBus _dataBus;
    private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly IUserDialog<ScriptAddWindow> _scriptAddDialog;
    private readonly ObservableCollection<ScriptViewModel> _scripts = new();
	private readonly IScriptExecutorViewModelFactory _scriptExecutorViewModelFactory;
    private readonly ICollection<IDisposable> _subscriptions = new List<IDisposable>();
	private readonly ObservableCollection<ScriptExecutorViewModel> _runningScripts = new();
    private bool? _isStartButtonEnable = null;
    private bool? _isStopButtonEnable = null;

    #endregion

    #region --Properties--

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveScriptsCommand))]
	[NotifyCanExecuteChangedFor(nameof(StartScriptsCommand))]
	private ScriptViewModel? _selectedScript;

	public bool? IsStartButtonEnable
    {
        get
        {
            bool defaultCondition = SelectedScripts.Any() &&
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

	public IEnumerable<ScriptViewModel> SelectedScripts => Scripts.Where(e => e.IsSelected).ToList();

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
			RemoveScriptsCommand.NotifyCanExecuteChanged();
			AddScriptCommand.NotifyCanExecuteChanged();
		};

		_dataBus = dataBus;
		_subscriptions.Add(_dataBus.RegisterHandler<ScriptExitedMessage>(OnScriptExited));
		_subscriptions.Add(_dataBus.RegisterHandler<ScriptAddedMessage>(OnScriptAdded));
		_serviceScopeFactory = serviceScopeFactory;
		_scriptExecutorViewModelFactory = scriptExecutorViewModelFactory;
		_scriptAddDialog = userDialog;
	}


	#endregion

	#region --Commands--

	[RelayCommand(CanExecute = nameof(CanAddScript))]
	private void AddScript() => _scriptAddDialog.ShowDialog();

	private bool CanAddScript() => RunningScripts.Count == 0;

	[RelayCommand(CanExecute = nameof(CanRemoveScripts))]
    private async Task RemoveScripts()
    {
		if (RemoveScriptsCommand.IsRunning)
		{
			return;
		}

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScriptService>();

		foreach (var script in SelectedScripts)
		{
			var response = await service.DeleteAsync(script.ScriptId);
			if (response.OperationStatus is StatusCode.Success)
			{
				await App.Current.Dispatcher.InvokeAsync(() => Scripts.Remove(script));
			}
			_dataBus.Send(response.Description);
		}
	}

    private bool CanRemoveScripts() => SelectedScripts.Any() && RunningScripts.Count == 0;

    [RelayCommand(CanExecute = nameof(OnCanStartScripts))]
    private async Task StartScripts()
    {
		if (StopScriptsCommand.IsRunning || StartScriptsCommand.IsRunning)
		{
			return;
		}

		IsStartButtonEnable = false;

		var scope = _serviceScopeFactory.CreateScope();
		var service = scope.ServiceProvider.GetRequiredService<IScriptService>();

		await Task.Run(async () =>
		{
			foreach (var script in SelectedScripts)
			{
				var startingResponse = await service.StartAsync(new StartScriptOptions(script.ScriptId)).ConfigureAwait(false);
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
		if (StopScriptsCommand.IsRunning || StartScriptsCommand.IsRunning)
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
		if (message.ExitedByTaskManager)
		{
            _dataBus.Send($"[{message.Sender.Title}] was killed by task manager.");
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScripts.Remove(message.Sender);
                message.Sender.Dispose();

                IsStopButtonEnable = null;
                IsStartButtonEnable = null;
            });
			return;
        }

        _dataBus.Send($"[{message.Sender.Title}] was successfully stopped.");
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
