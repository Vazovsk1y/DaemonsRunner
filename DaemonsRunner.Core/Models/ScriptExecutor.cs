using DaemonsRunner.Core.Enums;
using DaemonsRunner.Core.Exceptions.Base;
using System.Diagnostics;
using System.Text;

namespace DaemonsRunner.Core.Models;

public class ScriptExecutor : IDisposable
{
    #region --Events--

    public event EventHandler<EventArgs>? ExitedByTaskManager;

    public event EventHandler<DataReceivedEventArgs>? OutputMessageReceived;

    #endregion

    #region --Fields--

    private readonly string _workingDirectory;

    private readonly Script _executableScript;

    private Process? _executableProcess;

    private bool _isMessagesReceivingEnable;

    private bool _isRunning;

    private bool _isDisposed = false;

    private bool _isClosedByTaskManager = true;

    #endregion

    #region --Properties--

    public Script ExecutableScript
    {
        get
        {
            ThrownExceptionIfDisposed();
            return _executableScript;
        }
    }

	public bool IsRunning
    {
        get
        {
            ThrownExceptionIfDisposed();
            return _isRunning;
        }
        private set => _isRunning = value;
    }

    public bool IsMessagesReceiving
    {
        get
        {
            ThrownExceptionIfDisposed();
            return _isMessagesReceivingEnable;
        }
        private set => _isMessagesReceivingEnable = value;
    }

    #endregion

    #region --Constructors--

    private ScriptExecutor(Script executableScript, string workingDirectory)
    {
        _executableScript = executableScript;
        _workingDirectory = workingDirectory;
    }

    #endregion

    #region --Methods--

    #region -Public-

    public static ScriptExecutor Create(Script executableScript)
    {
        ArgumentNullException.ThrowIfNull(executableScript, nameof(executableScript));
        return new ScriptExecutor(executableScript, executableScript.WorkingDirectory is not null ? executableScript.WorkingDirectory.Path : string.Empty);
    }

    public void Start()
    {
        ThrownExceptionIfDisposed();

        if (IsRunning)
        {
            return;
        }

        _executableProcess = ConfigureExecutableProcess(_executableScript.RuntimeType);
        _executableProcess.Exited += OnProcessExited;
        bool startingResult = _executableProcess.Start();
        if (!startingResult)
        {
            throw new DomainException("Process starting result was equal to false.");
        }

        IsRunning = true;
    }

    public void StartMessagesReceiving()
    {
        ThrownExceptionIfDisposed();
        if (IsMessagesReceiving)
        {
            return;
        }

        if (!IsRunning)
        {
            throw new DomainException($"{nameof(ScriptExecutor)} wasn't started. Unable to start messages receiving.");
        }

        _executableProcess!.ErrorDataReceived += OnProcessOutputDataReceived;
        _executableProcess.OutputDataReceived += OnProcessOutputDataReceived;
        _executableProcess.BeginOutputReadLine();
        _executableProcess.BeginErrorReadLine();
        IsMessagesReceiving = true;
    }

    public void ExecuteCommand()
    {
        ThrownExceptionIfDisposed();

        if (!IsRunning)
        {
            throw new DomainException($"{nameof(ScriptExecutor)} wasn't started. Unable execute command.");
        }

        _executableProcess!.StandardInput.WriteLine(ExecutableScript.Command);
        _executableProcess.StandardInput.Flush();
    }

    public void Stop()
    {
        ThrownExceptionIfDisposed();

        if (!IsRunning)
        {
            return;
        }

        _isClosedByTaskManager = false;
        StopMessagesReceiving();
        _executableProcess!.Kill(true);
        _executableProcess.Exited -= OnProcessExited;
        _executableProcess.Dispose();
        IsRunning = false;
    }

    public void StopMessagesReceiving()
    {
        ThrownExceptionIfDisposed();

        if (!IsMessagesReceiving || !IsRunning)
        {
            return;
        }

        _executableProcess!.CancelErrorRead();
        _executableProcess.CancelOutputRead();
        _executableProcess.ErrorDataReceived -= OnProcessOutputDataReceived;
        _executableProcess.OutputDataReceived -= OnProcessOutputDataReceived;
        IsMessagesReceiving = false;
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose() => CleanUp();

#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

	#endregion

	#region --EventHandlers--

	private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
	{
		const string cmdEndcodingMessage = "Active code page: 65001";
		if (!string.IsNullOrWhiteSpace(e.Data) && e.Data != cmdEndcodingMessage)
		{
			OutputMessageReceived?.Invoke(this, e);
		}
	}

	private void OnProcessExited(object? sender, EventArgs e)
    {
        if (!_isClosedByTaskManager)
        {
            return;
        }

        if (IsMessagesReceiving)
        {
            StopMessagesReceiving();
        }

        IsRunning = false;
        ExitedByTaskManager?.Invoke(this, EventArgs.Empty);
    }

	#endregion

	#region --Private--
	private void CleanUp()
    {
        if (_isDisposed)
        {
            return;
        }

        if (IsRunning)
        {
            Stop();
        }

        _executableProcess?.Dispose();
		_isDisposed = true;
    }

    private Process ConfigureExecutableProcess(RuntimeType runtimeType)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = runtimeType == RuntimeType.Cmd ? "cmd" : "powershell",
            Arguments = runtimeType == 
            RuntimeType.Cmd ? "/k chcp 65001" : "-NoExit $OutputEncoding=[Console]::InputEncoding=[Console]::OutputEncoding=New-Object System.Text.UTF8Encoding $false",      // set UTF8 endcoding to cmd/powershell output.
			RedirectStandardInput = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			WorkingDirectory = _workingDirectory,
			CreateNoWindow = true,
		};

        if (runtimeType == RuntimeType.Powershell)
        {
            processStartInfo.StandardOutputEncoding = Encoding.UTF8;
            processStartInfo.StandardErrorEncoding = Encoding.UTF8;
        }

        return new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };
    }

    private void ThrownExceptionIfDisposed() => ObjectDisposedException.ThrowIf(_isDisposed, this);

	#endregion

	#endregion
}