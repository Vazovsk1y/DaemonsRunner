using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DaemonsRunner.Application.Services.Interfaces;
using System.Windows.Input;
using DaemonsRunner.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaemonsRunner.WPF.Infrastructure.Messages;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptExecutorViewModel : ObservableObject, IDisposable
{
    #region --Fields--

    private readonly IDataBus _dataBus;
    private bool _disposed = false;
	private readonly ScriptExecutor _scriptExecutor;

	#endregion

	#region --Properties--

	public ObservableCollection<string> OutputMessages { get; } = new ObservableCollection<string>();

    public string Title => $"Executable script [{_scriptExecutor.ExecutableScript.Title}].";

    #endregion

    #region --Constructors--

    public ScriptExecutorViewModel(
        ScriptExecutor scriptExecutor,
        IDataBus dataBus)
    {
        _scriptExecutor = scriptExecutor;
        _scriptExecutor.OutputMessageReceived += OnScriptOutputMessageReceived;
        _scriptExecutor.ExitedByTaskManager += OnScriptExitedByTaskManager;
        _dataBus = dataBus;
    }

    #endregion

    #region --Commands--

    public ICommand StopScriptCommand => new RelayCommand(() =>
    {
        _dataBus.Send(new ScriptExitedMessage(this));
    });

    #endregion

    #region --Methods--

    public void Dispose() => CleanUp();

    public void Stop() => _scriptExecutor.Stop();

    private void OnScriptExitedByTaskManager(object? sender, EventArgs e) => _dataBus.Send(new ScriptExitedMessage(this, true)); 

    private async Task OnScriptOutputMessageReceived(object sender, string message) => 
        await App.Current.Dispatcher.InvokeAsync(() => OutputMessages.Add($"[{DateTime.Now.ToShortTimeString()}]: {message!}"));

    private async void CleanUp()
    {
        if (_disposed)
        {
            return;
        }

        _scriptExecutor.OutputMessageReceived -= OnScriptOutputMessageReceived;
        _scriptExecutor.ExitedByTaskManager -= OnScriptExitedByTaskManager;
        _scriptExecutor.Dispose();
        await App.Current.Dispatcher.InvokeAsync(OutputMessages.Clear);
        _disposed = true;
    }

    #endregion
}
