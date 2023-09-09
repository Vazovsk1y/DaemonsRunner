using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using System.Windows.Input;
using DaemonsRunner.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DaemonsRunner.ViewModels;

internal partial class ScriptExecutorViewModel : ObservableObject, IDisposable
{
    #region --Fields--

    private readonly IDataBus _dataBus;
    private bool _disposed = false;
	private readonly ScriptExecutor _scriptExecutor;

	#endregion

	#region --Properties--

	public ObservableCollection<string> OutputMessages { get; } = new ObservableCollection<string>();

    public string Title => _scriptExecutor.Title;

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
        _dataBus.Send(new ScriptExitedMessage(this, ExitType.ByButtonInsideApp));
    });

    #endregion

    #region --Methods--

    public void Dispose() => CleanUp();

    public void Stop() => _scriptExecutor.Stop();

    private void OnScriptExitedByTaskManager(object? sender, EventArgs e) => _dataBus.Send(new ScriptExitedMessage(this, ExitType.ByTaskManager)); 

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
